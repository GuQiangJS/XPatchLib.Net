// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Xml;

namespace XPatchLib
{
    /// <summary>
    ///     复杂类型增量内容合并类。
    /// </summary>
    internal class CombineObject : CombineBase
    {
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
                //读取除Action以外的所有Action，将其赋值给属性
                foreach (var variable in Attributes.KeysValuePairs)
                {
                    MemberWrapper member;
                    if (TryFindMember(variable.Key, out member))
                    {
                        Type.SetMemberValue(pOriObject, variable.Key, variable.Value);
#if DEBUG
                        Debug.WriteLine(string.Format("{2} SetMemberValue: {0}={1}", variable.Key, variable.Value,
                            Type.TypeFriendlyName));
#endif
                    }
                }

                if (pReader.NodeType == NodeType.Element)
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
                                new EnumWrapper(memberType).TransFromString(pReader.ReadString()));
#if (NET || NETSTANDARD_2_0_UP)
                        else  if (member.IsColor)
                            Type.SetMemberValue(pOriObject, member.Name,
                                ColorHelper.TransFromString(pReader.ReadString()));
#endif
                        else
                            Type.SetMemberValue(pOriObject, member.Name,
                                CombineInstanceContainer.GetCombineInstance(TypeExtendContainer.GetTypeExtend(memberType, null, Type)).Combine(pReader,
                                    pOriObject, member.Name));
                    }
                    else if (member.IsIEnumerable)
                    {
                        //当前处理的属性的类型是 集合 类型时
                        Object memberObj = Type.GetMemberValue(pOriObject, member.Name);

                        memberObj =
                            CombineInstanceContainer.GetCombineInstance(TypeExtendContainer.GetTypeExtend(memberType, null, Type)).Combine(
                                pReader, memberObj, member.Name);
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
                            memberObj = TypeExtendContainer.GetTypeExtend(memberType, null, Type).CreateInstance();
                            created = true;
                        }
                        //调用CombineObject类型的Combine方法，对现有属性实例（或新创建的属性实例）进行增量数据合并。
                        CombineInstanceContainer.GetCombineInstance(TypeExtendContainer.GetTypeExtend(memberType, null, Type)).Combine(pReader, memberObj,
                            member.Name);
                        //将数据合并后的实例赋值给当前正在处理的属性，替换原有的数据实例。实现数据合并功能。

                        if (!(created && TypeExtendContainer.GetTypeExtend(memberType, null, Type).CreateInstance().Equals(memberObj)))
                            Type.SetMemberValue(pOriObject, member.Name, memberObj);
                    }
                    if (string.Equals(pReader.Name, member.Name))
                        pReader.Read();
                    continue;
                }

                //如果不是当前正在读取的节点的结束标记，就一直往下读
                pReader.Read();
            }
            return pOriObject;
        }

        #region Internal Constructors

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.CombineObject" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        internal CombineObject(TypeExtend pType)
            : base(pType)
        {
        }

        #endregion Internal Constructors
    }
}