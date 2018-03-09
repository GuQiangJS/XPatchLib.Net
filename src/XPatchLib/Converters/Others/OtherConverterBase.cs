// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
#if !NET_20
using System.Linq;
#endif
#if (NET_35_UP || NETSTANDARD)
#else
using XPatchLib.NoLinq;
#endif

namespace XPatchLib
{
    internal abstract class OtherConverterBase : ConverterBase
    {
        protected OtherConverterBase(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        protected OtherConverterBase(TypeExtend pType) : base(pType)
        {
        }

        #region Divide

        internal abstract MemberWrapper[] DivideMembers { get; }

        protected abstract object GetMemberValue(MemberWrapper member, object Obj);

        /// <summary>
        ///     产生增量内容的实际方法。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。</returns>
        protected override bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null)
        {
            var result = false;
            foreach (var member in DivideMembers)
            {
                var pOriMemberValue = GetMemberValue(member, pOriObject);
                var pRevMemberValue = GetMemberValue(member, pRevObject);

                if (!TypeExtend.NeedSerialize(member.DefaultValue, pOriMemberValue, pRevMemberValue,
                    Writer.Setting.SerializeDefalutValue))
                    continue;

                var memberType = member.MemberType;

                ConverterBase divide;

                //基础类型 ReflectionUtils.IsBasicType
                if (member.IsBasicType)
                {
                    //如果是枚举类型，则先将枚举值转化为字符串
                    if (member.IsEnum)
                    {
                        pOriMemberValue = new EnumWrapper(memberType).TransToString(pOriMemberValue);
                        pRevMemberValue = new EnumWrapper(memberType).TransToString(pRevMemberValue);
                        memberType = typeof(string);
                    }
#if (NET || NETSTANDARD_2_0_UP)
                    //如果是Color类型
                    if (member.IsColor)
                    {
                        pOriMemberValue = ColorHelper.TransToString(pOriMemberValue);
                        pRevMemberValue = ColorHelper.TransToString(pRevMemberValue);
                        memberType = typeof(string);
                    }
#endif
                    divide = new ConverterBasic(Writer,
                        TypeExtendContainer.GetTypeExtend(Writer.Setting, memberType,
                            Writer.Setting.IgnoreAttributeType,
                            Type, member.MemberInfo));
                }
                //集合类型
                else if (member.IsIEnumerable)
                {
                    divide = new ConverterIEnumerable(Writer,
                        TypeExtendContainer.GetTypeExtend(Writer.Setting, memberType,
                            Writer.Setting.IgnoreAttributeType,
                            Type, member.MemberInfo));
                }
                else
                {
                    divide = new ConverterCore(Writer,
                        TypeExtendContainer.GetTypeExtend(Writer.Setting, memberType,
                            Writer.Setting.IgnoreAttributeType,
                            Type, member.MemberInfo));
                }

                if (pAttach == null)
                    pAttach = new DivideAttachment();
                if (!result)
                {
                    ParentObject parentObject = new ParentObject(pName, pOriObject, Type, GetType(pOriObject, pRevObject));
                    if (pAttach.ParentQuere.Count <= 0 || !pAttach.ParentQuere.Last().Equals(parentObject))
                    {
                        //将当前节点加入附件中，如果遇到子节点被写入前，会首先根据队列先进先出写入附件中的节点的开始标记
                        //只有当没有写入过的情况下才需要写入父节点
                        pAttach.ParentQuere.Enqueue(new ParentObject(pName, pOriObject, Type, GetType(pOriObject, pRevObject))
                        {
                            Action = pAttach.CurrentAction
                        });
                        pAttach.CurrentAction = Action.Edit;
                    }
                }

                //只要成功写入过一个节点，就表示父节点都写入过了
                divide.ParentElementWrited = result;
                divide.Assign(this);
                var childResult = divide.Divide(member.Name, pOriMemberValue, pRevMemberValue, pAttach);
                //if (childResult)
                //{
                //    //结束子节点标记
                //    Writer.WriteEndProperty();
                //}

                if (!result)
                    result = childResult;
            }

            ////如果有子节点成功写入，则需要写入当前节点的关闭。
            //if (result)
            //        Writer.WriteEndObject();
            return result;
        }

        #endregion

        #region Combine

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            Dictionary<string, object> values = GetOriValues(pOriObject, StringComparer.OrdinalIgnoreCase);
            //Dictionary<string, object> revValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;

                //pReader.MoveToElement();

                if (string.Equals(pName, pReader.Name))
                {
                    pReader.Read();
                    continue;
                }

                if (pReader.NodeType == NodeType.Element)
                {
                    string proName = pReader.Name;

                    if (values.ContainsKey(proName))
                        values[proName] = GetMemberValue(proName, pOriObject, pReader);
                    else
                        values.Add(proName, GetMemberValue(proName, pOriObject, pReader));
                }

                pReader.Read();
            }

            return CreateInstance(values);
        }

        /// <summary>
        ///     获取现有对象需要比较的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected abstract Dictionary<string, object> GetOriValues(object obj, IEqualityComparer<string> comparer);

        protected abstract object CreateInstance(Dictionary<string, object> values);

        protected abstract object GetMemberValue(string proName, object pObj, ITextReader pReader);

        #endregion
    }
}