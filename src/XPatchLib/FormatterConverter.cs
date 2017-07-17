#if NET || NETSTANDARD_2_0_UP

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace XPatchLib
{

    internal class FormatterConverter : IFormatterConverter
    {
        ISerializeSetting _setting;
        internal FormatterConverter(ISerializeSetting setting)
        {
            _setting = setting;
        }

        private T Convert<T>(TypeCode typeCode, object value)
        {
            return (T)CombineBasic.CombineAction(typeCode, false, _setting.Mode, value.ToString());
        }

        public object Convert(object value, Type type)
        {
            return System.Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        public object Convert(object value, System.TypeCode typeCode)
        {
            return System.Convert.ChangeType(value, typeCode, CultureInfo.InvariantCulture);
        }

        public bool ToBoolean(object value)
        {
            return (value == null) ? default(Boolean) : Convert<Boolean>(TypeCode.Boolean, value);
        }

        public byte ToByte(object value)
        {
            return (value == null) ? default(Byte) : Convert<Byte>(TypeCode.Byte, value);
        }

        public char ToChar(object value)
        {
            return (value == null) ? default(Char) : Convert<Char>(TypeCode.Char, value);
        }

        public DateTime ToDateTime(object value)
        {
            return (value == null) ? default(DateTime) : Convert<DateTime>(TypeCode.DateTime, value);
        }

        public decimal ToDecimal(object value)
        {
            return (value == null) ? default(Decimal) : Convert<Decimal>(TypeCode.Decimal, value);
        }

        public double ToDouble(object value)
        {
            return (value == null) ? default(Double) : Convert<Double>(TypeCode.Double, value);
        }

        public short ToInt16(object value)
        {
            return (value == null) ? default(Int16) : Convert<Int16>(TypeCode.Int16, value);
        }

        public int ToInt32(object value)
        {
            return (value == null) ? default(Int32) : Convert<Int32>(TypeCode.Int32, value);
        }

        public long ToInt64(object value)
        {
            return (value == null) ? default(Int64) : Convert<Int64>(TypeCode.Int64, value);
        }

        public sbyte ToSByte(object value)
        {
            return (value == null) ? default(SByte) : Convert<SByte>(TypeCode.SByte, value);
        }

        public float ToSingle(object value)
        {
            return (value == null) ? default(Single) : Convert<Single>(TypeCode.Single, value);
        }

        public string ToString(object value)
        {
            return (value == null) ? default(String) : Convert<String>(TypeCode.String, value);
        }

        public ushort ToUInt16(object value)
        {
            return (value == null) ? default(UInt16) : Convert<UInt16>(TypeCode.UInt16, value);
        }

        public uint ToUInt32(object value)
        {
            return (value == null) ? default(UInt32) : Convert<UInt32>(TypeCode.UInt32, value);
        }

        public ulong ToUInt64(object value)
        {
            return (value == null) ? default(UInt64) : Convert<UInt64>(TypeCode.UInt64, value);
        }
    }
}
#endif