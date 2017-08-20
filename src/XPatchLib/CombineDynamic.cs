// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NET_40_UP|| NETSTANDARD_2_0_UP

using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;

namespace XPatchLib
{
    /// <summary>
    ///     动态类型增量内容产生类。
    /// </summary>
    /// <seealso cref="XPatchLib.DivideBase" />
    internal class CombineDynamic : CombineObject
    {
        /// <summary>
        ///     使用指定的类型初始化 <see cref="CombineDynamic" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        public CombineDynamic(TypeExtend pType) : base(pType)
        {
        }

        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            //当原始对象为null时，先创建一个实例。
            if (pOriObject == null)
                pOriObject = Type.CreateInstance();

            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase))
                    if (pReader.NodeType == NodeType.Element)
                    {
                        pReader.Read();
                        continue;
                    }
                    else if (pReader.NodeType == NodeType.EndElement)
                    {
                        break;
                    }

                Type memberType = null;

                //读取除Action以外的所有Action，将其赋值给属性
                for (int i = 0; i < Attributes.Count; i++)
                {
                    MemberWrapper member;
                    String key = Attributes.Keys[i];
                    if (TryFindMember(key, out member))
                    {
                        Type.SetMemberValue(pOriObject, key, Attributes.Values[i]);
#if DEBUG
                        Debug.WriteLine("{2} SetMemberValue: {0}={1}", key, Attributes.Values[i],
                            Type.TypeFriendlyName);
#endif
                    }
                }

                string[,] curAttrs = pReader.GetAttributes();
                string assembly = string.Empty;
                Action action = Action.Edit;
                for (int i = 0; i < curAttrs.GetLength(0); i++)
                {
                    if (curAttrs[i, 0] == pReader.Setting.AssemblyQualifiedName)
                    {
                        memberType = System.Type.GetType(curAttrs[i, 1]);
                        assembly = curAttrs[i, 1];
                        continue;
                    }
                    if (curAttrs[i, 0] == pReader.Setting.ActionName)
                    {
                        ActionHelper.TryParse(curAttrs[i, 1], out action);
                    }
                }

                if (string.IsNullOrEmpty(assembly))
                    throw new InvalidOperationException(
                        ResourceHelper.GetResourceString(LocalizationRes.Exp_String_TypeAssemblyQualifiedNameNotFound,
                            pReader.Name));

                if (memberType == null)
                    throw new FileNotFoundException(
                        ResourceHelper.GetResourceString(LocalizationRes.Exp_String_TypeAssemblyQualifiedNameNotFound,
                            assembly));

                string curName = pReader.Name;

                if (pReader.NodeType == NodeType.Element || pReader.NodeType == NodeType.FullElement)
                {
                    DynamicObject oriValue = pOriObject as DynamicObject;
                    if (oriValue != null)
                        if (action != Action.SetNull)
                        {
                            oriValue.SetMemberValue(pReader.Name, CombineInstanceContainer
                                .GetCombineInstance(
                                    TypeExtendContainer.GetTypeExtend(pReader.Setting, memberType, null, Type))
                                .Combine(pReader, pOriObject, pReader.Name));
                        }
                        else
                        {
                            oriValue.SetMemberValue(pReader.Name, null);
                        }

                    while (!(curName == pReader.Name && (pReader.NodeType == NodeType.EndElement ||
                                                         pReader.NodeType == NodeType.FullElement)))
                    {
                        pReader.Read();
                    }
                }

                //如果不是当前正在读取的节点的结束标记，就一直往下读
                pReader.Read();
            }
            return pOriObject;
        }
    }
}
#endif