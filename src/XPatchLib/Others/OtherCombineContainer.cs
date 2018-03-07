using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace XPatchLib.Others
{
    internal static class OtherCombineContainer
    {
        private static Dictionary<Type, Type> _innerDic = new Dictionary<Type, Type>();
        private static Dictionary<Type, ICombine> _instanceDic = new Dictionary<Type, ICombine>();


        static OtherCombineContainer()
        {
            lock (_innerDic)
            {
                _innerDic.Add(typeof(Regex), typeof(CombineRegex));
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
            lock (_instanceDic)
            {
                _instanceDic.Clear();
            }
        }

        internal static bool TryGet(TypeExtend pType, out ICombine instance)
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
                            instance = TypeHelper.CreateInstance(t, pType) as ICombine;
                            _instanceDic.Add(pType.OriType, instance);
                        }
                    }
                }
            }
            return instance != null;
        }
    }
}
