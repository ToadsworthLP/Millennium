using SubjectNerd.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace SavePort.Saving {

    [CreateAssetMenu(fileName = "New SavePort Configuration", menuName = "SavePort Configuration")]
    public class SaveConfiguration : ScriptableObject {

        [SerializeField]
        [Reorderable]
        private List<ContainerTableEntry> containerEntries = new List<ContainerTableEntry>();

        public List<ContainerTableEntry> GetContainerEntries() {
            return containerEntries;
        }

        internal void Initialize() {
            SaveManager.SetSaveConfiguration(this);
        }
    }

}
