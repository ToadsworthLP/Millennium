# Changelog

## [1.0.0-preview.8] - 2019-03-28

- Fix for iOS symbol names growing too long, reduced footprint of function names via pretty printer and a hash

## [1.0.0-preview.7] - 2019-03-28

- Burst will now only generate debug information for AOT when targeting a Development Build.
- Added support for locating the build tools (standalone) for generating AOT builds on windows, without having to install Visual Studio complete
- Fix Log Timings was incorrectly being passed along to AOT builds, causing them to fail.
- Fix editor crash if burst aborted compilation half way through (because editor was being closed)
- Fix issue with job compilation that could be disabled when using the burst inspector
- Fix issue with spaces in certain paths (e.g. ANDROID_NDK_ROOT) when building for AOT
- Restore behavior of compiling ios projects from windows with burst, (burst does not support cross compiling for ios) - we still generate a valid output project, but with no burst code.
- Add support for Android embedded NDK.
- Fix issue where certain control flow involving object construction would crash the compiler in release mode

## [1.0.0-preview.6] - 2019-03-17

- Fix invalid codegen with deep nested conditionals
- Fix issue with burst menu "Enable Compilation" to also disable cache jobs
- Improve handling of PS4 toolchain detection

## [1.0.0-preview.5] - 2019-03-16

- Fix regression with JIT caching that was not properly recompiling changed methods
- Remove NativeDumpFlags from public API
- Remove usage of PropertyChangingEventHandler to avoid conflicts with custom Newtonsoft.Json
- Fix issue when a job could implement multiple job interfaces (IJob, IJobParallelFor...) but only the first one would be compiled

## [1.0.0-preview.4] - 2019-03-15

- Fix "Error while verifying module: Invalid bitcast" that could happen with return value in the context of deep nested conditionals
- Fix support for AOT compilation with float precision/mode
- Fix fast math for iOS/PS4
- Fix issue with double not using optimized intrinsics for scalars
- Fix issue when loading a MDB file was failing when building a standalone player
- Fix no-alias analysis that would be disabled in a standalone player if only one of the method was failing
- Fix bug with explicit layout struct returned as a pointer by a property but creating an invalid store
- Change `FloatPrecision.Standard` defaulting from `FloatPrecision.High` (ULP1) to `FloatPrecision.Medium` (ULP3.5)

## [1.0.0-preview.3] - 2019-03-14

- Fix compilation issue with uTiny builds

## [1.0.0-preview.2] - 2019-03-13

- Fix no-alias warning spamming when building a standalone player
- Improve the layout of the options/buttons for the inspector so that they at least attempt to layout better when the width is too small for all the buttons
- Fix formatting of error messages so the Unity Console can correctly parse the location as a clickable item (Note however it does not appear to allow double clicking on absolute paths)
- Change burst menu to Jobs/Burst. Improve order of menu items
- Fix for AOTSettings bug related to StandaloneWindows vs StandaloneWindows64

## [1.0.0-preview.1] - 2019-03-11

- Fix regression when resolving the type of generic used in a field
- Fix linker for XboxOne, UWP
- Fix performance codegen when using large structs
- Fix codegen when a recursive function is involved with platform dependent ABI transformations

## [0.2.4-preview.50] - 2019-02-27

- Fix meta file conflict
- Fix changelog format

## [0.2.4-preview.49] - 2019-02-27

- Move back com.unity.burst.experimental for function pointers support, but use internal modifier for this API
- Restructure package for validation

## [0.2.4-preview.48] - 2019-02-26

- Move back com.unity.burst.experimental for function pointers support, but use internal modifier for this API

## [0.2.4-preview.47] - 2019-02-26

- Fix an issue during publish stage which was preventing to release the binaries.

## [0.2.4-preview.46] - 2019-02-26

- iOS player builds now use static linkage (to support TestFlight)  - Minimum supported Unity versions are 2018.3.6f1 or 2019.1.0b4
- Fix a warning in Burst AOT settings
- Enable forcing synchronous job compilation from menu

## [0.2.4-preview.45] - 2019-02-07

- Disable Burst AOT settings support for unity versions before 2019.1

## [0.2.4-preview.44] - 2019-02-06

- Fix incorrect conversions when performing subtraction with enums and floats
- Fix compatability issue with future unity versions
- Fix bug with ldfld bitcast on structs with explicit layouts
- Guard against an issue resolving debug locations if the scope is global

## [0.2.4-preview.43] - 2019-02-01

- Add preliminary support for burst AOT settings in the player settings
- Move BurstCompile (delegate/function pointers support) from com.unity.burst package to com.unity.burst.experimental package
- Fix issue with stackalloc allocating a pointer size for the element type resulting in possible StackOverflowException
- Add support for disabling burst compilation from Unity editor with the command line argument `--burst-disable-compilation` 
- Add support for forcing synchronous compilation from Unity editor with the command line argument `--burst-force-sync-compilation`
- Fix a compiler crash when generating debugging information
- Fix invalid codegen involving ternary operator

