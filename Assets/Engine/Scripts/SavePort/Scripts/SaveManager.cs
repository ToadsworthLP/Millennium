using OdinSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SavePort.Saving {

    public static class SaveManager {

        /// <summary>
        /// This version number is added to the start of every SavePort save file.
        /// Increase it if you change the file structure or format in an incompatible way!
        /// </summary>
        public const byte formatVersion = 1;

        private static SaveConfiguration configuration;

        internal static void SetSaveConfiguration(SaveConfiguration config) {
            configuration = config;
        }

        public static SaveConfiguration GetSaveConfiguration() {
            if (configuration != null) {
                return configuration;
            } else {
                Debug.LogError("Error loading save configuration! Returning default instance to avoid crashes!");
                return ScriptableObject.CreateInstance<SaveConfiguration>();
            }
        }

        public static bool SaveContainers(string fileName) {
            try {
                using (BinaryWriter writer = new BinaryWriter(File.Open(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate))) {
                    Dictionary<string, object> dataDict = new Dictionary<string, object>();

                    foreach (ContainerTableEntry entry in configuration.GetContainerEntries()) {
                        dataDict.Add(entry.ID, entry.container.UntypedValue);
                    }

                    List<UnityEngine.Object> unityObjects = new List<UnityEngine.Object>();
                    byte[] serializedData = SerializationUtility.SerializeValue(dataDict, DataFormat.Binary, out unityObjects);

                    string unityObjectJson = JsonUtility.ToJson(new UnityObjectList(unityObjects));
                    byte[] unityObjectRefs = Encoding.ASCII.GetBytes(unityObjectJson);

                    writer.Write(Encoding.ASCII.GetBytes("SPDAT"));
                    writer.Write(formatVersion);

                    writer.Write((UInt32)unityObjectRefs.Length);
                    writer.Write((UInt32)serializedData.Length);
                    writer.Write(unityObjectRefs);
                    writer.Write(serializedData);
                }
                return true;
            }
            catch (Exception e) {
                Debug.LogError("Failed to save data! " + e.ToString());
                return false;
            }
        }

        public static bool LoadContainers(string fileName, bool ignoreFormatVersion = false) {
            try {
                using (BinaryReader reader = new BinaryReader(File.Open(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate))) {
                    if (reader.BaseStream.Length == 0) return false;

                    string filePrefix = Encoding.ASCII.GetString(reader.ReadBytes(5));
                    byte fileVersion = reader.ReadByte();

                    if (!ignoreFormatVersion) {
                        if (filePrefix != "SPDAT") {
                            Debug.LogError("The file " + fileName + " is not a SavePort data file!");
                            return false;
                        } else if (fileVersion != formatVersion) {
                            Debug.LogError("The file " + fileName + " uses SPDAT format version " + fileVersion + ", but the current one is " + formatVersion + "! It seems like the file format has been updated in an incompatible way since this file was created!");
                            //You could put save file updating code here - if you do, return whether the update was successful instead of 'false'
                            //The recommended way to do this is to use your own method to make your own version of LoadContainers which is compatible with the old version.
                            return false;
                        }
                    }

                    UInt32 unityObjectLength = reader.ReadUInt32();
                    UInt32 serializedDataLength = reader.ReadUInt32();

                    byte[] unityObjectRefs = reader.ReadBytes((int)unityObjectLength);
                    byte[] serializedData = reader.ReadBytes((int)serializedDataLength);

                    string unityObjectJson = Encoding.ASCII.GetString(unityObjectRefs);

                    List<UnityEngine.Object> unityObjects = JsonUtility.FromJson<UnityObjectList>(unityObjectJson);
                    Dictionary<string, object> dataDict = SerializationUtility.DeserializeValue<Dictionary<string, object>>(serializedData, DataFormat.Binary, unityObjects);

                    foreach (ContainerTableEntry entry in configuration.GetContainerEntries()) {
                        object data;
                        dataDict.TryGetValue(entry.ID, out data);

                        if (data != null) {
                            if (data.GetType().Equals(entry.container.ValueType)) {
                                entry.container.UntypedValue = data;
                            } else {
                                Debug.LogWarning("Data type mismatch for container " + entry.ID + ": Container value type: " + entry.container.ValueType + ", Serialized value type: " + data.GetType());
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e) {
                Debug.LogError("Failed to load data! " + e.ToString() + e.StackTrace);
                return false;
            }
        }

    }

    [Serializable]
    public struct ContainerTableEntry {
        public string ID;
        public UntypedDataContainer container;

        public ContainerTableEntry(string ID, UntypedDataContainer container) {
            this.ID = ID;
            this.container = container;
        }

        public static implicit operator UntypedDataContainer(ContainerTableEntry entry) {
            return entry.container;
        }
    }

    [Serializable]
    struct UnityObjectList {
        public List<UnityEngine.Object> objects;

        public UnityObjectList(List<UnityEngine.Object> objectList) {
            this.objects = objectList;
        }

        public static implicit operator List<UnityEngine.Object>(UnityObjectList objectList) {
            return objectList.objects;
        }
    }

}
