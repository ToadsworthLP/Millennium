#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ShowIfAttribute : PropertyAttribute {

    public string propertyName;
    public object propertyValue;

    public ShowIfAttribute(string propertyName, object propertyValue) {
        this.propertyName = propertyName;
        this.propertyValue = propertyValue;
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfAttributeDrawer : PropertyDrawer{

    private ShowIfAttribute showIfAttribute;
    private SerializedProperty checkProperty;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if(showIfAttribute == null || checkProperty == null){
            showIfAttribute = (ShowIfAttribute)attribute;
            checkProperty = property.serializedObject.FindProperty(showIfAttribute.propertyName);
        }

        bool show = PropertyShown(checkProperty, showIfAttribute.propertyValue);

        if (show){
            EditorGUI.PropertyField(position, property);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (showIfAttribute == null || checkProperty == null) {
            showIfAttribute = (ShowIfAttribute)attribute;
            checkProperty = property.serializedObject.FindProperty(showIfAttribute.propertyName);
        }

        if (PropertyShown(checkProperty, showIfAttribute.propertyValue))
            return base.GetPropertyHeight(property, label);

        return 0;
    }

    //This actually checks if the property should be shown. You can put custom checks here.
    private bool PropertyShown(SerializedProperty checkProperty, object value){
        string checkPropertyType = checkProperty.type;

        if (checkProperty == null){
            Debug.LogWarning("The ShowIf attribute was given an invalid property name to check for!" + Utils.NewLine() + "Defaulting to property shown.");
            return true;
        }

        if (checkPropertyType == "bool") {
            return checkProperty.boolValue.Equals(value);
        } else if (checkPropertyType == "Enum") {
            return checkProperty.enumNames[checkProperty.enumValueIndex].Equals(value.ToString());
        } else {
            Debug.LogWarning("The ShowIf attribute only works if the checked property is a boolean or an enum, but " + checkProperty.type + " was given!" + Utils.NewLine() + "Defaulting to property shown.");
            return true;
        }
    }

}
#endif
