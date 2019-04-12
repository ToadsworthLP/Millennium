// As BurstCompiler.Compile is not supported on Tiny/ZeroPlayer, we can ifdef the entire file
#if !UNITY_ZEROPLAYER && !UNITY_CSHARP_TINY
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
#if !BURST_INTERNAL
using Unity.Jobs.LowLevel.Unsafe;
#endif

#if BURST_INTERNAL
namespace Burst.Compiler.IL
#else
namespace Unity.Burst
#endif
{
    /// <summary>
    /// Options shared between the package and the burst compiler.
    /// NOTE: This file is shared via a csproj cs link in Burst.Compiler.IL
    /// </summary>
    internal sealed partial class BurstCompilerOptions
    {
        private const string DisableCompilationArg = "--burst-disable-compilation";

        private const string ForceSynchronousCompilationArg = "--burst-force-sync-compilation";

        private static bool _enableBurstCompilation;
        private static bool _forceDisableBurstCompilation;
        private static bool _enableBurstCompileSynchronously;
        private static bool _forceBurstCompilationSynchronously;
        private static bool _enableBurstSafetyChecks;
        private static bool _enableBurstTimings;

        public const string DefaultLibraryName = "lib_burst_generated";

        // -------------------------------------------------------
        // Common options used by the compiler
        // -------------------------------------------------------
        public const string OptionGroup = "group";
        public const string OptionPlatform = "platform=";
        public const string OptionBackend = "backend=";
        public const string OptionSafetyChecks = "safety-checks";
        public const string OptionDisableSafetyChecks = "disable-safety-checks";
        public const string OptionNoAlias = "noalias";
        public const string OptionDisableNoAlias = "disable-noalias";
        public const string OptionDisableOpt = "disable-opt";
        public const string OptionFastMath = "fastmath";
        public const string OptionTarget = "target=";
        public const string OptionIROpt = "ir-opt";
        public const string OptionCpuOpt = "cpu-opt=";
        public const string OptionFloatPrecision = "float-precision=";
        public const string OptionFloatMode = "float-mode=";
        public const string OptionDump = "dump=";
        public const string OptionFormat = "format=";
        public const string OptionDebugTrap = "debugtrap";
        public const string OptionDisableVectors = "disable-vectors";
        public const string OptionDebug = "debug";
        public const string OptionDisableDebugSymbols = "disable-load-debug-symbols";
        public const string OptionStaticLinkage = "generate-static-linkage-methods";

        // -------------------------------------------------------
        // Options used by the Jit compiler
        // -------------------------------------------------------

        public const string OptionJitDisableFunctionCaching = "disable-function-caching";
        public const string OptionJitEnableModuleCaching = "enable-module-caching";
        public const string OptionJitEnableModuleCachingDebugger = "enable-module-caching-debugger";
        public const string OptionJitEnableSynchronousCompilation = "enable-synchronous-compilation";

        // TODO: Remove this option and use proper dump flags or revisit how we log timings
        public const string OptionJitLogTimings = "log-timings";
        public const string OptionJitCacheDirectory = "cache-directory";

        // -------------------------------------------------------
        // Options used by the Aot compiler
        // -------------------------------------------------------
        public const string OptionAotAssemblyFolder = "assembly-folder=";
        public const string OptionAotMethod = "method=";
        public const string OptionAotType = "type=";
        public const string OptionAotAssembly = "assembly=";
        public const string OptionAotOutputPath = "output=";
        public const string OptionAotKeepIntermediateFiles = "keep-intermediate-files";
        public const string OptionAotNoLink = "nolink";
        public const string OptionVerbose = "verbose";

#if !BURST_INTERNAL

        private const string GlobalSettingsName = "global";
        private static BurstCompilerOptions _global = null;

        public BurstCompilerOptions() : this(null)
        {
        }

        public BurstCompilerOptions(string name)
        {
            Name = name;
            // By default, burst is enabled as well as safety checks
            EnableBurstCompilation = true;
            EnableBurstSafetyChecks = true;
        }

        public static BurstCompilerOptions Global
        {
            get
            {
                // Make sure to late initialize the settings
                return _global ?? (_global = new BurstCompilerOptions(GlobalSettingsName)); // naming used only for debugging
            }
        }

        private bool _enableEnhancedAssembly;
        private bool _disableOptimizations;
        private bool _enableFastMath;

        public string Name { get; set; }

        public bool IsEnabled
        {
            get => EnableBurstCompilation && !_forceDisableBurstCompilation;
        }
        
        public bool EnableBurstCompilation
        {
            get => _enableBurstCompilation;
            set
            {
                bool changed = _enableBurstCompilation != value;
               _enableBurstCompilation = value;

                // Modify only JobsUtility.JobCompilerEnabled when modifying global settings
                if (Name == GlobalSettingsName)
               {
                   // We need also to disable jobs as functions are being cached by the job system
                   // and when we ask for disabling burst, we are also asking the job system
                   // to no longer use the cached functions
                   JobsUtility.JobCompilerEnabled = value;
               }
                if (changed) OnOptionsChanged();
            }
        }

        public bool EnableBurstCompileSynchronously
        {
            get => _enableBurstCompileSynchronously;
            set
            {
                bool changed = _enableBurstCompileSynchronously != value;
                _enableBurstCompileSynchronously = value;
                if (changed) OnOptionsChanged();
            }
        }

        public bool EnableBurstSafetyChecks
        {
            get => _enableBurstSafetyChecks;
            set
            {
                bool changed = _enableBurstSafetyChecks != value;
                _enableBurstSafetyChecks = value;
                if (changed) OnOptionsChanged();
            }
        }

        public bool EnableEnhancedAssembly
        {
            get => _enableEnhancedAssembly;
            set
            {
                bool changed = _enableEnhancedAssembly != value;
                _enableEnhancedAssembly = value;
                if (changed) OnOptionsChanged();
            }
        }

        public bool DisableOptimizations
        {
            get => _disableOptimizations;
            set
            {
                bool changed = _disableOptimizations != value;
                _disableOptimizations = value;
                if (changed) OnOptionsChanged();
            }
        }

        public bool EnableFastMath
        {
            get => _enableFastMath;
            set
            {
                bool changed = _enableFastMath != value;
                _enableFastMath = value;
                if (changed) OnOptionsChanged();
            }
        }

        public bool EnableBurstTimings
        {
            get => _enableBurstTimings;
            set
            {
                bool changed = _enableBurstTimings != value;
                _enableBurstTimings = value;
                if (changed) OnOptionsChanged();
            }
        }

        public Action OptionsChanged { get; set; }

        public BurstCompilerOptions Clone()
        {
            // WARNING: for some reason MemberwiseClone() is NOT WORKING on Mono/Unity
            // so we are creating a manual clone
            var clone = new BurstCompilerOptions
            {
                EnableBurstCompilation = EnableBurstCompilation,
                EnableBurstCompileSynchronously = EnableBurstCompileSynchronously,
                EnableBurstSafetyChecks = EnableBurstSafetyChecks,
                EnableEnhancedAssembly = EnableEnhancedAssembly,
                EnableFastMath = EnableFastMath,
                DisableOptimizations = DisableOptimizations,
                EnableBurstTimings = EnableBurstTimings
            };
            return clone;
        }

        public bool IsSupported(MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            BurstCompileAttribute attr;
            return TryGetAttribute(member, out attr);
        }

        private bool TryGetAttribute(MemberInfo member, out BurstCompileAttribute attribute)
        {
            attribute = null;
            // We don't fail if member == null as this method is being called by native code and doesn't expect to crash
            if (!IsEnabled || member == null)
            {
                return false;
            }

            // Fetch options from attribute
            attribute = member.GetCustomAttribute<BurstCompileAttribute>();
            return attribute != null;
        }

        public bool TryGetOptions(MemberInfo member, bool isJit, out string flagsOut)
        {
            flagsOut = null;
            BurstCompileAttribute attr;
            if (!TryGetAttribute(member, out attr))
            {
                return false;
            }
            var flagsBuilderOut = new StringBuilder();

            if (isJit && (attr.CompileSynchronously || _forceBurstCompilationSynchronously || EnableBurstCompileSynchronously))
            {
                AddOption(flagsBuilderOut, GetOption(OptionJitEnableSynchronousCompilation));
            }

            if (attr.FloatMode != FloatMode.Default)
            {
                AddOption(flagsBuilderOut, GetOption(OptionFloatMode, attr.FloatMode));
            }

            if (attr.FloatPrecision != FloatPrecision.Standard)
            {
                AddOption(flagsBuilderOut, GetOption(OptionFloatPrecision, attr.FloatPrecision));
            }

            if (isJit && EnableEnhancedAssembly)
            {
                AddOption(flagsBuilderOut, GetOption(OptionDebug));
            }

            if (DisableOptimizations)
            {
                AddOption(flagsBuilderOut, GetOption(OptionDisableOpt));
            }

            if (EnableFastMath)
            {
                AddOption(flagsBuilderOut, GetOption(OptionFastMath));
            }

            if (attr.Options != null)
            {
                foreach (var option in attr.Options)
                {
                    if (!string.IsNullOrEmpty(option))
                    {
                        AddOption(flagsBuilderOut, option);
                    }
                }
            }

            // Fetch options from attribute
            if (EnableBurstSafetyChecks)
            {
                AddOption(flagsBuilderOut, GetOption(OptionSafetyChecks));
            }
            else
            {
                AddOption(flagsBuilderOut, GetOption(OptionDisableSafetyChecks));
                // Enable NoAlias ahen safety checks are disable
                AddOption(flagsBuilderOut, GetOption(OptionNoAlias));
            }

            if (isJit && EnableBurstTimings)
            {
                AddOption(flagsBuilderOut, GetOption(OptionJitLogTimings));
            }

            flagsOut = flagsBuilderOut.ToString();
            return true;
        }
        
        private static void AddOption(StringBuilder builder, string option)
        {
            if (builder.Length != 0)
                builder.Append('\n'); // Use \n to separate options

            builder.Append(option);
        }
        public static string GetOption(string optionName, object value = null)
        {
            if (optionName == null) throw new ArgumentNullException(nameof(optionName));
            return "--" + optionName + (value ?? String.Empty);
        }

        private void OnOptionsChanged()
        {
            OptionsChanged?.Invoke();
        }

#if !UNITY_ZEROPLAYER && !UNITY_CSHARP_TINY
        /// <summary>
        /// Static initializer based on command line arguments
        /// </summary>
        static BurstCompilerOptions()
        {
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                switch (arg)
                {
                    case DisableCompilationArg:
                        _forceDisableBurstCompilation = true;
                        break;
                    case ForceSynchronousCompilationArg:
                        _forceBurstCompilationSynchronously = false;
                        break;
                }
            }
        }
#endif
#endif // !BURST_INTERNAL
    }

