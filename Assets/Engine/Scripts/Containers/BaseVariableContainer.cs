using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class UntypedVariableContainer : ScriptableObject{
    public abstract object UntypedValue { get; }
}

[Serializable]
public abstract class BaseVariableContainer<T> : UntypedVariableContainer {

    [SerializeField]
    private T serializedValue;

    [SerializeField]
    [HideInInspector]
    public UnityEvent OnValueUpdated = new UnityEvent();

    public T Value{
        get{
            return serializedValue;
        }

        set{
            serializedValue = Validate(value);
            InvokeUpdate();
        }
    }

    public abstract T Validate(T input);

    public override object UntypedValue
    {
        get { return Value; }
    }

    private void InvokeUpdate() {
        if (OnValueUpdated != null) {
            OnValueUpdated.Invoke();
        }
    }
}