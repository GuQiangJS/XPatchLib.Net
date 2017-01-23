using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XPatchLib
{
    /// <summary>
    /// 复杂类型增量内容合并类。 
    /// </summary>
    internal class CombineObject : CombineBase
    {
        #region Internal Constructors

        /// <summary>
        /// 使用指定的类型初始化 <see cref="XPatchLib.CombineObject" /> 类的新实例。 
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <remarks>
        /// 默认在字符串与 System.DateTime 之间转换时，转换时应保留时区信息。 
        /// </remarks>
        internal CombineObject(TypeExtend pType)
            : base(pType) { }

        /// <summary>
        /// 使用指定的类型和指定的 <see cref="System.Xml.XmlDateTimeSerializationMode" /> 初始化
        /// <see cref="XPatchLib.CombineObject" /> 类的新实例。
        /// </summary>
        /// <param name="pType">
        /// 指定的类型。 
        /// </param>
        /// <param name="pMode">
        /// 指定在字符串与 System.DateTime 之间转换时，如何处理时间值。 
        /// <para> 是用 <see cref="XmlDateTimeSerializationMode.Utc" /> 方式转换时，需要自行进行转换。 </para>
        /// </param>
        /// <exception cref="PrimaryKeyException">
        /// 当 <paramref name="pType" /> 的 <see cref="PrimaryKeyAttribute" /> 定义异常时。 
        /// </exception>
        internal CombineObject(TypeExtend pType, XmlDateTimeSerializationMode pMode)
            : base(pType, pMode) { }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// 将增量内容对象合并至原始对象上。 
        /// </summary>
        /// <param name="pOriObject">
        /// 原始对象集合。 
        /// </param>
        /// <param name="pElement">
        /// 增量节点。 
        /// </param>
        /// <returns>
        /// 返回数据合并后的对象。 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 当 <paramref name="pElement" /> is null 时。 
        /// </exception>
        /// <exception cref="FormatException">
        /// 当增量内容中有需要转换为 <see cref="System.Drawing.Color" /> 类型的数据，而数据无法被转换为
        /// <see cref="System.Drawing.Color" /> 实例时。
        /// </exception>
        internal Object Combine(Object pOriObject, XElement pElement)
        {
            Guard.ArgumentNotNull(pElement, "pElement");

            //当原始对象为null时，先创建一个实例。
            if (pOriObject == null)
            {
                pOriObject = this.Type.CreateInstance();
            }

            Action action;
            if (pElement.TryGetActionAttribute(out action) && action == Action.SetNull)
            {
                //如果在增量数据上找到了Action的Attribute，并且Action是SetNull时，表示增量对象的结果是null。
                return null;
            }
            //当前处理的对象类型上的可以被序列化的属性集合。
            IEnumerable<MemberWrapper> members = this.Type.FieldsToBeSerialized;
            //遍历当前处理的对象类型上的可以被序列化的属性集合。
            foreach (MemberWrapper member in members)
            {
                //在增量数据中按照待处理的属性名称，查找对应的增量数据子节点
                XElement memberElement = pElement.Element(member.Name);
                XAttribute memberAttr = pElement.Attribute(member.Name);

                //当没有在传入的XElement中找到对应属性名称的子属性时，可能当前的动作是Remove。
                //所以增加判断如果当前动作是Remove时，在Attribute中获取属性名称对应的值，创建一个值相同的XElement，以供后面使用。
                if (memberElement == null)
                {
                    if (memberAttr != null)
                    {
                        memberElement = new XElement(member.Name, memberAttr.Value);
                    }
                }

                //如果没有找到则表示此属性无需进行增量内容合并（数据未作修改）
                if (memberElement != null)
                {
                    //处理找到的的增量数据子节点数据，将其值赋给当前正在处理的属性。
                    if (memberElement.TryGetActionAttribute(out action) && action == Action.SetNull)
                    {
                        //如果在增量数据上找到了Action的Attribute，并且Action是SetNull时，表示增量对象的结果是null。
                        //将正在处理的属性设置为null。
                        this.Type.SetMemberValue(pOriObject, member.Name, null);
                        continue;
                    }
                    Type memberType = ReflectionUtils.IsNullable(member.Type) ? ReflectionUtils.GetNullableValueType(member.Type) : member.Type;
                    if (member.IsBasicType)
                    {
                        //当前处理的属性的类型是 基础 类型时
                        if (member.IsColor)
                        {
                            //将增量子节点转换为Color类型后的实例赋值给当前正在处理的属性，替换原有的Color实例。实现数据合并功能。
                            //增量子节点的Value值不能被转换为Color类型实例时，抛出 FormatException 异常。
                            this.Type.SetMemberValue(pOriObject, member.Name, ColorHelper.TransFromString(memberElement.Value));
                        }
                        else if (member.IsEnum)
                        {
                            //单独处理Enum类型
                            this.Type.SetMemberValue(pOriObject, member.Name, new EnumWrapper(memberType).TransFromString(memberElement.Value));
                        }
                        else
                        {
                            //处理其他基础类型，调用CombineBasic类型的Combine方法，创建出基础类型的实例，再将其赋值给当前正在处理的属性，替换原有的数据实例。实现数据合并功能。
                            this.Type.SetMemberValue(pOriObject, member.Name, new CombineBasic(TypeExtendContainer.GetTypeExtend(memberType)).Combine(memberElement));
                        }
                    }
                    else if (member.IsIEnumerable)
                    {
                        //当前处理的属性的类型是 集合 类型时
                        Object memberObj = this.Type.GetMemberValue(pOriObject, member.Name);
                        foreach (XNode n in memberElement.Nodes())
                        {
                            XElement ele = n as XElement;
                            if (ele == null)
                            {
                                continue;
                            }
                            memberObj = new CombineIEnumerable(TypeExtendContainer.GetTypeExtend(memberType)).Combine(memberObj, ele);
                        }
                        this.Type.SetMemberValue(pOriObject, member.Name, memberObj);
                    }
                    else
                    {
                        //当前处理的属性的类型是 复杂 类型时
                        //当前正在处理的属性的现有属性值实例
                        Object memberObj = this.Type.GetMemberValue(pOriObject, member.Name);
                        //如果当前正在处理的属性的现有属性值实例为null时，先创建一个新的实例。
                        if (memberObj == null)
                        {
                            memberObj = TypeExtendContainer.GetTypeExtend(memberType).CreateInstance();
                        }
                        //调用CombineObject类型的Combine方法，对现有属性实例（或新创建的属性实例）进行增量数据合并。
                        new CombineCore(TypeExtendContainer.GetTypeExtend(memberType)).Combine(memberObj, memberElement);
                        //将数据合并后的实例赋值给当前正在处理的属性，替换原有的数据实例。实现数据合并功能。

                        this.Type.SetMemberValue(pOriObject, member.Name, memberObj);
                    }
                }
                else
                {
                    //当复杂类型编辑时，会在增量内容中记录主键内容，用来在合并时确定待合并的数据实例
                    /*
                     * 所以此时还需要根据属性的名称查找增量内容的Attribte内容（主键是以Attribute的方式进行记录的）
                     * <MultilevelClass>
                     *    <Items>
                     *      <FirstLevelClass ID=""2"">
                     *        <Second>
                     *          <SecondID>3-1</SecondID>
                     *        </Second>
                     *      </FirstLevelClass>
                     *    </Items>
                     *  </MultilevelClass>
                     *  上例中的Id="2",记录了FirstLevelClass对象的主键为ID，值为2。
                     *  进行增量合并时需要先遍历MultilevelClass对象的Items集合中的所有FirstLevelClass对象，确定Id=2的实例
                     *  将其Second属性中SecondID值变更为"3-1"
                     */
                    if (memberAttr != null)
                    {
                        this.Type.SetMemberValue(pOriObject, member.Name, memberAttr.Value);
                    }
                }
            }
            return pOriObject;
        }

        #endregion Internal Methods
    }
}