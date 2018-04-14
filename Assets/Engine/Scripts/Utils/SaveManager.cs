using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour {

    public string fileName = "save.dat";
    public HammerAsset defaultHammer;
    public Backpack backpack;

    public SaveData LoadGame() {
        SaveData data = new SaveData();

        try {
            using (StreamReader file = new StreamReader(Application.persistentDataPath + "/" + fileName)) {
                String dataString = file.ReadToEnd();
                data = Utils.Deserialize<SaveData>(dataString);

                if(data.shelfSerialized != null) {
                    data.shelf = Utils.DeserializeShelf(data.shelfSerialized);
                } else{
                    data.shelf = new Dictionary<string, object>();
                }

                if (data.playtimeSerialized != null) {
                    data.playtime = JsonConvert.DeserializeObject<DateTime>(data.playtimeSerialized);
                } else {
                    data.playtime = new DateTime(0);
                }

                if(data.currentHammer == null) {
                    data.currentHammer = defaultHammer;
                }
            }
            return data;
        } catch (Exception e) {
            print("Failed to load data! " + e.ToString());
            return data.GetDefaults();
        }
    }

    public bool SaveGame() {
        try {
            using (StreamWriter file = new StreamWriter(Application.persistentDataPath + "/" + fileName)) {
                SaveData data = new SaveData();

                data.hp = backpack.HP;
                data.maxHp = backpack.maxHP;
                data.fp = backpack.FP;
                data.maxFp = backpack.maxFP;
                data.sp = backpack.SP;
                data.maxSp = backpack.maxSP;
                data.bp = backpack.BP;
                data.starPoints = backpack.starPoints;
                data.level = backpack.level;
                data.coins = backpack.coins;
                data.shineSprites = backpack.shineSprites;
                data.starPieces = backpack.starPieces;
                data.items = backpack.inventory;

                data.playtime = backpack.GetPlaytime();
                data.shelf = backpack.shelf;
                data.currentScene = SceneManager.GetActiveScene().name;
                data.currentPosition = FindObjectOfType<PlayerMachine>().transform.position + new Vector3(0, -0.5f, 0);
                data.shelfSerialized = Utils.SerializeShelf(data.shelf);
                data.playtimeSerialized = JsonConvert.SerializeObject(data.playtime);

                file.WriteLine(Utils.Serialize(data));
            }
            return true;
        } catch (Exception e) {
            print("Failed to save data! " + e.ToString());
            return false;
        }
    }

    [Serializable]
    public class SaveData
    {
        public string playerName;
        public int maxHp;
        public int hp;
        public int maxFp;
        public int fp;
        public int maxSp;
        public int sp;
        public int coins;
        public int starPoints;
        public int level;
        public int bp;
        public int shineSprites;
        public int starPieces;
        public string currentScene;
        public Vector3 currentPosition;
        public List<BaseItem> items;
        public HammerAsset currentHammer;
        public Dictionary<string, object> shelf;
        public string shelfSerialized;
        public DateTime playtime;
        public string playtimeSerialized;

        public SaveData GetDefaults() {
            playerName = "Mario";
            maxHp = 10;
            hp = 10;
            maxFp = 5;
            fp = 5;
            coins = 0;
            starPoints = 0;
            level = 1;
            bp = 3;
            shineSprites = 0;
            starPieces = 0;
            playtime = new DateTime(0);
            currentScene = "TestMap";
            currentPosition = Vector3.zero;
            items = new List<BaseItem>();
            shelf = new Dictionary<string, object>();
            return this;
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(SaveManager))]
public class BackpackEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Reset save data")) {
            SaveManager saveManager = (SaveManager)target;

            try {
                using (StreamWriter file = new StreamWriter(Application.persistentDataPath + "/" + saveManager.fileName)) {
                    SaveManager.SaveData data = new SaveManager.SaveData().GetDefaults();
                    data.currentHammer = saveManager.defaultHammer;
                    file.WriteLine(Utils.Serialize(data));
                }
            } catch (Exception e) {
                Debug.Log("Failed to reset data! " + e.ToString());
            }
        }
    }
}
#endif