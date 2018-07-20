using System;
using UnityEngine;
using UnityEngine.Events;

namespace SavePort {

    [Serializable]
    public abstract class UntypedDataReference {

        public bool localMode = false;

    }

    [Serializable]
    public abstract class BaseDataReference<DataType, ContainerType> : UntypedDataReference where ContainerType : BaseDataContainer<DataType> {

        [SerializeField]
        private ContainerType container;
        [SerializeField]
        private DataType localData;

        public DataType Value {
            get {
                if (localMode)
                    return localData;

                return container.Value;
            }

            set {
                if (localMode) {
                    localData = value;
                } else {
                    container.Value = value;
                }
            }
        }

        public void AddUpdateListener(UnityAction action) {
            if (localMode) return;
            container.AddUpdateListener(action);
        }

        public void RemoveUpdateListener(UnityAction action) {
            if (localMode) return;
            container.RemoveUpdateListener(action);
        }

        public void RemoveAllUpdateListeners() {
            if (localMode) return;
            container.RemoveAllUpdateListeners();
        }

        public void ForceUpdate() {
            if (localMode) return;
            container.ForceUpdate();
        }

        public static implicit operator DataType(BaseDataReference<DataType, ContainerType> reference) {
            return reference.Value;
        }

        public override string ToString() {
            return Value.ToString();
        }

    }

}
