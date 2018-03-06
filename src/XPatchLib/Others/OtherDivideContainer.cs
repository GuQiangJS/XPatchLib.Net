using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace XPatchLib.Others
{
    internal static class OtherDivideContainer
    {
        private static Dictionary<Type, Type> _innerDic = new Dictionary<Type, Type>();
        private static Dictionary<Type, IDivide> _instanceDic = new Dictionary<Type, IDivide>();


        static OtherDivideContainer()
        {
            lock (_innerDic)
            {
                _innerDic.Add(typeof(Regex), typeof(DivideRegex));
                _innerDic.Add(typeof(TimeSpan), typeof(DivideBasic));
            }
        }

        internal static void ClearAll()
        {
            lock (_innerDic)
            {
                _innerDic.Clear();
            }
            ClearInstances();
        }

        internal static void ClearInstances()
        {
            lock (_instanceDic)
            {
                _instanceDic.Clear();
            }
        }

        internal static bool TryGet(ITextWriter pWriter, TypeExtend pType, out IDivide instance)
        {
            instance = null;
            lock (_instanceDic)
            {
                if (!_instanceDic.TryGetValue(pType.OriType, out instance))
                {
                    Type t;
                    lock (_instanceDic)
                    {
                        if (_innerDic.TryGetValue(pType.OriType, out t))
                        {
                            instance = TypeHelper.CreateInstance(t, pWriter, pType) as IDivide;
                            _instanceDic.Add(pType.OriType, instance);
                        }
                    }
                }
            }
            return instance != null;
        }
    }
}
