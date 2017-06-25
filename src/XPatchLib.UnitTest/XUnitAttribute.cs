// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.


#if XUNIT

// https://xunit.github.io/docs/comparisons.html
// .net Core 项目使用 Xunit 进行测试
// 此处增加的特性标记均为 Xunit 没有的特性标记。
// 这里增加这个标记是为了项目在 .net core 版本下能够编译通过

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPatchLib.UnitTest
{
    public class TestFixtureAttribute : Attribute
    {
    }

    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string desc)
        {
        }
    }
}
#endif