## [0.2.4-preview.42] - 2019-01-22

- Fix a compilation error when implicit/explicit operators are used returning different type for the same input type

## [0.2.4-preview.41] - 2019-01-17

- Fix codegen issue with Interlocked.Decrement that was instead performing an increment
- Fix codegen issue for an invalid layout of struct with nested recursive pointer references 
- Fix for Fogbugz case : https://fogbugz.unity3d.com/f/cases/1109514/
- Fix codegen issue with ref bool on a method argument creating a compiler exception

## [0.2.4-preview.40] - 2018-12-19

- Fix bug when a write to a pointer type of an argument of a generic function
- Breaking change of API: `Accuracy` -> `FloatPrecision`, and `Support` => `FloatMode`
- Add `FloatMode.Deterministic` mode with early preview of deterministic mathematical functions
- Fix bug with fonts in inspector being incorrectly reloaded

## [0.2.4-preview.39] - 2018-12-06

- Add preview support for readonly static arrays typically used for LUT
- Fix an issue with generics incorrectly being resolved in certain situations
- Fix ARM32/ARM64 compilation issues for some instructions
- Fix ARM compilation issues on UWP
- Fix issue with math.compress
- Add support for `ldnull` for storing a managed null reference to a ref field (e.g for DisposeSentinel)

## [0.2.4-preview.38] - 2018-11-17

- Fix issue when converting an unsigned integer constant to a larger unsigned integer (e.g (ulong)uint.MaxValue)
- Fix crash in editor when IRAnalysis can return an empty string 
- Fix potential crash of Cecil when reading symbols from assembly definition

## [0.2.4-preview.37] - 2018-11-08

- Fix a crash on Linux and MacOS in the editor with dlopen crashing when trying to load burst-llvm (linux)

## [0.2.4-preview.36] - 2018-11-08

- Fix a crash on Linux and MacOS in the editor with dlopen crashing when trying to load burst-llvm (mac)

## [0.2.4-preview.35] - 2018-10-31

- Try to fix a crash on macosx in the editor when a job is being compiled by burst at startup time
- Fix burst accidentally resolving reference assemblies
- Add support for burst for ARM64 when building UWP player

## [0.2.4-preview.34] - 2018-10-12

- Fix compiler exception with an invalid cast that could occur when using pinned variables (e.g `int32&` resolved to `int32**` instead of `int32*`)

## [0.2.4-preview.33] - 2018-10-10

- Fix a compiler crash with methods incorrectly being marked as external and throwing an exception related to ABI

## [0.2.4-preview.32] - 2018-10-04

- Fix codegen and linking errors for ARM when using mathematical functions on plain floats
- Add support for vector types GetHashCode
- Add support for DllImport (only compatible with Unity `2018.2.12f1`+ and ` 2018.3.0b5`+)
- Fix codegen when converting uint to int when used in a binary operation

## [0.2.4-preview.31] - 2018-09-24

- Fix codegen for fmodf to use inline functions instead
- Add extended disassembly output to the burst inspector
- Fix generic resolution through de-virtualize methods
- Fix bug when accessing float3.zero. Prevents static constructors being considered intrinsics.
- Fix NoAlias attribute checking when generics are used

## [0.2.4-preview.30] - 2018-09-11

- Fix IsValueType throwing a NullReferenceException in case of using generics
- Fix discovery for burst inspector/AOT methods inheriting from IJobProcessComponentData or interfaces with generics
- Add NoAliasAttribute
- Improved codegen for csum
- Improved codegen for abs(int)
- Improved codegen for abs on floatN/doubleN

## [0.2.4-preview.29] - 2018-09-07

- Fix issue when calling an explicit interface method not being matched through a generic constraint
- Fix issue with or/and binary operation on a bool returned by a function

## [0.2.4-preview.28] - 2018-09-05

- Fix a compilation issue when storing a bool returned from a function to a component of a bool vector
- Fix AOT compilation issue with a duplicated dictionary key 
- Fix settings of ANDROID_NDK_ROOT if it is not setup in Unity Editor

## [0.2.4-preview.27] - 2018-09-03

- Improve detection of jobs within nested generics for AOT/burst inspector
- Fix compiler bug of comparison of a pointer to null pointer
- Fix crash compilation of sincos on ARM (neon/AARCH64) 
- Fix issue when using a pointer to a VectorType resulting in an incorrect access of a vector type
- Add support for doubles (preview)
- Improve AOT compiler error message/details if the compiler is failing before the linker

## [0.2.4-preview.26] - 2018-08-21

