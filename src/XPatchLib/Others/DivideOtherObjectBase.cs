﻿// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
#if !NET_20
using System.Linq;
#endif

#if (NET_35_UP || NETSTANDARD)

#else 
using XPatchLib.NoLinq;
#endif

namespace XPatchLib.Others
{
    internal abstract class DivideOtherObjectBase : DivideBase
    {
        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideBase" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pWriter" /> is null 时。</exception>
        public DivideOtherObjectBase(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

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
            foreach (var member in DivideAndCombineRegex.Fields)
            {
                var pOriMemberValue = GetMemberValue(member, pOriObject);
                var pRevMemberValue = GetMemberValue(member, pRevObject);

                if (!TypeExtend.NeedSerialize(member.DefaultValue, pOriMemberValue, pRevMemberValue,
                    Writer.Setting.SerializeDefalutValue))
                    continue;

                var memberType = member.MemberType;

                DivideBase divide;

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
                    divide = new DivideBasic(Writer,
                        TypeExtendContainer.GetTypeExtend(Writer.Setting, memberType,
                            Writer.Setting.IgnoreAttributeType,
                            Type, member.MemberInfo));
                }
                //集合类型
                else if (member.IsIEnumerable)
                {
                    divide = new DivideIEnumerable(Writer,
                        TypeExtendContainer.GetTypeExtend(Writer.Setting, memberType,
                            Writer.Setting.IgnoreAttributeType,
                            Type, member.MemberInfo));
                }
                else
                {
                    divide = new DivideCore(Writer,
                        TypeExtendContainer.GetTypeExtend(Writer.Setting, memberType,
                            Writer.Setting.IgnoreAttributeType,
                            Type, member.MemberInfo));
                }

                if (pAttach == null)
                    pAttach = new DivideAttachment();
                if (!result)
                {
                    ParentObject parentObject = new ParentObject(pName, pOriObject, Type);
                    if (pAttach.ParentQuere.Count <= 0 || !pAttach.ParentQuere.Last().Equals(parentObject))
                    {
                        //将当前节点加入附件中，如果遇到子节点被写入前，会首先根据队列先进先出写入附件中的节点的开始标记
                        //只有当没有写入过的情况下才需要写入父节点
                        pAttach.ParentQuere.Enqueue(new ParentObject(pName, pOriObject, Type)
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
    }
}