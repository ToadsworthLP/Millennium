using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Burst.Compiler.IL.Tests.Helpers;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using Unity.Burst;
using Unity.Mathematics;
using UnityBenchShared;
using UnityEngine;

namespace Burst.Compiler.IL.Tests
{

    internal class TestCompilerAttribute : TestCompilerAttributeBase
    {
#pragma warning disable 0414
        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();
#pragma warning restore 0414
        public TestCompilerAttribute(params object[] arguments) : base(arguments)
        {
        }

        protected override Boolean IsCommandLine()
        {
            return false;
        }

        protected override Boolean IsMono()
        {
            return true;
        }

        protected override bool SupportException {
            get {
                // We don't support exception in Unity editor
                return false;
            }
        }

        protected override TestCompilerCommandBase GetTestCommand(Test test, TestMethod originalMethod,
            Type expectedException,
            Boolean ExpectCompilerException)
        {
            return new TestCompilerCommand(test, originalMethod, expectedException, ExpectCompilerException);
        }


        public class TestCompilerCommand : TestCompilerCommandBase
        {
            public TestCompilerCommand(Test test, TestMethod originalMethod, Type expectedException,
                bool expectCompilerException) : base(test, originalMethod, expectedException, expectCompilerException)
            {
            }

            protected override int GetRunCount()
            {
                return 1;
            }

            protected override void CompleteTest(ITestExecutionContext context)
            {
                context.CurrentResult.SetResult(ResultState.Success);
            }

            protected override int GetULP()
            {
                return 512;
            }

            protected override object[] GetArgumentsArray(TestMethod method)
            {
                return method.parms.Arguments.ToArray();
            }

            protected override Delegate CompileDelegate(ITestExecutionContext context, MethodInfo methodInfo,
                Type delegateType)
            {
                var functionDelegate = Delegate.CreateDelegate(delegateType, methodInfo);
                var compiledFunction = BurstCompiler.CompileDelegate(functionDelegate);

                return compiledFunction;
            }

#if UNITY_BURST_FEATURE_FUNCPTR
            protected override IFunctionPointer CompileFunctionPointer(MethodInfo methodInfo, Type functionType)
            {
                throw new NotImplementedException();
            }
#endif
            protected override void Setup()
            {
            }

            protected override TestResult HandleCompilerException(ITestExecutionContext context, MethodInfo methodInfo)
            {

                var arguments = GetArgumentsArray(_originalMethod);
                Type[] nativeArgTypes = new Type[arguments.Length];

                for (var i = 0; i < arguments.Length; ++i)
                {
                    nativeArgTypes[i] = arguments[i].GetType();
                }

                var delegateType = DelegateHelper.NewDelegateType(methodInfo.ReturnType, nativeArgTypes);

                var functionDelegate = Delegate.CreateDelegate(delegateType, methodInfo);
                Delegate compiledFunction = BurstCompiler.CompileDelegate(functionDelegate);

                if (functionDelegate == compiledFunction)
                    context.CurrentResult.SetResult(ResultState.Success);
                else
                    context.CurrentResult.SetResult(ResultState.Failure, $"The function have been compiled successfully, but an error was expected.");

                return context.CurrentResult;
            }
        }
    }
}
