// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
#if (NET_35_UP || NETSTANDARD)
using System.Linq;

#endif

namespace XPatchLib
{
    internal class ConverterObject : ConverterBase
    {
        public ConverterObject(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

        public ConverterObject(TypeExtend pType) : base(pType)
        {
        }

        /// <summary>
        ///     产生增量内容的实际方法。
        /// </summary>
        /// <param name="pName">增量内容对象的名称。</param>
        /// <param name="pOriObject">原始对象。</param>
        /// <param name="pRevObject">更新后的对象。</param>
        /// <param name="pAttach">生成增量时可能用到的附件。</param>
        /// <returns>
        ///     返回是否成功写入内容。如果成功写入返回 <c>true</c> ，否则返回 <c>false</c> 。
        /// </returns>
        protected override bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null)
        {
            var result = false;
            //遍历当前的类型上可以序列化的属性集合，逐个产生增量内容，之后再加入整个类型实例的增量结果result中
            IEnumerable<MemberWrapper> members = Type.FieldsToBeSerialized;
            foreach (var member in members)
            {
                var pOriMemberValue = Type.GetMemberValue(pOriObject, member.Name);
                var pRevMemberValue = Type.GetMemberValue(pRevObject, member.Name);

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
                        pAttach.ParentQuere.Enqueue(new ParentObject(pName, pOriObject, Type,GetType(pOriObject,pRevObject))
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

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            //当原始对象为null时，先创建一个实例。
            if (pOriObject == null)
                pOriObject = Type.CreateInstance();

            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;
                //if (Attributes.Keys != null)
                //{
                //读取除Action以外的所有Action，将其赋值给属性
                for (int i = 0; i < Attributes.Count; i++)
                {
                    MemberWrapper member;
                    String key = Attributes.Keys[i];
                    if (TryFindMember(key, out member))
                    {
                        Type.SetMemberValue(pOriObject, key, Attributes.Values[i]);
#if DEBUG
                        Debug.WriteLine(string.Format("{2} SetMemberValue: {0}={1}", key, Attributes.Values[i],
                            Type.TypeFriendlyName));
#endif
                    }
                }
                //}

                if (pReader.NodeType == NodeType.Element || pReader.NodeType == NodeType.FullElement)
                {
                    MemberWrapper member;

                    if (!TryFindMember(pReader.Name, out member))
                    {
                        pReader.Read();
                        continue;
                    }

                    Type memberType = ReflectionUtils.IsNullable(member.Type)
                        ? ReflectionUtils.GetNullableValueType(member.Type)
                        : member.Type;

                    if (member.IsBasicType)
                    {
                        //当前处理的属性的类型是 基础 类型时
                        if (member.IsEnum)
                            Type.SetMemberValue(pOriObject, member.Name,
                                new EnumWrapper(memberType).TransFromString(pReader.GetValue()));
#if (NET || NETSTANDARD_2_0_UP)
                        else if (member.IsColor)
                            Type.SetMemberValue(pOriObject, member.Name,
                                ColorHelper.TransFromString(pReader.GetValue()));
#endif
                        else
                            Type.SetMemberValue(pOriObject, member.Name,
                                CombineInstanceContainer
                                    .GetCombineInstance(
                                        TypeExtendContainer.GetTypeExtend(pReader.Setting, memberType, null, Type))
                                    .Combine(pReader, pOriObject, member.Name));
                    }
                    else if (member.IsIEnumerable)
                    {
                        //当前处理的属性的类型是 集合 类型时
                        Object memberObj = Type.GetMemberValue(pOriObject, member.Name);

                        memberObj =
                            CombineInstanceContainer
                                .GetCombineInstance(
                                    TypeExtendContainer.GetTypeExtend(pReader.Setting, memberType, null, Type))
                                .Combine(pReader, memberObj, member.Name);
                        Type.SetMemberValue(pOriObject, member.Name, memberObj);
                    }
                    else
                    {
                        //当前处理的属性的类型是 复杂 类型时
                        //当前正在处理的属性的现有属性值实例
                        Object memberObj = Type.GetMemberValue(pOriObject, member.Name);
                        bool created = false;
                        //如果当前正在处理的属性的现有属性值实例为null时，先创建一个新的实例。
                        if (memberObj == null)
                        {
                            memberObj = TypeExtendContainer.GetTypeExtend(pReader.Setting, memberType, null, Type)
                                .CreateInstance();
                            created = true;
                        }

                        //调用CombineObject类型的Combine方法，对现有属性实例（或新创建的属性实例）进行增量数据合并。
                        CombineInstanceContainer
                            .GetCombineInstance(
                                TypeExtendContainer.GetTypeExtend(pReader.Setting, memberType, null, Type))
                            .Combine(pReader, memberObj, member.Name);
                        //将数据合并后的实例赋值给当前正在处理的属性，替换原有的数据实例。实现数据合并功能。

                        if (!(created && TypeExtendContainer.GetTypeExtend(pReader.Setting, memberType, null, Type)
                                  .CreateInstance().Equals(memberObj)))
                            Type.SetMemberValue(pOriObject, member.Name, memberObj);
                    }

                    while (string.Equals(pReader.Name, member.Name)) pReader.Read();
                    continue;
                }

                //如果不是当前正在读取的节点的结束标记，就一直往下读
                pReader.Read();
            }

            return pOriObject;
        }
    }
}