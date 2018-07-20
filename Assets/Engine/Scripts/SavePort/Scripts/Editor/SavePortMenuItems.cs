using SavePort.Saving;
using System.Collections.Generic;
using UnityEditor;

public class SavePortMenuItems {

    private static List<SaveConfiguration> configAssets;

    [MenuItem("Edit/SavePort/Run Configuration Cleanup")]
    public static void RunConfigCleanup() {
        if(configAssets == null) configAssets = SavePortEditorUtils.FindAllAssetsOfType<SaveConfiguration>();

        foreach (SaveConfiguration config in configAssets) {
            List<ContainerTableEntry> emptyEntries = new List<ContainerTableEntry>();

            foreach (ContainerTableEntry entry in config.GetContainerEntries()) {
                if (entry.container == null) {
                    emptyEntries.Add(entry);
                }
            }

            foreach (ContainerTableEntry entry in emptyEntries) {
                config.GetContainerEntries().Remove(entry);
            }

            EditorUtility.SetDirty(config);
        }
    }
}
