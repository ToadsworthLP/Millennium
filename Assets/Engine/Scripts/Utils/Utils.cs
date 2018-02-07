using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class Utils {

    //Utility enums
    public enum EnumDirection
    {
        LEFT, RIGHT, UP, DOWN, UNKNOWN
    }

    //Enum utility methods
    public static EnumDirection GetOpposite(this EnumDirection direction) {
        if (direction == EnumDirection.LEFT) {
            return EnumDirection.RIGHT;
        } else if (direction == EnumDirection.RIGHT) {
            return EnumDirection.LEFT;
        } else if (direction == EnumDirection.UP) {
            return EnumDirection.DOWN;
        } else if (direction == EnumDirection.DOWN) {
            return EnumDirection.UP;
        }
        return EnumDirection.UNKNOWN;
    }

    //Dictionary serialization
    public static byte[] SerializeDict(this Dictionary<string, object> obj) {
        if (obj == null) {
            return null;
        }

        using (var memoryStream = new MemoryStream()) {
            var binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(memoryStream, obj);

            return memoryStream.ToArray();
        }
    }

    public static Dictionary<string, object> DeSerializeDict(this byte[] arrBytes) {
        if(arrBytes.Length > 0){
            using (var memoryStream = new MemoryStream()) {
                var binaryFormatter = new BinaryFormatter();

                memoryStream.Write(arrBytes, 0, arrBytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return (Dictionary<string, object>)binaryFormatter.Deserialize(memoryStream);
            }
        }
        return new Dictionary<string, object>();
    }

    //Utility methods
    public static string NewLine(){
        return "\r\n";
    }

    public static Transform ClearChildren(this Transform transform) {
        foreach (Transform child in transform) {
            UnityEngine.Object.Destroy(child.gameObject);
        }
        return transform;
    }

    //Save serialization utilities
    public static string Serialize(object value) {
        return JsonUtility.ToJson(value);
    }

    public static T Deserialize<T>(string json) {
        return JsonUtility.FromJson<T>(json);
    }

    //Reflection utilities
    public static Type[] GetAllSubclasses(Type parentType, bool allowAbstractClasses = false){
        IEnumerable<Type> children;

        if(allowAbstractClasses){
            children = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                       from type in assembly.GetTypes()
                       where type.IsSubclassOf(parentType)
                       select type;
        } else{
            children = from assembly in AppDomain.CurrentDomain.GetAssemblies()
              from type in assembly.GetTypes()
              where type.IsSubclassOf(parentType)
              where !type.IsAbstract
              select type;
        }

        return children.ToArray();
    }

    //Stat utilities
    [Serializable]
    public class StatModifier
    {
        public StatType statToModify;
        public int value;
    }

    [Serializable]
    public enum StatType
    {
        HP, FP, SP, ATTACK, DEFENSE
    }

    //Animator utilities
    [Serializable]
    public class AnimationModifier
    {
        public AnimationModifierType propertyType;
        public string propertyName;

        public int intValue;
        public float floatValue;
        public bool boolValue;

        public enum AnimationModifierType { INT, FLOAT, BOOL, TRIGGER }

        public void SetProperty(Animator animator) {
            switch (propertyType) {
                case AnimationModifierType.INT:
                    animator.SetInteger(propertyName, intValue);
                    break;
                case AnimationModifierType.FLOAT:
                    animator.SetFloat(propertyName, floatValue);
                    break;
                case AnimationModifierType.BOOL:
                    animator.SetBool(propertyName, boolValue);
                    break;
                case AnimationModifierType.TRIGGER:
                    animator.SetTrigger(propertyName);
                    break;
            }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AnimationModifier))]
    public class AnimationModifierDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, new GUIContent("Animation Modifier"));

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            EditorGUI.PropertyField(GetRectForRow(position, 1), property.FindPropertyRelative("propertyName"), new GUIContent("Property Name"));
            EditorGUI.PropertyField(GetRectForRow(position, 2), property.FindPropertyRelative("propertyType"), new GUIContent("Property Type"));

            int modifierTypeEnumIndex = property.FindPropertyRelative("propertyType").enumValueIndex;

            switch (modifierTypeEnumIndex) {
                case 0:
                    EditorGUI.PropertyField(GetRectForRow(position, 3), property.FindPropertyRelative("intValue"), new GUIContent("Integer Value"));
                    break;
                case 1:
                    EditorGUI.PropertyField(GetRectForRow(position, 3), property.FindPropertyRelative("floatValue"), new GUIContent("Float Value"));
                    break;
                case 2:
                    EditorGUI.PropertyField(GetRectForRow(position, 3), property.FindPropertyRelative("boolValue"), new GUIContent("Boolean Value"));
                    break;
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(property.FindPropertyRelative("propertyType").enumValueIndex < 3)
                return 64;
            return 48;
        }

        private Rect GetRectForRow(Rect baseRect, int row) {
            return new Rect(baseRect.x, baseRect.y + 16 * row, baseRect.width, 15);
        }

    }
#endif
}
