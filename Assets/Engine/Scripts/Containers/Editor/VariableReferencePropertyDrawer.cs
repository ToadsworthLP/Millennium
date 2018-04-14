using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UntypedVariableReference), true)]
public class VariableReferencePropertyDrawer : PropertyDrawer
{
    private bool useLocalValue;
    private GUIStyle localToggleStyle;

    private GUIContent localContent;
    private GUIContent containerContent;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        SerializedProperty isLocalProp = property.FindPropertyRelative("useLocalValue");
        SerializedProperty localValueProp = property.FindPropertyRelative("localValue");
        SerializedProperty containerRefProp = property.FindPropertyRelative("containerReference");

        if(localToggleStyle == null){
            localToggleStyle = new GUIStyle();

            localToggleStyle.fixedWidth = 15;
            localToggleStyle.fixedHeight = 15;

            useLocalValue = isLocalProp.boolValue;
        }

        if(localContent == null){
            localContent = new GUIContent("L");
            containerContent = new GUIContent("C");
        }

        label = EditorGUI.BeginProperty(position, label, property);

        Rect localTogglePosition = position;
        localTogglePosition.xMin = position.xMax - 12;

        position.xMax -= 18;


        if(GUI.Button(localTogglePosition, useLocalValue ? localContent : containerContent, localToggleStyle)){
            useLocalValue =! useLocalValue;
            isLocalProp.boolValue = useLocalValue;
        }

        if (useLocalValue){
            EditorGUI.PropertyField(position, localValueProp, label);
        }else{
            EditorGUI.PropertyField(position, containerRefProp, label);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight + 2;
    }
}
