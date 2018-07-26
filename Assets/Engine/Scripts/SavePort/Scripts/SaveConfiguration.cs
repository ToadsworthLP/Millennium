using System.Collections.Generic;
using UnityEngine;

namespace SavePort.Saving {

    [CreateAssetMenu(fileName = "New SavePort Configuration", menuName = "SavePort Configuration")]
    public class SaveConfiguration : ScriptableObject {

        [SerializeField]
        private List<ContainerTableEntry> containerEntries;

        public List<ContainerTableEntry> GetContainerEntries() {
            return containerEntries;
        }

        internal void Initialize() {
            SaveManager.SetSaveConfiguration(this);
        }
    }

}