#if UNITY_EDITOR
    // NOTE: This must be synchronized with Backend.TargetPlatform
    internal enum TargetPlatform
    {
        Windows = 0,
        macOS = 1,
        Linux = 2,
        Android = 3,
        iOS = 4,
        PS4 = 5,
        XboxOne = 6,
        WASM = 7,
        UWP = 8,
    }

    // NOTE: This must be synchronized with Backend.TargetCpu
    internal enum TargetCpu
    {
        Auto = 0,
        X86_SSE2 = 1,
        X86_SSE4 = 2,
        X64_SSE2 = 3,
        X64_SSE4 = 4,
        AVX = 5,
        AVX2 = 6,
        AVX512 = 7,
        WASM32 = 8,
        ARMV7A_NEON32 = 9,
        ARMV8A_AARCH64 = 10,
        THUMB2_NEON32 = 11,
    }
#endif

    /// <summary>
    /// Flags used by <see cref="NativeCompiler.CompileMethod"/> to dump intermediate compiler results.
    /// </summary>
    [Flags]
#if BURST_INTERNAL
    public enum NativeDumpFlags
#else
    internal enum NativeDumpFlags
#endif
    {
        /// <summary>
        /// Nothing is selected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Dumps the IL of the method being compiled
        /// </summary>
        IL = 1 << 0,

        /// <summary>
        /// Dumps the reformated backend API Calls
        /// </summary>
        Backend = 1 << 1,

        /// <summary>
        /// Dumps the generated module without optimizations
        /// </summary>
        IR = 1 << 2,

        /// <summary>
        /// Dumps the generated backend code after optimizations (if enabled)
        /// </summary>
        IROptimized = 1 << 3,

        /// <summary>
        /// Dumps the generated ASM code (by default will also compile the function as using <see cref="Function"/> flag)
        /// </summary>
        Asm = 1 << 4,

        /// <summary>
        /// Generate the native code
        /// </summary>
        Function = 1 << 5,

        /// <summary>
        /// Dumps the result of analysis
        /// </summary>
        Analysis = 1 << 6,

        /// <summary>
        /// Dumps the diagnostics from optimisation
        /// </summary>
        IRPassAnalysis = 1 << 7,

        /// <summary>
        /// Dumps the IL before all transformation of the method being compiled
        /// </summary>
        ILPre = 1 << 8,

        All = IL | ILPre | Backend | IR | IROptimized | Asm | Function | Analysis | IRPassAnalysis
    }
}
#endif