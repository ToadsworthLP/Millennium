using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
    public static Type[] GetAllSubclasses(Type parentType){
        var children = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                       from type in assembly.GetTypes()
                       where type.IsSubclassOf(parentType)
                       select type;

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

}
