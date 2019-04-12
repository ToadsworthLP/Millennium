using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Burst.Compiler.IL.Tests.Helpers;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Commands;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityBenchShared;

#if UNITY_EDITOR
using ExecutionContext = NUnit.Framework.Internal.ITestExecutionContext;
#else
using ExecutionContext = NUnit.Framework.Internal.TestExecutionContext;
#endif

namespace Burst.Compiler.IL.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    internal abstract class TestCompilerAttributeBase : TestCaseAttribute, ITestBuilder, IWrapTestMethod
    {
        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        public const string GoldFolder = "gold";
        public const string GeneratedFolder = "generated";

        public TestCompilerAttributeBase(params object[] arguments) : base(arguments)
        {
        }

        public Type ExpectedException { get; set; }

        // TODO: Add support for matching the content of an exception message
        public bool ExpectCompilerException { get; set; }

        public bool FastMath { get; set; }

#if UNITY_BURST_FEATURE_FUNCPTR
        public string[] ProvidedFunctionPointers { get; set; }

        public string[] FunctionPointers { get; set; }
#endif

        /// <summary>
        /// Use this property when the JIT calculation is wrong (e.g when using float)
        /// </summary>
        public object OverrideResultOnMono { get; set; }

        public bool? IsDeterministic { get; set; }

        protected virtual bool SupportException => true;

        IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test suite)
        {
            // If the system doesn't support exceptions (Unity editor for delegates) we should not test with exceptions
            bool skipTest = (ExpectCompilerException || ExpectedException != null) && !SupportException;

            var expectResult = !method.ReturnType.IsType(typeof(void));
            var name = method.Name;
            var arguments = this.Arguments;
            var permutations = CreatePermutation(0, arguments, method.GetParameters());

            // TODO: Workaround for a scalability bug with R# or Rider
            // Run only one testcase if not running from the commandline
            if (!IsCommandLine())
            {
                permutations = permutations.Take(1);
            }

            foreach (var newArguments in permutations)
            {
                var caseParameters = new TestCaseParameters(newArguments);
                if (expectResult)
                {
                    caseParameters.ExpectedResult = true;
                    if (OverrideResultOnMono != null && IsMono())
                    {
                        caseParameters.Properties.Set(nameof(OverrideResultOnMono), OverrideResultOnMono);
                    }
                }

                // Transfer FastMath parameter to the compiler
                caseParameters.Properties.Set(nameof(FastMath), FastMath);

                var test = _builder.BuildTestMethod(method, suite, caseParameters);
                if (skipTest)
                {
                    test.RunState = RunState.Skipped;
                    test.Properties.Add(PropertyNames.SkipReason, "Exceptions are not supported");
                }

                yield return test;
            }
        }

        private static IEnumerable<object[]> CreatePermutation(int index, object[] args, IParameterInfo[] parameters)
        {
            if (index >= args.Length)
            {
                yield return args;
                yield break;
            }
            var copyArgs = (object[])args.Clone();
            bool hasRange = false;
            for (; index < args.Length; index++)
            {
                var arg = copyArgs[index];
                if (arg is DataRange)
                {
                    var range = (DataRange)arg;
                    // TEMP: Disable NaN test for now
                    //range = range & ~(DataRange.NaN);
                    foreach (var value in range.ExpandRange(parameters[index].ParameterType, index))
                    {
                        copyArgs[index] = value;
                        foreach (var subPermutation in CreatePermutation(index + 1, copyArgs, parameters))
                        {
                            hasRange = true;
                            yield return subPermutation;
                        }
                    }
                }
            }
            if (!hasRange)
            {
                yield return copyArgs;
            }
        }

        TestCommand ICommandWrapper.Wrap(TestCommand command)
        {
            if (command is TestMethodCommand)
            {
                var testMethodCommand = (TestMethodCommand)command;
                var testMethod = (TestMethod)testMethodCommand.Test;
                return GetTestCommand(testMethod, testMethod, ExpectedException, ExpectCompilerException);
            }
            else
            {
                return command;
            }
        }

        protected abstract bool IsCommandLine();

        protected abstract bool IsMono();

        protected abstract TestCompilerCommandBase GetTestCommand(Test test, TestMethod originalMethod, Type expectedException, bool ExpectCompilerException);
    }

    internal abstract class TestCompilerCommandBase : TestCommand
    {
        protected readonly TestMethod _originalMethod;
        readonly Type _expectedException;
        protected readonly bool _expectCompilerException;

        public TestCompilerCommandBase(Test test, TestMethod originalMethod, Type expectedException, bool expectCompilerException) : base(test)
        {
            _originalMethod = originalMethod;
            _expectedException = expectedException;
            _expectCompilerException = expectCompilerException;
        }

        public override TestResult Execute(ExecutionContext context)
        {
            // Make sure that we are not running in // the tests
            lock (typeof(TestCompilerCommandBase))
            {
                TestResult lastResult = null;
                for (int i = 0; i < GetRunCount(); i++)
                {
                    lastResult = ExecuteMethod(context);
                }

                return lastResult;
            }
        }

        protected virtual Type CreateNativeDelegateType(Type returnType, Type[] arguments, out bool isInRegistry, out Func<object, object[], object> caller)
        {
            isInRegistry = false;
            StaticDelegateCallback staticDelegate;
            if (StaticDelegateRegistry.TryFind(returnType, arguments, out staticDelegate))
            {
                isInRegistry = true;
                caller = staticDelegate.Caller;
                return staticDelegate.DelegateType;
            }
            else
            {
                // Else we try to do it with a dynamic call
                var type = DelegateHelper.NewDelegateType(returnType, arguments);
                caller = StaticDynamicDelegateCaller;
                return type;
            }
        }

        private static Func<object, object[], object> StaticDynamicDelegateCaller = new Func<object, object[], object>((del, arguments) => ((Delegate)del).DynamicInvoke(arguments));

        private TestResult ExecuteMethod(ExecutionContext context)
        {
            Setup();
            var methodInfo = _originalMethod.Method.MethodInfo;

            // Special case for Compiler Expceptions when IL can't be translated
            if (_expectCompilerException)
            {
                return HandleCompilerException(context, methodInfo);
            }

            var arguments = GetArgumentsArray(_originalMethod);
            object[] nativeArgs;
            Type[] nativeArgTypes;
            TransformArguments(_originalMethod.Method.MethodInfo, arguments, out nativeArgs, out nativeArgTypes);

            bool isInRegistry = false;
            Func<object, object[], object> nativeDelegateCaller;
            var delegateType = CreateNativeDelegateType(_originalMethod.Method.MethodInfo.ReturnType, nativeArgTypes, out isInRegistry, out nativeDelegateCaller);
            if (!isInRegistry)
            {
                Console.WriteLine($"Warning, the delegate for the method `{_originalMethod.Method}` has not been generated");
            }

            var compiledFunction = CompileDelegate(context, methodInfo, delegateType);

            if (compiledFunction == null)
                return context.CurrentResult;

            // Special case if we have an expected exception
            if (_expectedException != null)
            {
                TryExpectedException(context, () => _originalMethod.Method.Invoke(context.TestObject, arguments), ".NET", type => type == _expectedException, _expectedException.FullName, false);
                if (context.CurrentResult.ResultState.Equals(ResultState.Success))
                {
                    TryExpectedException(context, () => nativeDelegateCaller(compiledFunction, nativeArgs), "Native", type => type == _expectedException, _expectedException.FullName, true);
                }
            }
            else
            {
                var resultNative = nativeDelegateCaller(compiledFunction, nativeArgs);
                var resultClr = _originalMethod.Method.Invoke(context.TestObject, arguments);
                
                var overrideResultOnMono = _originalMethod.Properties.Get("OverrideResultOnMono");
                if (overrideResultOnMono != null)
                {
                    Console.WriteLine($"Using OverrideResultOnMono: `{overrideResultOnMono}` instead of `{resultClr}` compare to burst `{resultNative}`");
                    resultClr = overrideResultOnMono;
                }

                // Use our own version (for handling correctly float precision)
                AssertHelper.AreEqual(resultClr, resultNative, GetULP());

                // Allow to process native result
                ProcessNativeResult(_originalMethod, resultNative);

                context.CurrentResult.SetResult(ResultState.Success);
            }

            // Check that the method is actually in the registry
            Assert.True(isInRegistry, "The test method is not in the registry, recompile the project with the updated StaticDelegateRegistry.generated.cs");

            // Make an attempt to clean up arguments (to reduce wasted native heap memory)
            DisposeObjects(arguments);
            DisposeObjects(nativeArgs);


            CompleteTest(context);

            return context.CurrentResult;
        }

        protected virtual void ProcessNativeResult(TestMethod method, object result)
        {
        }

        private static void DisposeObjects(object[] arguments)
        {
            foreach (object o in arguments)
            {
                IDisposable disp = o as IDisposable;
                disp?.Dispose();
            }
        }

        private object[] CloneArguments(object[] arguments)
        {
            var newArguments = new object[arguments.Length];
            for (int i = 0; i < arguments.Length; i++)
            {
                newArguments[i] = arguments[i];
            }
            return newArguments;
        }

        protected void TransformArguments(MethodInfo method, object[] args, out object[] nativeArgs, out Type[] nativeArgTypes)
        {
            // Transform Arguments if necessary
            nativeArgs = (object[])args.Clone();

            for (var i = 0; i < nativeArgs.Length; i++)
            {
                var arg = args[i];
                if (arg == null)
                {
                    throw new AssertionException($"Argument number `{i}` for method `{method}` cannot be null");
                }
                if (arg.GetType() == typeof(float[]))
                {
                    args[i] = ConvertToNativeArray((float[])arg);
                }
                else if (arg.GetType() == typeof(int[]))
                {
                    args[i] = ConvertToNativeArray((int[])arg);
                }
                else if (arg.GetType() == typeof(float3[]))
                {
                    args[i] = ConvertToNativeArray((float3[])arg);
                }
                else if (arg is Type)
                {
                    var attrType = (Type)arg;
                    if (typeof(IArgumentProvider).IsAssignableFrom(attrType))
                    {
                        var argumentProvider = (IArgumentProvider)Activator.CreateInstance(attrType);
                        // Duplicate the input for C#/Burst in case the code is modifying the data
                        args[i] = argumentProvider.Value;
                        nativeArgs[i] = argumentProvider.Value;
                    }
                }
            }

            var parameters = method.GetParameters();
            nativeArgTypes = new Type[nativeArgs.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var expectedArgType = parameters[i].ParameterType;
                var actualArgType = args[i].GetType();
                var actualNativeArgType = nativeArgs[i].GetType();

                // If the expected parameter for the native is a reference, we need to specify it here
                if (expectedArgType.IsByRef)
                {
                    actualArgType = actualArgType.MakeByRefType();
                    actualNativeArgType = actualNativeArgType.MakeByRefType();
                }
                if (expectedArgType == typeof(IntPtr) && actualNativeArgType == typeof(int))
                {
                    nativeArgs[i] = new IntPtr((int)args[i]);
                    args[i] = new IntPtr((int)args[i]);
                    actualNativeArgType = typeof(IntPtr);
                    actualArgType = typeof(IntPtr);
                }
                if (expectedArgType == typeof(UIntPtr) && actualNativeArgType == typeof(uint))
                {
                    nativeArgs[i] = new UIntPtr((uint)args[i]);
                    args[i] = new UIntPtr((uint)args[i]);
                    actualNativeArgType = typeof(UIntPtr);
                    actualArgType = typeof(UIntPtr);
                }
                if (expectedArgType.IsPointer && actualNativeArgType == typeof(IntPtr))
                {
                    nativeArgs[i] = args[i];
                    args[i] = args[i];
                    actualNativeArgType = expectedArgType;
                    actualArgType = expectedArgType;
                }

#if UNITY_BURST_FEATURE_FUNCPTR
                if (typeof(IFunctionPointer).IsAssignableFrom(expectedArgType) && actualNativeArgType == typeof(string))
                {
                    var methodName = (string)args[i];
                    var candidates =
                        _originalMethod.Method.MethodInfo.DeclaringType?
                            .GetMethods()
                            .Where(x => x.IsStatic && x.Name.Equals(methodName))
                            .ToArray();
                    
                    if (candidates == null || candidates.Length != 1)
                    {
                        throw new ArgumentException($"Could not resolve an unambigoues static method from name {methodName}.");
                    }

                    var functionPointer = CompileFunctionPointer(candidates[0], expectedArgType);
                    nativeArgs[i] = functionPointer;
                    args[i] = functionPointer;
                    actualNativeArgType = expectedArgType;
                    actualArgType = expectedArgType;
                }
#endif

                nativeArgTypes[i] = actualNativeArgType;

                if (expectedArgType != actualArgType)
                {
                    throw new ArgumentException($"Type mismatch in parameter {i} passed to {method.Name}: expected {expectedArgType}, got {actualArgType}.");
                }
            }
        }

        private static NativeArray<T> ConvertToNativeArray<T>(T[] array) where T : struct
        {
            var nativeArray = new NativeArray<T>(array.Length, Allocator.Persistent);
            for (var j = 0; j < array.Length; j++)
                nativeArray[j] = array[j];
            return nativeArray;
        }

        protected void TryExpectedException(ExecutionContext context, Action action, string contextName, Func<Type, bool>  expectedException, string expectedExceptionName, bool isTargetException)
        {
            Type caughtType = null;

            Exception caughtException = null;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (isTargetException && ex is TargetInvocationException)
                {
                    ex = ((TargetInvocationException)ex).InnerException;
                }
                if (ex is NUnitException)
                    ex = ex.InnerException;
                caughtException = ex;
                if (caughtException != null)
                {
                    caughtType = caughtException.GetType();
                }
            }

            if (caughtType != null && expectedException(caughtType))
            {
                context.CurrentResult.SetResult(ResultState.Success);
            }
            else if (caughtType != null)
            {
                context.CurrentResult.SetResult(ResultState.Failure, $"In {contextName} code, expected {expectedExceptionName} but got {caughtType.Name}. Exception: {caughtException}");
            }
            else
                context.CurrentResult.SetResult(ResultState.Failure, $"In {contextName} code, expected {expectedExceptionName} but no exception was thrown");
        }

        protected abstract int GetRunCount();

        protected abstract void CompleteTest(ExecutionContext context);

        protected abstract int GetULP();

        protected abstract object[] GetArgumentsArray(TestMethod method);

        protected abstract Delegate CompileDelegate(ExecutionContext context, MethodInfo methodInfo, Type delegateType);

#if UNITY_BURST_FEATURE_FUNCPTR
        protected abstract IFunctionPointer CompileFunctionPointer(MethodInfo methodInfo, Type functionType);
#endif
        protected abstract void Setup();

        protected abstract TestResult HandleCompilerException(ExecutionContext context, MethodInfo methodInfo);
    }
}