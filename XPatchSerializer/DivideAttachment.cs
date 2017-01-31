// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace XPatchLib
{
    /// <summary>
    ///     生成增量时可能用到的附件类型定义。
    /// </summary>
    internal class DivideAttachment
    {
        public DivideAttachment()
        {
            ParentQuere = new Queue<ParentObject>();
            CurrentAction = Action.Edit;
        }

        /// <summary>
        ///     获取当前元素的父级元素队列
        /// </summary>
        public Queue<ParentObject> ParentQuere { get; private set; }

        public Object CurrentObj { get; set; }

        /// <summary>
        ///     获取或设置当前元素的主键名称数组。
        /// </summary>
        public string[] PrimaryKeys { get; set; }

        /// <summary>
        ///     获取父级对象类型。
        /// </summary>
        public TypeExtend CurrentType { get; set; }

        /// <summary>
        ///     获取或设置当前正在进行的操作。
        /// </summary>
        public Action CurrentAction { get; set; }
    }

    internal struct ParentObject
    {
        public ParentObject(String pName, Object pCurrentObj, TypeExtend pType)
        {
            Name = pName;
            Type = pType;
            Action = Action.Edit;
            CurrentObj = pCurrentObj;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ParentObject)
            {
                ParentObject parentObject = (ParentObject) obj;
                return string.Equals(parentObject.Name, Name) && Equals(parentObject.Type, Type);
            }
            return false;
        }

        public Object CurrentObj { get; set; }

        /// <summary>
        ///     获取父级对象类型。
        /// </summary>
        public TypeExtend Type { get; }

        /// <summary>
        ///     获取父级元素名称。
        /// </summary>
        public String Name { get; }

        public Action Action { get; set; }
    }
}