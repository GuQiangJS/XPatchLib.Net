// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;

namespace XPatchLib
{
    /// <summary>
    ///     XML类型写入器的默认设置。
    /// </summary>
    /// <seealso cref="XPatchLib.ISerializeSetting" />
    public class XmlSerializeSetting : SerializeSetting
    {
        /// <summary>创建作为当前实例副本的新对象。</summary>
        /// <returns>作为此实例副本的新对象。</returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            XmlSerializeSetting result=new XmlSerializeSetting();
            result.MemberType = this.MemberType;
#if NET || NETSTANDARD_2_0_UP
            result.EnableOnDeserializedAttribute = this.EnableOnDeserializedAttribute;
            result.EnableOnSerializedAttribute = this.EnableOnSerializedAttribute;
            result.EnableOnDeserializingAttribute = this.EnableOnDeserializingAttribute;
            result.EnableOnSerializingAttribute = this.EnableOnSerializingAttribute;
#endif
            result.ActionName = this.ActionName;
            result.Mode = this.Mode;
            result.Modifier = this.Modifier;
            result.SerializeDefalutValue = this.SerializeDefalutValue;
            return result;
        }
    }
}