using SavePort;
using SavePort.Saving;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UntypedDataContainer), true)]
public class DataContainerInspector : Editor {

    private UntypedDataContainer container;

    private void OnEnable() {
        container = (UntypedDataContainer)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Runtime Value");
        EditorGUILayout.SelectableLabel(container.UntypedValue.ToString());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Force Value Update Event")) {
            container.ForceUpdate();
        }

        if (GUILayout.Button("Saving & Loading")) {
            GenericMenu menu = new GenericMenu();
            List<SaveConfiguration> configurations = SavePortEditorUtils.FindAllAssetsOfType<SaveConfiguration>();

            if (configurations.Count == 0) {
                EditorUtility.DisplayDialog("SavePort", "No SavePort configuration files were found in this project. Please create one to proceed.", "OK");
                return;
            }

            menu.AddDisabledItem(new GUIContent("Select configuration"));

            foreach (SaveConfiguration config in configurations) {
                bool isSaved = ArrayUtility.Contains(config.GetContainerEntries().Select(x => x.container).ToArray(), container);
                menu.AddItem(new GUIContent(config.name), isSaved, () => ProcessConfigMenu(container, config, isSaved));
            }

            menu.ShowAsContext();
        }

        GUILayout.EndHorizontal();
    }

    private static void ProcessConfigMenu(UntypedDataContainer container, SaveConfiguration config, bool isSaved) {
        if (isSaved) {
            foreach (ContainerTableEntry entry in config.GetContainerEntries()) {
                if (entry.container.Equals(container)) {
                    config.GetContainerEntries().Remove(entry);
                    EditorUtility.SetDirty(config);
                    return;
                }
            }
        } else {
            string[] savedIDs = config.GetContainerEntries().Select(x => x.ID).ToArray();

            if (savedIDs.Contains(container.name)) {
                config.GetContainerEntries().Add(new ContainerTableEntry(container.name + "_" + container.GetInstanceID(), container));
            } else {
                config.GetContainerEntries().Add(new ContainerTableEntry(container.name, container));
            }

            EditorUtility.SetDirty(config);
        }
    }

}
