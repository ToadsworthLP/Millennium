using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class Backpack : MonoBehaviour {

    private GameManager gameManager;
    public string startSceneName;
    public Vector3 startPosition;
    public HammerAsset defaultHammer;
    public string savefileName;

    private PlayerData data;
    private Stopwatch deltaPlaytime;

    void Start() {
        DontDestroyOnLoad(gameObject);
        if(!LoadData()){
            data = new PlayerData().GetDefaults();
        }
        SceneManager.sceneLoaded += SceneLoaded;
        SceneLoaded(SceneManager.GetSceneByName(data.currentScene), LoadSceneMode.Single);
        deltaPlaytime = Stopwatch.StartNew();
    }

    public void SceneLoaded(Scene scene, LoadSceneMode mode) {
        if (SceneManager.GetSceneByName(data.currentScene).Equals(scene)) {
            gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            if(data.currentPosition == Vector3.zero){
                gameManager.playerMachine.transform.position = startPosition;
            } else{
                gameManager.playerMachine.transform.position = data.currentPosition;
            }
            StartCoroutine(InitializeHUD());
        }
    }

    IEnumerator InitializeHUD(){
        yield return new WaitForEndOfFrame();
        gameManager.hudController.SetData(data, true);
        gameManager.blackOverlay.FadeOut();
    }

    //Getters and setters
    public string playerName
    {
        get {
            return data.playerName;
        }

        set {
            data.playerName = value;
        }
    }
    public int hp
    {
        get {
            return data.hp;
        }

        set {
            data.hp = Mathf.Clamp(value, 0, maxHp);
            gameManager.hudController.SetHP(data.hp);
        }
    }
    public int maxHp
    {
        get {
            return data.maxHp;
        }

        set {
            data.maxHp = Mathf.Clamp(value, 0, 99);
            gameManager.hudController.SetMaxHP(data.maxHp);
        }
    }
    public int fp
    {
        get {
            return data.fp;
        }

        set {
            data.fp = Mathf.Clamp(value, 0, maxFp);
            gameManager.hudController.SetFP(data.fp);
        }
    }
    public int maxFp
    {
        get {
            return data.maxFp;
        }

        set {
            data.maxFp = Mathf.Clamp(value, 0, 99);
            gameManager.hudController.SetMaxFP(data.maxFp);
        }
    }
    public int sp
    {
        get {
            return data.sp;
        }

        set {
            data.sp = Mathf.Clamp(value, 0, maxSp);
        }
    }
    public int maxSp
    {
        get {
            return data.maxSp;
        }

        set {
            data.maxSp = Mathf.Clamp(value, 0, 99);
        }
    }
    public int coins
    {
        get {
            return data.coins;
        }

        set {
            data.coins = Mathf.Clamp(value, 0, 999);
            gameManager.hudController.SetCoins(data.coins);
        }
    }
    public int starPoints
    {
        get {
            return data.starPoints;
        }

        set {
            data.starPoints = Mathf.Clamp(value, 0, 99);
            gameManager.hudController.SetStarPoints(data.starPoints);
        }
    }
    public int level
    {
        get {
            return data.level;
        }

        set {
            data.level = Mathf.Clamp(value, 0, 99);
        }
    }
    public int bp
    {
        get {
            return data.bp;
        }

        set {
            data.bp = Mathf.Clamp(value, 0, 99);
        }
    }
    public int shineSprites
    {
        get {
            return data.shineSprites;
        }

        set {
            data.shineSprites = Mathf.Clamp(value, 0, 99);
        }
    }
    public int starPieces
    {
        get {
            return data.starPieces;
        }

        set {
            data.starPieces = Mathf.Clamp(value, 0, 99);
        }
    }
    public StarRank starRank
    {
        get {
            if(data.level < 10){
                return StarRank.RISING_STAR;
            }else if (data.level < 20){
                return StarRank.B_LIST_STAR;
            } else if (data.level < 30) {
                return StarRank.A_LIST_STAR;
            } else{
                return StarRank.SUPERSTAR;
            }
        }
    }
    public DateTime playtime
    {
        get {
            deltaPlaytime.Stop();
            data.playtime = data.playtime.AddTicks(deltaPlaytime.ElapsedTicks);
            deltaPlaytime.Reset();
            deltaPlaytime.Start();
            return data.playtime;
        }
    }

    public List<BaseItem> items
    {
        get {
            return data.items;
        }
    }
    public List<BaseItem> normalItems
    {
        get {
            if(items != null && items.Count > 0){
                return items.FindAll(IsNormal);
            }else{
                return new List<BaseItem>();
            }
        }
    }
    public List<BaseItem> importantItems
    {
        get {
            if (items != null && items.Count > 0) {
                return items.FindAll(IsImportant);
            } else {
                return new List<BaseItem>();
            }
        }
    }
    public List<BaseItem> badgeItems
    {
        get {
            if (items != null && items.Count > 0) {
                return items.FindAll(IsBadge);
            } else {
                return new List<BaseItem>();
            }
        }
    }
    public HammerAsset currentHammer{
        get{
            if(data.currentHammer == null){
                return defaultHammer;
            }else{
                return data.currentHammer;
            }
        }

        set{
            data.currentHammer = value;
        }
    }

    public List<StatusEffect> statusEffects;

    public Dictionary<string, object> shelf{
        get{ return data.shelf; }
    }

    //Save system stuff
    public bool LoadData() {
        try{
            using (StreamReader file = new StreamReader(Application.persistentDataPath + "/" + savefileName)){
                String dataString = file.ReadToEnd();
                data = Utils.Deserialize<PlayerData>(dataString);
                data.shelf = data.shelfSerialized.DeSerializeDict();
            }
            return true;
        } catch(Exception e){
            print("Failed to load data! " + e.ToString());
            return false;
        }
    }

    public bool SaveData(){
        try{
            using (StreamWriter file = new StreamWriter(Application.persistentDataPath + "/" + savefileName)){
                data.playtime = playtime;
                data.currentScene = SceneManager.GetActiveScene().name;
                data.currentPosition = FindObjectOfType<PlayerMachine>().transform.position + new Vector3(0,-0.5f,0);
                data.shelfSerialized = data.shelf.SerializeDict();
                file.WriteLine(Utils.Serialize(data));
            }
            return true;
        }catch(Exception e){
            print("Failed to save data! " + e.ToString());
            return false;
        }
    }

    //Sorting funtions
    private static bool IsImportant(BaseItem item) {
        if (item is UsableItem && ((UsableItem)item).isImportantItem) {
            return true;
        } else {
            return false;
        }
    }
    private static bool IsNormal(BaseItem item) {
        if (item is UsableItem && !((UsableItem)item).isImportantItem) {
            
            return true;
        } else {
            return false;
        }
    }
    private static bool IsBadge(BaseItem item) {
        if (item is BadgeItem) {
            return true;
        } else {
            return false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Backpack))]
    public class BackpackEditor : Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (GUILayout.Button("Reset save data")) {
                try {
                    using (StreamWriter file = new StreamWriter(Application.persistentDataPath + "/" + ((Backpack)target).savefileName)){
                        PlayerData data = new PlayerData().GetDefaults();
                        file.WriteLine(Utils.Serialize(data));
                    }
                } catch (Exception e) {
                    print("Failed to reset data! " + e.ToString());
                }
            }
        }
    }
#endif

}

public class PlayerData{
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
    public DateTime playtime;
    public string currentScene;
    public Vector3 currentPosition;
    public List<BaseItem> items;
    public HammerAsset currentHammer;
    public Dictionary<string, object> shelf;
    public byte[] shelfSerialized;

    public PlayerData GetDefaults(){
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
        currentHammer = null;
        shelf = new Dictionary<string, object>();
        return this;
    }
}

[Serializable]
public enum StatusEffect{
    POISONED, ELECRIFICATED, CHARGED, COMMAND_LOSS, DIZZY, FROZEN, BIG, STOPWATCH, AGRESSIVE
}

[Serializable]
public enum GameMode{
    OVERWORLD, BATTLE, UNKNOWN
}