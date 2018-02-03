// Copyright © 2013-2018 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;
using log4net;
#if !NET
using System.IO;
using log4net.Config;
using log4net.Repository;
#endif

namespace XPatchLib.UnitTest
{
    internal class LogHelper
    {
        /// <summary>
        /// 日志实例
        /// </summary>
        private static readonly ILog LogInstance;

        static LogHelper()
        {
#if NET 
            LogInstance = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else 
            Console.SetOut(new StringWriter());
            //ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            //FileInfo fi = new FileInfo("log4net.config");
            //XmlConfigurator.Configure(repository, fi);
            //LogInstance = LogManager.GetLogger(repository.Name, typeof(LogHelper));
#endif
        }

        internal static void Debug(object message)
        {
#if NET
            LogInstance.Debug(message);
#else 
            Console.WriteLine(message);
#endif
        }

        internal static void Debug(string format, params object[] args)
        {
#if NET
            LogInstance.DebugFormat(format, args);
#else 
            Console.WriteLine(format, args);
#endif
        }
    }
}