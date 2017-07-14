# XPatchLib - .Net 增量内容 序列化/反序列化 工具

#### Patch Serialization Library for the .Net

本项目旨在基于 .Net 创建一套 将指定的两个同一类型的对象实例间增量的内容序列化为指定格式文档，也可以将包含增量内容的文档反序列化并附加至原始的对象实例上 的工具。 

## 使用

使用 `XPatchLib` 最简单的方法是通过 XPatchLib NuGet 软件包。 只需将 [NuGet](https://www.nuget.org/packages/XPatchLib/) 包添加到您的VS项目中即可。

## 支持版本

运行时库被构建为便携式类库，支持：

* .NET Framework 2.0 SP1 及以上版本

* .NET Standard 1.3 及以上版本

可以在 Visual Studio 2008 及后续版本中使用 `XPatchLib`。

## 功能

相对于 [XmlSerializer](https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer(v=vs.110).aspx) 增加如下功能：

* 增加支持以下类型属性序列化 
    - [IList](https://msdn.microsoft.com/zh-cn/library/system.collections.ilist(v=vs.110).aspx) 
    - [IEnumerable](https://msdn.microsoft.com/zh-cn/library/system.collections.ienumerable(v=vs.110).aspx) 
    - [ICollection](https://msdn.microsoft.com/zh-cn/library/system.collections.icollection(v=vs.110).aspx) 
    - [IDictionary](https://msdn.microsoft.com/zh-cn/library/system.collections.idictionary(v=vs.110).aspx) 
    - [Nullable](https://msdn.microsoft.com/zh-cn/library/system.nullable.aspx) 
    - [Uri](https://msdn.microsoft.com/zh-cn/library/system.uri(v=vs.110).aspx) 
    - [BigInteger](https://msdn.microsoft.com/zh-cn/library/system.numerics.biginteger(v=vs.110).aspx) 
    - [DateTimeOffset](https://msdn.microsoft.com/library/system.datetimeoffset.aspx) 
    - [TimeSpan](https://msdn.microsoft.com/zh-cn/library/system.timespan.aspx)

* 支持在序列化时排除默认值。

## 性能对比

## 编译

使用 Visual Studio 2017 及以上版本打开 `src/XPatchLib.sln` 。

因为项目文件使用了 Visual Studio 2017 中提供的新的 `csproj` 格式，所以 *开发者* 只能使用 Visual Studio 2017 及以上版本，*使用者* 可以在 Visual Studio 2008 及后续版本中使用 `XPatchLib`。

也可以在安装了 [Visual Studio 2017 生成工具](https://www.visualstudio.com/zh-hans/downloads/) 的前提下，执行 `builder/MasterBuild.bat` 进行编译。

## 测试

单元测试使用 [NUnit3](https://github.com/nunit/nunit)。可以通过安装 [NUnit Console](https://github.com/nunit/nunit-console) 后，执行 `builder/unittest.bat` ，自动执行 .Net20,.Net35,.Net40 等多个版本的单元测试。也可以使用 Visual Studio 2017 的 `测试资源管理器`执行。

单元测试项目引用了 [Microsoft.NET.Test.Sdk](https://github.com/microsoft/vstest/) , [NUnit](http://nunit.org/) , [NUnit3TestAdapter](https://github.com/nunit/docs/wiki/Visual-Studio-Test-Adapter) 。

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
string context = string.Empty;
using (MemoryStream stream = new MemoryStream())
{
    var settings = new XmlWriterSettings();
    settings.Encoding = Encoding.UTF8;
    settings.Indent = true;
    using (var xmlWriter = XmlWriter.Create(fs, settings))
    {
         using (var writer = new XmlTextWriter(xmlWriter))
         {
              serializer.Divide(writer, card1, card2);
         }
    }
    using (var stremReader = new StreamReader(stream, settings.Encoding))
    {
        context = stremReader.ReadToEnd();
    }
}
```
经过执行以上代码，context的内容将为：
```xml
<?xml version=""1.0"" encoding=""utf-8""?>
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
     using (var xmlReader = XmlReader.Create(fs))
     {
           using (var reader = new XmlTextReader(xmlReader))
           {
                card3 = (CreditCard)serializer.Combine(reader, card1);
           }
     }
}
```
经过以上代码，可以使新增的 card3 实例的 CardExpiration 属性的值由card1实例中的 "05/12" 变更为增量内容中记录的 "05/17"，CardNumber的值也由card1实例中的"0123456789"变更为了增量内容中记录的"9876543210"。如果使用值比较的方式比较 card3 和 card2 两个实例，会发现这两个实例完全相同。


## **文档**

[在线帮助](https://guqiangjs.github.io/XPatchLib.Net.Doc/)

## 相关链接

* [XPatchLib.Net.Localization](https://github.com/GuQiangJS/XPatchLib.Net.Localization)

## 后续计划

* ~~部分对象类型尚未支持。例如：`DateTimeOffset` ， `BigInteger` 等。(已支持)~~

* 支持除 `XML` 外的其他格式输出。

* 支持更多的 .NET [目标框架](https://docs.microsoft.com/zh-cn/dotnet/standard/frameworks)。
