using Millennium.Containers;
using SavePort.Saving;
using SavePort.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Backpack : MonoBehaviour {

    public static Backpack instance;

    private GameManager gameManager;
    public string startSceneName;
    [HideInInspector]
    public int targetEntranceId;

    [Header("Container References")]
    public StringReference playerName;
    public IntReference HP;
    public IntReference maxHP;
    public IntReference FP;
    public IntReference maxFP;
    public FloatReference SP;
    public FloatReference maxSP;
    public IntReference BP;
    public IntReference starPoints;
    public IntReference level;
    public IntReference coins;
    public IntReference shineSprites;
    public IntReference starPieces;
    public ItemListReference inventory;
    public HammerAssetReference hammer;
    public Vector3Reference playerSpawnPosition;
    public StringReference playerSpawnScene;
    public DateTimeReference playtime;

    private Stopwatch deltaPlaytime;

    private void Start() {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        SaveManager.LoadContainers("save.spdat");

        SceneManager.sceneLoaded += SceneLoaded;

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        gameManager.blackOverlay.FadeOut();

        PreparePlayer();

        deltaPlaytime = Stopwatch.StartNew();
    }

    public void SceneLoaded(Scene scene, LoadSceneMode mode) {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        StartCoroutine(DelayedContainerForcedUpdate());

        if(targetEntranceId < gameManager.sceneEntrances.Length)
        {
            gameManager.sceneEntrances[targetEntranceId].PlayerArrives();
        }
    }

    private void PreparePlayer(){
        if (playerSpawnPosition == Vector3.zero || SceneManager.GetActiveScene().name != playerSpawnScene) {
            gameManager.playerMachine.transform.position = transform.position;
        } else {
            gameManager.playerMachine.transform.position = playerSpawnPosition;
        }
    }

    IEnumerator DelayedContainerForcedUpdate()
    {
        yield return new WaitForEndOfFrame();
        SaveManager.ForceAllContainerUpdateEvents();
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
    public float legacySP
    {
        get {
            return SP;
        }

        set {
            maxSP.Value = value;
        }
    }
    public float legacyMaxSP
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

    public DateTime GetPlaytime(){
        deltaPlaytime.Stop();
        playtime.Value = playtime.Value.AddTicks(deltaPlaytime.ElapsedTicks);
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