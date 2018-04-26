using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Backpack : MonoBehaviour {

    private GameManager gameManager;
    public SaveManager saveManager;
    public string startSceneName;
    public Vector3 startPosition;
    [HideInInspector]
    public int targetEntranceId;

    [Header("Container References")]
    public StringReference playerName;
    public IntReference HP;
    public IntReference maxHP;
    public IntReference FP;
    public IntReference maxFP;
    public IntReference SP;
    public IntReference maxSP;
    public IntReference BP;
    public IntReference starPoints;
    public IntReference level;
    public IntReference coins;
    public IntReference shineSprites;
    public IntReference starPieces;
    public ItemListReference inventory;

    private Stopwatch deltaPlaytime;

    private void Start() {
        if (GameObject.FindGameObjectsWithTag("Backpack").Length > 1){
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        SaveManager.SaveData data = saveManager.LoadGame();
        HP.Value = data.hp;
        maxHP.Value = data.maxHp;
        FP.Value = data.fp;
        maxFP.Value = data.maxFp;
        SP.Value = data.sp;
        maxSP.Value = data.maxSp;
        BP.Value = data.bp;
        starPoints.Value = data.starPoints;
        level.Value = data.level;
        coins.Value = data.coins;
        shineSprites.Value = data.shineSprites;
        starPieces.Value = data.starPieces;
        inventory.Value = data.items;
        playtime = data.playtime;
        shelf = data.shelf;

        SceneManager.sceneLoaded += SceneLoaded;

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        gameManager.UpdateHammer(currentHammer);
        gameManager.blackOverlay.FadeOut();

        PreparePlayer(data);

        deltaPlaytime = Stopwatch.StartNew();

        StartCoroutine(AfterFirstFrame());
    }

    public void SceneLoaded(Scene scene, LoadSceneMode mode) {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        gameManager.UpdateHammer(currentHammer);

        if(gameManager.sceneEntrances.Length-1 <= targetEntranceId){
            gameManager.sceneEntrances[targetEntranceId].PlayerArrives();
        }

        StartCoroutine(AfterFirstFrame());
    }

    private void PreparePlayer(SaveManager.SaveData data){
        if (data.currentPosition == Vector3.zero || SceneManager.GetActiveScene().name != data.currentScene) {
            gameManager.playerMachine.transform.position = startPosition;
        } else {
            gameManager.playerMachine.transform.position = data.currentPosition;
        }

        gameManager.playerMachine.hammer.UpdateHammer(data.currentHammer);
    }

    private IEnumerator AfterFirstFrame() {
        yield return new WaitForEndOfFrame();

        //Manually calls update on all relevant containers to fix problem where update listeners are
        //called before they are registered

        HP.ForceUpdate();
        maxHP.ForceUpdate();
        FP.ForceUpdate();
        maxFP.ForceUpdate();
        SP.ForceUpdate();
        maxSP.ForceUpdate();
        BP.ForceUpdate();
        starPoints.ForceUpdate();
        level.ForceUpdate();
        coins.ForceUpdate();
        shineSprites.ForceUpdate();
        starPieces.ForceUpdate();
    }

    //Getters and setters
    public string legacyPlayerName
    {
        get {
            return playerName.Value;
        }

        set {
            playerName.Value = value;
        }
    }
    public int legacyHP
    {
        get {
            return HP.Value;
        }

        set {
            HP.Value = value;
        }
    }
    public int legacyMaxHP
    {
        get {
            return maxHP;
        }

        set {
            maxHP.Value = value;
        }
    }
    public int legacyFP
    {
        get {
            return FP;
        }

        set {
            FP.Value = value;
        }
    }
    public int legacyMaxFP
    {
        get {
            return maxFP;
        }

        set {
            maxFP.Value = value;
        }
    }
    public int legacySP
    {
        get {
            return SP;
        }

        set {
            maxSP.Value = value;
        }
    }
    public int legacyMaxSP
    {
        get {
            return maxSP;
        }

        set {
            maxSP.Value = value;
        }
    }
    public int legacyCoins
    {
        get {
            return coins;
        }

        set {
            coins.Value = value;
        }
    }
    public int legacyStarPoints
    {
        get {
            return starPoints;
        }

        set {
            starPoints.Value = value;
        }
    }
    public int legacyLevel
    {
        get {
            return level;
        }

        set {
            level.Value = value;
        }
    }
    public int legacyBP
    {
        get {
            return BP;
        }

        set {
            BP.Value = value;
        }
    }
    public int legacyShineSprites
    {
        get {
            return shineSprites;
        }

        set {
            shineSprites.Value = value;
        }
    }
    public int legacyStarPieces
    {
        get {
            return starPieces;
        }

        set {
            starPieces.Value = value;
        }
    }
    public StarRank starRank
    {
        get {
            if(level < 10){
                return StarRank.RISING_STAR;
            }else if (level < 20){
                return StarRank.B_LIST_STAR;
            } else if (level < 30) {
                return StarRank.A_LIST_STAR;
            } else{
                return StarRank.SUPERSTAR;
            }
        }
    }

    private DateTime playtime;
    public DateTime GetPlaytime(){
        deltaPlaytime.Stop();
        playtime = playtime.AddTicks(deltaPlaytime.ElapsedTicks);
        deltaPlaytime.Reset();
        deltaPlaytime.Start();
        return playtime;
    }

    public List<BaseItem> items
    {
        get {
            return inventory.Value;
        }
    }
    public List<BaseItem> normalItems
    {
        get {
            if(inventory.Value != null && inventory.Value.Count > 0){
                return inventory.Value.FindAll(IsNormal);
            }else{
                return new List<BaseItem>();
            }
        }
    }
    public List<BaseItem> importantItems
    {
        get {
            if (inventory.Value != null && inventory.Value.Count > 0) {
                return inventory.Value.FindAll(IsImportant);
            } else {
                return new List<BaseItem>();
            }
        }
    }
    public List<BaseItem> badgeItems
    {
        get {
            if (inventory.Value != null && inventory.Value.Count > 0) {
                return inventory.Value.FindAll(IsBadge);
            } else {
                return new List<BaseItem>();
            }
        }
    }
    public HammerAsset currentHammer{
        get{
            HammerAsset currentHammer = gameManager.playerMachine.hammer.hammer;
            if(currentHammer == null){
                return saveManager.defaultHammer;
            }else{
                return currentHammer;
            }
        }

        set{
            Hammer hammer = gameManager.playerMachine.hammer;
            hammer.UpdateHammer(value);
        }
    }

    public List<StatusEffect> statusEffects;
    public Dictionary<string, object> shelf;

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

}

[Serializable]
public enum StatusEffect{
    POISONED, ELECRIFICATED, CHARGED, COMMAND_LOSS, DIZZY, FROZEN, BIG, STOPWATCH, AGRESSIVE
}

[Serializable]
public enum GameMode{
    OVERWORLD, BATTLE, UNKNOWN
}