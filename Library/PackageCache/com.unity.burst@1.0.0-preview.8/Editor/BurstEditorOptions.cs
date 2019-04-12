using System.ComponentModel;
using System.Diagnostics;
using UnityEditor;

namespace Unity.Burst.Editor
{
    /// <summary>
    /// Responsible to synchronize <see cref="BurstCompilerOptions.Global"/> with the menu
    /// </summary>
    internal static class BurstEditorOptions
    {
        private const string EnableBurstSafetyChecksText = "BurstSafetyChecks";
        private const string EnableBurstCompilationText = "BurstCompilation";
        private const string EnableBurstTimingsText = "BurstShowTimings";
        private const string EnableBurstCompileSynchronouslyText = "BurstCompileSynchronously";

        /// <summary>
        /// <c>true</c> if the menu options are synchronized with <see cref="BurstCompilerOptions.Global"/>
        /// </summary>
        private static bool _isSynchronized;

        public static void EnsureSynchronized()
        {
            GetGlobalOptions();
        }

        public static bool EnableBurstCompilation
        {
            get => GetGlobalOptions().EnableBurstCompilation;
            set => GetGlobalOptions().EnableBurstCompilation = value;
        }

        public static bool EnableBurstSafetyChecks
        {
            get => GetGlobalOptions().EnableBurstSafetyChecks;
            set => GetGlobalOptions().EnableBurstSafetyChecks = value;
        }

        public static bool EnableBurstCompileSynchronously
        {
            get => GetGlobalOptions().EnableBurstCompileSynchronously;
            set => GetGlobalOptions().EnableBurstCompileSynchronously = value;
        }

        public static bool EnableBurstTimings
        {
            get => GetGlobalOptions().EnableBurstTimings;
            set => GetGlobalOptions().EnableBurstTimings = value;
        }


        private static BurstCompilerOptions GetGlobalOptions()
        {
            var global = BurstCompilerOptions.Global;
            // If options are not synchronize with our global instance, setup the sync
            if (!_isSynchronized)
            {
                // setup the synchronization
                global.EnableBurstCompilation = EditorPrefs.GetBool(EnableBurstCompilationText, true);
                global.EnableBurstSafetyChecks = EditorPrefs.GetBool(EnableBurstSafetyChecksText, true);
                global.EnableBurstCompileSynchronously = EditorPrefs.GetBool(EnableBurstCompileSynchronouslyText, false);
                global.EnableBurstTimings = EditorPrefs.GetBool(EnableBurstTimingsText, false);

                global.OptionsChanged += GlobalOnOptionsChanged;
                _isSynchronized = true;
            }

            return global;
        }

        private static void GlobalOnOptionsChanged()
        {
            // We are not optimizing anything here, so whenever one option is set, we reset all of them
            EditorPrefs.SetBool(EnableBurstCompilationText, BurstCompilerOptions.Global.EnableBurstCompilation);
            EditorPrefs.SetBool(EnableBurstSafetyChecksText, BurstCompilerOptions.Global.EnableBurstSafetyChecks);
            EditorPrefs.SetBool(EnableBurstCompileSynchronouslyText, BurstCompilerOptions.Global.EnableBurstCompileSynchronously);
            EditorPrefs.SetBool(EnableBurstTimingsText, BurstCompilerOptions.Global.EnableBurstTimings);
        }
    }
}