- Added support for cosh, sinh and tanh

## [0.2.4-preview.25] - 2018-08-16

- Fix warning in unity editor

## [0.2.4-preview.24] - 2018-08-15

- Improve codegen of math.compress
- Improve codegen of math.asfloat/asint/asuint
- Improve codegen of math.csum for int4
- Improve codegen of math.count_bits
- Support for lzcnt and tzcnt intrinsics
- Fix AOT compilation errors for PS4 and XboxOne
- Fix an issue that could cause wrong code generation for some unsafe ptr operations

## [0.2.4-preview.23] - 2018-07-31

- Fix bug with switch case to support not only int32

## [0.2.4-preview.22] - 2018-07-31

- Fix issue with pointers comparison not supported
- Fix a StackOverflow exception when calling an interface method through a generic constraint on a nested type where the declaring type is a generic
- Fix an issue with EntityCommandBuffer.CreateEntity/AddComponent that could lead to ArgumentException/IndexOutOfRangeException

## [0.2.4-preview.21] - 2018-07-25

- Correct issue with Android AOT compilation being unable to find the NDK.

## [0.2.4-preview.20] - 2018-07-05

- Prepare the user documentation for a public release

## [0.2.4-preview.19] - 2018-07-02

- Fix compilation error with generics when types are coming from different assemblies

## [0.2.4-preview.18] - 2018-06-26

- Add support for subtracting pointers

## [0.2.4-preview.17] - 2018-06-25

- Bump only to force a new version pushed


## [0.2.4-preview.16] - 2018-06-25

- Fix AOT compilation errors

## [0.2.4-preview.15] - 2018-06-25

- Fix crash for certain access to readonly static variable
- Fix StackOverflowException when using a generic parameter type into an interface method

## [0.2.4-preview.14] - 2018-06-23

- Fix an issue with package structure that was preventing burst to work in Unity

## [0.2.4-preview.13] - 2018-06-22

- Add support for burst timings menu
- Improve codegen for sin/cos
- Improve codegen when using swizzles on vector types
- Add support for sincos intrinsic
- Fix AOT deployment

## [0.2.4-preview.12] - 2018-06-13

- Fix a bug in codegen that was collapsing methods overload of System.Threading.Interlocked to the same method

## [0.2.4-preview.11] - 2018-06-05

- Fix exception in codegen when accessing readonly static fields from different control flow paths

## [0.2.4-preview.10] - 2018-06-04

- Fix a potential stack overflow issue when a generic parameter constraint on a type is also referencing another generic parameter through a generic interface constraint
- Update to latest Unity.Mathematics:
  - Fix order of parameters and codegen for step functions

## [0.2.4-preview.9] - 2018-05-29

- Fix bug when casting an IntPtr to an enum pointer that was causing an invalid codegen exception

## [0.2.4-preview.8] - 2018-05-24

- Breaking change: Move Unity.Jobs.Accuracy/Support to Unity.Burst
- Deprecate ComputeJobOptimizationAttribute in favor of BurstCompileAttribute
- Fix bug when using enum with a different type than int
- Fix bug with IL stind that could lead to a memory corruption.

## [0.2.4-preview.7] - 2018-05-22

- Add support for nested structs in SOA native arrays
- Add support for arbitrary sized elements in full SOA native arrays
- Fix bug with conversion from signed/unsigned integers to signed numbers (integers & floats)
- Add support for substracting pointers at IL level
- Improve codegen with pointers arithmetic to avoid checking for overflows

## [0.2.4-preview.6] - 2018-05-11

- Remove `bool1` from mathematics and add proper support in burst
- Add support for ARM platforms in the burst inspector UI

## [0.2.4-preview.5] - 2018-05-09

- Add support for readonly static fields
- Add support for stackalloc
- Fix potential crash on MacOSX when using memset is used indirectly
- Fix crash when trying to write to a bool1*
- Fix bug with EnableBurstCompilation checkbox not working in Unity Editor

## [0.2.4-preview.4] - 2018-05-03

- Fix an issue on Windows with `DllNotFoundException` occurring when trying to load `burst-llvm.dll` from a user profile containing unicode characters in the folder path
- Fix an internal compiler error occurring with IL dup instruction

## [0.2.4-preview.3] - 2018-05-03

- Add support for struct with an explicit layout
- Fix noalias regression (that was preventing the auto-vectorizer to work correctly on basic loops)

## 0.2.3 (21 March 2018)

- Improve error messages for static field access
- Improve collecting of compilable job by trying to collect concrete job type instances (issue #23)

## 0.2.2 (19 March 2018)

- Improve error messages in case using `is` or `as` cast in C#
- Improve error messages if a static delegate instance is used
- Fix codegen error when converting a byte/ushort to a float
