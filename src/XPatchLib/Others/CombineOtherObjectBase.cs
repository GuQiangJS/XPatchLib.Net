using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace XPatchLib.Others
{
    internal abstract class CombineOtherObjectBase:CombineBase
    {
        /// <summary>
        ///     使用指定的类型初始化 <see cref="XPatchLib.CombineBase" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        ///     指定的类型。
        /// </param>
        public CombineOtherObjectBase(TypeExtend pType) : base(pType)
        {
        }

        /// <summary>
        ///     根据增量内容创建基础类型实例。
        /// </summary>
        /// <param name="pReader">XML读取器。</param>
        /// <param name="pOriObject">现有待合并数据的对象。</param>
        /// <param name="pName">当前读取的内容名称。</param>
        /// <returns></returns>
        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            Regex oriValue = pOriObject as Regex;

            Dictionary<string, object> values = GetOriValues(pOriObject, StringComparer.OrdinalIgnoreCase);
            //Dictionary<string, object> revValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;

                //pReader.MoveToElement();

                if (string.Equals(pName, pReader.Name))
                {
                    pReader.Read();
                    continue;
                }

                if (pReader.NodeType == NodeType.Element)
                {
                    string proName = pReader.Name;

                    if (values.ContainsKey(proName))
                        values[proName] = GetMemberValue(proName, pOriObject, pReader);
                    else
                        values.Add(proName, GetMemberValue(proName, pOriObject, pReader));
                }
                pReader.Read();
            }

            return CreateInstance(values);

//#if NET_45_UP || NETSTANDARD
//            return new Regex(pattern, (RegexOptions)options, timeout);
//#else
//            return new Regex(pattern, (RegexOptions)options);
//#endif
        }

        /// <summary>
        /// 获取现有对象需要比较的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected abstract Dictionary<string, object> GetOriValues(object obj, IEqualityComparer<string> comparer);

        protected abstract object CreateInstance(Dictionary<string, object> values);

        protected abstract object GetMemberValue(string proName, object pObj,ITextReader pReader);
    }
}
