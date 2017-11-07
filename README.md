# XPatchLib - .Net 增量内容 序列化/反序列化 工具

#### Patch Serialization Library for the .Net

本项目旨在基于 .Net 创建一套 将指定的两个同一类型的对象实例间增量的内容序列化为指定格式文档，也可以将包含增量内容的文档反序列化并附加至原始的对象实例上 的工具。 

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/XPatchLib.svg)](https://www.nuget.org/packages/XPatchLib/)
[![License-LGPL--3.0](https://img.shields.io/badge/%20License-LGPL--3.0-brightgreen.svg)](https://github.com/GuQiangJS/XPatchLib.Net/blob/master/LICENSE)
[![docs](https://img.shields.io/badge/docs-Overview-brightgreen.svg)](https://guqiangjs.github.io/XPatchLib.Net.Doc/html/9afec0dc-3356-d067-ab5e-a67445fcf631.htm)
[![exts-Json](https://img.shields.io/badge/exts-Json-orange.svg)](https://github.com/GuQiangJS/XPatchLib.Net.Json)


## 使用

使用 `XPatchLib` 最简单的方法是通过 XPatchLib NuGet 软件包。 只需将 [NuGet](https://www.nuget.org/packages/XPatchLib/) 包添加到您的VS项目中即可。

## 支持版本

运行时库被构建为便携式类库，支持：

* .NET Framework 2.0 SP1 及以上版本

* .NET Standard 1.0 及以上版本

可以在 Visual Studio 2008 及后续版本中使用 `XPatchLib`。

## 功能

相对于 [XmlSerializer](https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer(v=vs.110).aspx) 增加如下功能：

* 支持输出 `JSON` 格式。[GitHub](https://github.com/GuQiangJS/XPatchLib.Net.Json)，[NuGet](https://www.nuget.org/packages/XPatchLib.Json/)

* 支持 [DynamicObject](https://msdn.microsoft.com/zh-cn/library/system.dynamic.dynamicobject.aspx) 实例 。

* 支持被不同 [访问修饰符](https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers) 声明的成员参与序列化或反序列化。参见：[ISerializeSetting.Modifier](https://guqiangjs.github.io/XPatchLib.Net.Doc/html/84d9897c-6316-c9d4-90c3-3c80753691a3.htm)。

* 支持选择序列化或反序列化 [属性](https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/properties)
，[字段](https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/fields)。参见：[ISerializeSetting.MemberType](https://guqiangjs.github.io/XPatchLib.Net.Doc/html/d99d22aa-d2ce-68ec-a765-24ccaffd3441.htm)。

* 支持 [Nullable\<T\>](https://msdn.microsoft.com/zh-cn/library/b3h38hb0.aspx) 结构类型属性。

* 支持 [IList\<T\>](https://msdn.microsoft.com/zh-cn/library/5y536ey6(v=vs.110).aspx)，[IEnumerable\<T\>](https://msdn.microsoft.com/zh-cn/library/9eekhta0(v=vs.110).aspx)，[ICollection\<T\>](https://msdn.microsoft.com/zh-cn/library/92t2ye13(v=vs.110).aspx)，[IDictionary\<TKey, TValue\>](https://msdn.microsoft.com/zh-cn/library/s4ys34ea(v=vs.110).aspx) 类型属性。

* 支持属性为接口类型，结构类型，泛型，字典类型。

* 支持 [ISerializable](https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.iserializable.aspx) 实例 。

* 支持在序列化时排除默认值。参见：([ISerializeSetting.SerializeDefalutValue](https://guqiangjs.github.io/XPatchLib.Net.Doc/html/08b1d89e-1f80-5c11-319b-55fbcd78e888.htm))

* 支持将枚举序列化到其文本名称。

* 支持自定义跳过属性序列化特性。参见：([ITextWriter.IgnoreAttributeType](https://guqiangjs.github.io/XPatchLib.Net.Doc/html/3b6ecafa-83a3-3cdd-568d-848b70f3f234.htm))

* 支持 [OnSerializingAttribute](https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.onserializingattribute(v=vs.110).aspx)
，[OnSerializedAttribute](https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.onserializedattribute(v=vs.110).aspx)
，[OnDeserializingAttribute](https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.ondeserializingattribute(v=vs.110).aspx)
，[OnDeserializedAttribute](https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.ondeserializedattribute(v=vs.110).aspx) 特性。
同时支持在序列化或反序列化开始前关闭其中某一项或多项的支持。参见：
[ISerializeSetting.EnableOnSerializingAttribute](https://guqiangjs.github.io/XPatchLib.Net.Doc/html/2415ef52-6307-c846-5843-1aaadea585f0.htm)
，[ISerializeSetting.EnableOnSerializedAttribute](https://guqiangjs.github.io/XPatchLib.Net.Doc/html/18a0a8e8-54a4-0c66-a598-21e4fd9dfa4b.htm)
，[ISerializeSetting.EnableOnDeserializedAttribute](https://guqiangjs.github.io/XPatchLib.Net.Doc/html/63752b2a-22fc-3bef-6ce6-2f7fd414dc6e.htm)
，[ISerializeSetting.EnableOnDeserializingAttribute](https://guqiangjs.github.io/XPatchLib.Net.Doc/html/2645e6e0-406d-4e9e-8190-34f4d2d86d42.htm)
  > 此功能仅在 `.NET Framework 2.0` 及以上版本 或 `.NET Standard 2.0` 及以上版本被支持。

## 性能对比

```ini
BenchmarkDotNet=v0.10.8, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Xeon CPU E3-1231 v3 3.40GHz, ProcessorCount=4
Frequency=10000000 Hz, Resolution=100.0000 ns, Timer=UNKNOWN
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.7.2101.1
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.7.2101.1

```
 |                                        Method |     Mean |    Error |   StdDev |
 |---------------------------------------------- |---------:|---------:|---------:|
 |               SerializeLargeXmlFile_XPatchLib | 185.6 ms | 1.383 ms | 1.226 ms |
 | SerializeLargetXmlFile_DataContractSerializer | 402.9 ms | 7.868 ms | 9.662 ms |
 |           SerializeLargeXmlFile_XmlSerializer | 386.2 ms | 7.593 ms | 9.873 ms |
 |------------------------------------------------ |----------|----------|----------|
 |               DeserializeLargeXmlFile_XPatchLib | 186.12 ms | 0.6233 ms | 0.5830 ms |
 | DeserializeLargetXmlFile_DataContractSerializer |  80.90 ms | 0.2640 ms | 0.2470 ms |
 |           DeserializeLargeXmlFile_XmlSerializer |  84.15 ms | 0.2591 ms | 0.2297 ms |

## 编译

使用 Visual Studio 2017 及以上版本打开 `src/XPatchLib.sln` 。

因为项目文件使用了 Visual Studio 2017 中提供的新的 `csproj` 格式，所以 *开发者* 只能使用 Visual Studio 2017 及以上版本，*使用者* 可以在 Visual Studio 2008 及后续版本中使用 `XPatchLib`。

也可以在安装了 [Visual Studio 2017 生成工具](https://www.visualstudio.com/zh-hans/downloads/) 的前提下，执行 `builder/MasterBuild.bat` 进行编译。

为支持 .NET Core 2.0 编译，请使用 Visual Studio 2017 15.4.0 及以上版本。

## 测试

单元测试使用 

[NUnit3](https://github.com/nunit/nunit) For .NET Framework

[XUnit](https://github.com/xunit/xunit) For .NET Standard

可以通过安装 [NUnit Console](https://github.com/nunit/nunit-console) 后，执行 `builder/unittest.bat` ，自动执行 .Net20,.Net35,.Net40 等多个版本的单元测试。也可以使用 Visual Studio 2017 的 `测试资源管理器`执行。

单元测试项目引用了 [Microsoft.NET.Test.Sdk](https://github.com/microsoft/vstest/) , [NUnit](http://nunit.org/) , [NUnit3TestAdapter](https://github.com/nunit/docs/wiki/Visual-Studio-Test-Adapter), [XUnit](https://github.com/xunit/xunit) , [xunit.runner.visualstudio](https://github.com/xunit/xunit) 。

建议使用 [ReSharper 2017.2.x及以上版本](https://www.jetbrains.com/resharper/eap/)

## **Example**

让我们看一个如何使用XPatchLib在两个类型相同但内容不同的对象间，创建增量内容以及合并增量内容的例子。
首先，我们定义了一个简单的CreditCard类：
```cs
public class CreditCard
{
    public string CardExpiration { get; set; }
    public string CardNumber { get; set; }
    public override bool Equals(object obj)
    {
        CreditCard card = obj as CreditCard;
        if (card == null)
        {
            return false;
        }
        return string.Equals(this.CardNumber, card.CardNumber) 
            && string.Equals(this.CardExpiration, card.CardExpiration);
    }
}
```
同时创建两个类型相同，但内容不同的CreditCard对象。
```cs
CreditCard card1 = new CreditCard()
{
    CardExpiration = "05/12",
    CardNumber = "0123456789"
};
CreditCard card2 = new CreditCard()
{
    CardExpiration = "05/17",
    CardNumber = "9876543210"
};
```

## **产生增量内容**
使用默认提供的 XML格式文档写入器 XmlTextWriter， 调用XPatchLib.Serializer对两个对象的增量内容进行序列化。

```cs
Serializer serializer = new Serializer(typeof(CreditCard));
StringBuilder context = new StringBuilder();
using(StringWriter strWriter = new StringWriter(context))
{
    using (ITextWriter xmlWriter = new XmlTextWriter(strWriter))
    {
        serializer.Divide(xmlWriter, card1, card2);
    }
}
```
经过执行以上代码，context的内容将为：
```xml
<?xml version=""1.0"" encoding=""utf-16""?>
<CreditCard>
  <CardExpiration>05/17</CardExpiration>
  <CardNumber>9876543210</CardNumber>
</CreditCard>
```
通过以上代码，我们实现了两个同类型的对象实例间，增量的序列化。记录了两个对象之间增量的内容。

## **合并增量内容**
下面将介绍如何使用默认提供的 XML格式文档读取器 XmlTextReader， 将已序列化的增量内容附加回原始对象实例，使其与修改后的对象实例形成两个值相同的对象实例。
```cs
CreditCard card3 = null;
Serializer serializer = new Serializer(typeof(CreditCard));
using (var fs = new FileStream(filename, FileMode.Open))
{
    using (var reader = new XmlTextReader(xmlReader))
    {
        card3 = (CreditCard)serializer.Combine(reader, card1);
    }
}
```
经过以上代码，可以使新增的 card3 实例的 CardExpiration 属性的值由card1实例中的 "05/12" 变更为增量内容中记录的 "05/17"，CardNumber的值也由card1实例中的"0123456789"变更为了增量内容中记录的"9876543210"。如果使用值比较的方式比较 card3 和 card2 两个实例，会发现这两个实例完全相同。


## **文档**

* [在线帮助](https://guqiangjs.github.io/XPatchLib.Net.Doc/)

## 相关链接

* [多语言包](https://github.com/GuQiangJS/XPatchLib.Net.Localization)

* [Json格式扩展](https://github.com/GuQiangJS/XPatchLib.Net.Json)

## 后续计划

- [x] 部分对象类型尚未支持。例如：`DateTimeOffset`，`BigInteger` 等。
- [x] 支持自定义跳过属性序列化特性。
- [x] 支持 private 属性。
- [ ] ~~支持[匿名类型](https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/anonymous-types)。~~
  > 匿名类型所有属性均为只读属性，所以无法支持。
- [ ] ~~支持循环引用实例的处理。~~
  > 实例间循环引用会抛出 `InvalidOperationException` 异常。
- [x] 支持 [ISerializable](https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.iserializable.aspx) 实例 。
- [x] 支持 [DynamicObject](https://msdn.microsoft.com/zh-cn/library/system.dynamic.dynamicobject.aspx) 实例 。
- [x] 支持除 `JSON` 格式输出。
- [ ] 支持更多的 .NET [目标框架](https://docs.microsoft.com/zh-cn/dotnet/standard/frameworks)。
