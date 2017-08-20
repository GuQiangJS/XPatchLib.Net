// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if NET_40_UP|| NETSTANDARD_2_0_UP

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace XPatchLib
{
    /// <summary>
    ///     动态类型增量内容产生类。
    /// </summary>
    /// <seealso cref="XPatchLib.DivideBase" />
    internal class DivideDynamic : DivideBase
    {
        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideBasic" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        public DivideDynamic(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }

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

            DynamicObject oriObj=pOriObject as DynamicObject;
            DynamicObject revObj=pRevObject as DynamicObject;

            List<string> names = new DynamicObject[] {oriObj, revObj}.GetMembers();

            foreach (string name in names)
            {
                object oriValue = oriObj.GetMemberValue(name);
                object revValue = revObj.GetMemberValue(name);
                Type memberType = GetType(oriValue, revValue);

                TypeExtend t =
                    TypeExtendContainer.GetTypeExtend(Writer.Setting, memberType, Writer.IgnoreAttributeType, Type);

                DivideBase d = new DivideCore(Writer, t);
                d.Assign(this);

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

                var childResult = d.Divide(name, oriValue, revValue, pAttach); if (!result)
                    result = childResult;
            }
            return result;
        }

        Type GetType(params object[] o)
        {
            foreach (object dynamicObject in o)
            {
                if (dynamicObject == null) continue;
                return dynamicObject.GetType();
            }
            return typeof(object);
        }
    }
}
#endif