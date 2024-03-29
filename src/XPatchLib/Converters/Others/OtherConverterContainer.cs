﻿// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace XPatchLib
{
    internal static class OtherConverterContainer
    {
        private static readonly Dictionary<Type, Type> _innerDic = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, ICombine> _instance_Combine_Dic = new Dictionary<Type, ICombine>();
        private static readonly Dictionary<Type, IDivide> _instance_Divide_Dic = new Dictionary<Type, IDivide>();
        private static readonly Dictionary<Type, IConverter> _instance_Converter_Dic = new Dictionary<Type, IConverter>();

        internal static bool TryGetConverter(TypeExtend pType, out IConverter instance)
        {
            instance = null;
            lock (_instance_Converter_Dic)
            {
                if (!_instance_Converter_Dic.TryGetValue(pType.OriType, out instance))
                {
                    Type t;
                    lock (_instance_Converter_Dic)
                    {
                        if (_innerDic.TryGetValue(pType.OriType, out t))
                        {
                            instance = TypeHelper.CreateInstance(t, pType) as IConverter;
                            _instance_Converter_Dic.Add(pType.OriType, instance);
                        }
                    }
                }
            }

            return instance != null;
        }

        static OtherConverterContainer()
        {
            lock (_innerDic)
            {
                _innerDic.Add(typeof(Regex), typeof(ConvertRegex));
                _innerDic.Add(typeof(CultureInfo), typeof(ConverterCultureInfo));
#if NET ||NETSTANDARD_2_0_UP
                _innerDic.Add(typeof(DriveInfo), typeof(ConverterDriveInfo));
#endif
#if NET || NETSTANDARD_1_3_UP
                _innerDic.Add(typeof(FileInfo),typeof(ConverterFileInfo));
                _innerDic.Add(typeof(DirectoryInfo), typeof(ConverterDirectoryInfo));
#endif
            }
        }

        internal static void Clear()
        {
            lock (_innerDic)
            {
                _innerDic.Clear();
            }

            ClearInstances();
        }

        internal static void ClearInstances()
        {
            ClearCombineInstances();
            ClearDivideInstances();
        }

        internal static void ClearCombineInstances()
        {
            lock (_instance_Combine_Dic)
            {
                _instance_Combine_Dic.Clear();
            }
        }

        internal static void ClearDivideInstances()
        {
            lock (_instance_Divide_Dic)
            {
                _instance_Divide_Dic.Clear();
            }
        }

        internal static bool TryGetCombine(TypeExtend pType, out ICombine instance)
        {
            instance = null;
            lock (_instance_Combine_Dic)
            {
                if (!_instance_Combine_Dic.TryGetValue(pType.OriType, out instance))
                {
                    Type t;
                    lock (_instance_Combine_Dic)
                    {
                        if (_innerDic.TryGetValue(pType.OriType, out t))
                        {
                            instance = TypeHelper.CreateInstance(t, pType) as ICombine;
                            _instance_Combine_Dic.Add(pType.OriType, instance);
                        }
                    }
                }
            }

            return instance != null;
        }

        internal static bool TryGetDivide(TypeExtend pType, ITextWriter pWriter, out IDivide instance)
        {
            instance = null;
            lock (_instance_Divide_Dic)
            {
                if (!_instance_Divide_Dic.TryGetValue(pType.OriType, out instance))
                {
                    Type t;
                    lock (_instance_Divide_Dic)
                    {
                        if (_innerDic.TryGetValue(pType.OriType, out t))
                        {
                            instance = TypeHelper.CreateInstance(t, pWriter, pType) as IDivide;
                            _instance_Divide_Dic.Add(pType.OriType, instance);
                        }
                    }
                }
            }

            return instance != null;
        }
    }
}