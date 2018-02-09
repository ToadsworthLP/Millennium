using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

    //Shelf serialization - Using Json.NET because it's way faster than BinarySerializer *and* produces better results
    public static string SerializeShelf(Dictionary<string, object> obj) {
        if (obj == null) {
            return null;
        }

        return JsonConvert.SerializeObject(obj);
    }

    public static Dictionary<string, object> DeserializeShelf(string json) {
        if(json.Length > 0){
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
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

    //Custom interpolation utilities
    //Interpolates between a and b by t using the given curve

    //Here, the end of the sequence is determined by the last keyframe
    public static float InterpolateByCurve(float a, float b, float t, AnimationCurve curve){
        return Mathf.Lerp(a, b, curve.Evaluate(curve.keys[curve.length-1].time * t));
    }

    public static Vector3 InterpolateByCurve(Vector3 a, Vector3 b, float t, AnimationCurve curve) {
        return Vector3.Lerp(a, b, curve.Evaluate(curve.keys[curve.length - 1].time * t));
    }

    public static Quaternion InterpolateByCurve(Quaternion a, Quaternion b, float t, AnimationCurve curve) {
        return Quaternion.Lerp(a, b, curve.Evaluate(curve.keys[curve.length - 1].time * t));
    }

    //Here, the end of the sequence is always time 1, regardless of how long the curve actually is
    public static float InterpolateByCurveAbsolute(float a, float b, float t, AnimationCurve curve) {
        return Mathf.Lerp(a, b, curve.Evaluate(t));
    }

    public static Vector3 InterpolateByCurveAbsolute(Vector3 a, Vector3 b, float t, AnimationCurve curve) {
        return Vector3.Lerp(a, b, curve.Evaluate(t));
    }

    public static Quaternion InterpolateByCurveAbsolute(Quaternion a, Quaternion b, float t, AnimationCurve curve) {
        return Quaternion.Lerp(a, b, curve.Evaluate(t));
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
