using System;
using UnityEngine;
using UnityEngine.Events;

namespace SavePort {

    [Serializable]
    public abstract class UntypedDataContainer : ScriptableObject {

        public abstract object UntypedValue { get; set; }
        public abstract Type ValueType { get; }

        [NonSerialized]
        protected UnityEvent OnValueUpdated = new UnityEvent();

        public void AddUpdateListener(UnityAction action) {
            OnValueUpdated.AddListener(action);
        }

        public void RemoveUpdateListener(UnityAction action) {
            OnValueUpdated.RemoveListener(action);
        }

        public void RemoveAllUpdateListeners() {
            OnValueUpdated.RemoveAllListeners();
        }

        public void ForceUpdate() {
            if (OnValueUpdated != null) {
                OnValueUpdated.Invoke();
            }
        }

    }

    [Serializable]
    public abstract class BaseDataContainer<DataType> : UntypedDataContainer, ISerializationCallbackReceiver {

        [SerializeField, Tooltip("The value which is saved to the container asset. Modifying it at runtime will update the runtime value as well.")]
        private DataType serializedValue;

        [NonSerialized]
        private DataType runtimeValue;

        [SerializeField, Tooltip("Whether the input is validated using the type-specific implementation of the Validate() function.")]
        private bool validateInput = false;

        public abstract DataType Validate(DataType input);

        public DataType Value {
            get {
                return runtimeValue;
            }

            set {
                if (validateInput) {
                    runtimeValue = Validate(value);
                } else {
                    runtimeValue = value;
                }
                InvokeUpdate();
            }
        }

#if UNITY_EDITOR
        private DataType previousValue; //DON'T USE OUTSIDE THIS EDITOR ONLY PART

        private void OnValidate() {
            if (validateInput && !serializedValue.Equals(previousValue)) {
                serializedValue = Validate(serializedValue);
                runtimeValue = serializedValue;
            }

            previousValue = serializedValue;
        }
#endif

        public override object UntypedValue {
            get {
                return (object)Value;
            }

            set {
                Value = (DataType)value;
            }
        }

        public override Type ValueType {
            get {
                return Value.GetType();
            }
        }

        public static implicit operator DataType(BaseDataContainer<DataType> container) {
            return container.Value;
        }

        public override string ToString() {
            return Value.ToString();
        }

        private void InvokeUpdate() {
            if (OnValueUpdated != null) {
                OnValueUpdated.Invoke();
            }
        }

        public void OnBeforeSerialize() {}

        public void OnAfterDeserialize() {
            runtimeValue = serializedValue;
        }
    }

}
