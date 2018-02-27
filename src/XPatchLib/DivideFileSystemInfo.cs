// Copyright © 2013-2018 - GuQiang55
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Reflection;

namespace XPatchLib
{
#if NET || NETSTANDARD_2_0_UP
    internal class DivideDriveInfo : DivideBase
    {
        protected override bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null)
        {
            Boolean result;

            DriveInfo oriObj = pOriObject as DriveInfo;
            DriveInfo revObj = pRevObject as DriveInfo;

            PropertyInfo pi = Type.OriType.GetProperty(CombineDriveInfo.NameName,
                BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);

            object oriValue = (oriObj != null) ? pi.GetValue(oriObj, null) : null;
            object revValue = (revObj != null) ? pi.GetValue(revObj, null) : null;

            TypeExtend t =
                TypeExtendContainer.GetTypeExtend(Writer.Setting, typeof(string), Writer.Setting.IgnoreAttributeType,
                    Type);

            DivideBase d = new DivideCore(Writer, t);
            d.Assign(this);

            if (pAttach == null)
                pAttach = new DivideAttachment();
            //将当前节点加入附件中，如果遇到子节点被写入前，会首先根据队列先进先出写入附件中的节点的开始标记
            pAttach.ParentQuere.Enqueue(new ParentObject(pName, pOriObject, Type));

            result = d.Divide(CombineDriveInfo.NameName, oriValue, revValue, pAttach);
            //if (!result)
            //    result = childResult;
            return result;
        }

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideBase" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pWriter" /> is null 时。</exception>
        public DivideDriveInfo(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }
    }
#endif

#if NET || NETSTANDARD_1_3_UP

    internal class DivideFileSystemInfo : DivideBase
    {
        protected override bool DivideAction(string pName, object pOriObject, object pRevObject,
            DivideAttachment pAttach = null)
        {
            Boolean result;

            FileSystemInfo oriObj = pOriObject as FileSystemInfo;
            FileSystemInfo revObj = pRevObject as FileSystemInfo;

            FieldInfo fi = Type.OriType.GetField(CombineFileSystemInfo.OriginalPathName,
                BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

            object oriValue = (oriObj != null) ? fi.GetValue(oriObj) : null;
            object revValue = (revObj != null) ? fi.GetValue(revObj) : null;

            TypeExtend t =
                TypeExtendContainer.GetTypeExtend(Writer.Setting, typeof(string), Writer.Setting.IgnoreAttributeType,
                    Type);

            DivideBase d = new DivideCore(Writer, t);
            d.Assign(this);

            if (pAttach == null)
                pAttach = new DivideAttachment();
            //将当前节点加入附件中，如果遇到子节点被写入前，会首先根据队列先进先出写入附件中的节点的开始标记
            pAttach.ParentQuere.Enqueue(new ParentObject(pName, pOriObject, Type));

            result = d.Divide(CombineFileSystemInfo.OriginalPathName, oriValue, revValue, pAttach);
            //if (!result)
            //    result = childResult;
            return result;
        }

        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.DivideBase" /> 类的新实例。
        /// </summary>
        /// <param name="pWriter">写入器。</param>
        /// <param name="pType">指定的类型。</param>
        /// <exception cref="PrimaryKeyException">当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。</exception>
        /// <exception cref="ArgumentNullException">当参数 <paramref name="pWriter" /> is null 时。</exception>
        public DivideFileSystemInfo(ITextWriter pWriter, TypeExtend pType) : base(pWriter, pType)
        {
        }
    }
#endif
}