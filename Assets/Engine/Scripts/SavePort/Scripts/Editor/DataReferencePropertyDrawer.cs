using UnityEditor;
using UnityEngine;

namespace SavePort.EditorOnly {
    [CustomPropertyDrawer(typeof(UntypedDataReference), true)]
    public class DataReferencePropertyDrawer : PropertyDrawer {
        private bool localMode;
        private GUIStyle localToggleStyle;

        private GUIContent localButtonContent;
        private GUIContent containerButtonContent;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            SerializedProperty isLocalProp = property.FindPropertyRelative("localMode");
            SerializedProperty localDataProp = property.FindPropertyRelative("localData");
            SerializedProperty containerProp = property.FindPropertyRelative("container");

            if (localToggleStyle == null) {
                localToggleStyle = new GUIStyle();

                localToggleStyle.fixedWidth = 15;
                localToggleStyle.fixedHeight = 15;

                localMode = isLocalProp.boolValue;
            }

            if (localButtonContent == null) {
                localButtonContent = new GUIContent("L");
                containerButtonContent = new GUIContent("C");
            }

            label = EditorGUI.BeginProperty(position, label, property);

            Rect localTogglePosition = position;
            localTogglePosition.xMin = position.xMax - 12;

            position.xMax -= 18;


            if (GUI.Button(localTogglePosition, localMode ? localButtonContent : containerButtonContent, localToggleStyle)) {
                localMode = !localMode;
                isLocalProp.boolValue = localMode;
            }

            if (localMode) {
                EditorGUI.PropertyField(position, localDataProp, label);
            } else {
                EditorGUI.PropertyField(position, containerProp, label);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight + 2;
        }

    } 
}