using System;
using UnityEngine.Events;

[Serializable]
public abstract class UntypedVariableReference{
    public bool useLocalValue = false;
}

[Serializable]
public abstract class BaseVariableReference<VariableType, ContainerType> : UntypedVariableReference where ContainerType : BaseVariableContainer<VariableType>
{
    public VariableType localValue;
    public ContainerType containerReference;

    public VariableType Value
    {
        get {
            if (useLocalValue)
                return localValue;

            return containerReference.Value;
        }

        set {
            if (useLocalValue) {
                localValue = value;
            } else {
                containerReference.Value = value;
            }
        }
    }

    public void AddUpdateListener(UnityAction call){
        containerReference.OnValueUpdated.AddListener(call);
    }

    public void RemoveUpdateListener(UnityAction call) {
        containerReference.OnValueUpdated.RemoveListener(call);
    }

    public void RemoveAllUpdateListeners() {
        containerReference.OnValueUpdated.RemoveAllListeners();
    }

    public void ForceUpdate() {
        if(!useLocalValue && containerReference.OnValueUpdated != null){
            containerReference.OnValueUpdated.Invoke();
        }
    }

    public static implicit operator VariableType(BaseVariableReference<VariableType, ContainerType> variable) {
        return variable.Value;
    }

    public override string ToString() {
        return Value.ToString();
    }

}