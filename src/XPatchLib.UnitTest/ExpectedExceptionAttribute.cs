// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace XPatchLib.UnitTest
{
    /// <summary>
    ///     A simple ExpectedExceptionAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ExpectedExceptionAttribute : NUnitAttribute, IWrapTestMethod
    {
        private readonly Type _expectedExceptionType;
        private readonly string _message;

        public ExpectedExceptionAttribute(Type type) : this(type, string.Empty)
        {
        }

        public ExpectedExceptionAttribute(Type type, string message)
        {
            _expectedExceptionType = type;
            _message = message;
        }

        public TestCommand Wrap(TestCommand command)
        {
            return new ExpectedExceptionCommand(command, _expectedExceptionType, _message);
        }

        private class ExpectedExceptionCommand : DelegatingTestCommand
        {
            private readonly Type _expectedType;
            private readonly string _message;

            public ExpectedExceptionCommand(TestCommand innerCommand, Type expectedType,string message)
                : base(innerCommand)
            {
                _expectedType = expectedType;
                _message = message;
            }

            public override TestResult Execute(TestExecutionContext context)
            {
                Type caughtType = null;

                try
                {
                    innerCommand.Execute(context);
                }
                catch (Exception ex)
                {
                    if (ex is NUnitException)
                        ex = ex.InnerException;
                    caughtType = ex.GetType();
                }

                if (caughtType == _expectedType)
                    context.CurrentResult.SetResult(ResultState.Success);
                else if (caughtType != null)
                    context.CurrentResult.SetResult(ResultState.Failure,
                        string.Format("Expected {0} but got {1}", _expectedType.Name, caughtType.Name));
                else
                    context.CurrentResult.SetResult(ResultState.Failure,
                        string.Format("Expected {0} but no exception was thrown", _expectedType.Name));

                return context.CurrentResult;
            }
        }
    }
}