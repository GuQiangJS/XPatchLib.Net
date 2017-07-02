// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if BENCHMARK

using System;
using System.Security.Cryptography;

namespace XPatchLib.UnitTest.Benchmarks
{
    public static class BenchmarkHelper
    {
        private static readonly DateTime _minDate = new DateTime(1990, 1, 1);
        private static readonly DateTime _maxDate = DateTime.Now.Date;

        private static readonly Random _random = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        ///     生成随机字符串
        /// </summary>
        /// <param name="length">目标字符串的长度</param>
        /// <param name="useNum">是否包含数字，1=包含，默认为包含</param>
        /// <param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        /// <param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        /// <param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        /// <param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        /// <returns>指定长度的随机字符串</returns>
        public static string RandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            string s = null, str = custom;
            if (useNum) str += "0123456789";
            if (useLow) str += "abcdefghijklmnopqrstuvwxyz";
            if (useUpp) str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (useSpe) str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            for (int i = 0; i < length; i++)
                s += str.Substring(_random.Next(0, str.Length - 1), 1);
            return s;
        }

        /// <summary>
        ///     随机datetime
        /// </summary>
        /// <returns></returns>
        public static DateTime RandomDate()
        {
            return RandomDate(_minDate, _maxDate);
        }

        /// <summary>
        ///     随机datetime
        /// </summary>
        /// <returns></returns>
        public static DateTime RandomDate(DateTime minDate, DateTime maxDate)
        {
            int totalDays = (int) maxDate.Subtract(minDate).TotalDays;
            int randomDays = _random.Next(0, totalDays);
            return minDate.AddDays(randomDays);
        }

        /// <summary>
        ///     随机bool
        /// </summary>
        /// <returns></returns>
        public static bool RandomBoolean()
        {
            return DateTime.Now.Second % 2 > 0;
        }

        /// <summary>
        ///     随机char
        /// </summary>
        /// <returns></returns>
        public static char RandomChar()
        {
            return Convert.ToChar(Convert.ToInt32(26 * _random.NextDouble() + 64));
        }

        /// <summary>
        ///     随机byte
        /// </summary>
        /// <returns></returns>
        public static byte RandomByte()
        {
            return RandomByte(0, byte.MaxValue);
        }

        /// <summary>
        ///     随机byte
        /// </summary>
        /// <returns></returns>
        public static byte RandomByte(byte min, byte max)
        {
            return (byte) RandomNumber(min, max);
        }

        /// <summary>
        ///     随机shrot
        /// </summary>
        /// <returns></returns>
        public static short RandomShort()
        {
            return RandomShort(0, short.MaxValue);
        }

        /// <summary>
        ///     随机short
        /// </summary>
        /// <returns></returns>
        public static short RandomShort(short min, short max)
        {
            return (short) RandomNumber(min, max);
        }

        /// <summary>
        ///     随机int
        /// </summary>
        /// <returns></returns>
        public static int RandomNumber()
        {
            return RandomNumber(int.MinValue, int.MaxValue);
        }

        public static double RandomDoubleNumber()
        {
            return _random.NextDouble();
        }

        /// <summary>
        ///     随机int
        /// </summary>
        /// <returns></returns>
        public static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
#endif