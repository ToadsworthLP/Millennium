using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Backpack : MonoBehaviour {

    public HUDController hudController;
    public string startSceneName;
    public string savefileName;

    private PlayerData data;
    private Stopwatch deltaPlaytime;

    void Start() {
        DontDestroyOnLoad(gameObject);
        loadData();
        deltaPlaytime = Stopwatch.StartNew();
        StartCoroutine(initializeHUD());
    }

    IEnumerator initializeHUD(){
        yield return new WaitForEndOfFrame();
        hudController.setData(data);
    }

    //Getters and setters
    public int hp
    {
        get {
            return data.hp;
        }

        set {
            data.hp = Mathf.Clamp(value, 0, maxHp);
            hudController.setHP(data.hp);
        }
    }
    public int maxHp
    {
        get {
            return data.maxHp;
        }

        set {
            data.maxHp = Mathf.Clamp(value, 0, 99);
            hudController.setMaxHP(data.maxHp);
        }
    }
    public int fp
    {
        get {
            return data.fp;
        }

        set {
            data.fp = Mathf.Clamp(value, 0, maxFp);
            hudController.setFP(data.fp);
        }
    }
    public int maxFp
    {
        get {
            return data.maxFp;
        }

        set {
            data.maxFp = Mathf.Clamp(value, 0, 99);
            hudController.setMaxFP(data.maxFp);
        }
    }
    public int coins
    {
        get {
            return data.coins;
        }

        set {
            data.coins = Mathf.Clamp(value, 0, 999);
            hudController.setCoins(data.coins);
        }
    }
    public int starPoints
    {
        get {
            return data.starPoints;
        }

        set {
            data.starPoints = Mathf.Clamp(value, 0, 99);
            hudController.setStarPoints(data.starPoints);
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

    public string progress
    {
        get {
            return data.progress;
        }

        set {
            data.progress = value;
        }
    }

    //Save system stuff
    public bool loadData() {
        if (File.Exists(Application.persistentDataPath + "/"+ savefileName)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + savefileName, FileMode.Open);

            data = (PlayerData)formatter.Deserialize(file);
            file.Close();

            if (data == null){
                data = new PlayerData().getDefaults();
                return false;
            }

            return true;
        }else{
            data = new PlayerData().getDefaults();
            return false;
        }
    }

    public bool saveData(){
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath+ "/" + savefileName, FileMode.OpenOrCreate);

        data.playtime = playtime;

        formatter.Serialize(file, data);
        file.Close();

        return true;
    }

    public bool deleteData(){
        if (File.Exists(Application.persistentDataPath + "/" + savefileName)) {
            File.Delete(Application.persistentDataPath + "/" + savefileName);
            data = new PlayerData().getDefaults();
            return true;
        }else{
            return false;
        }
    }

}

[Serializable]
public class PlayerData{
    public int maxHp;
    public int hp;
    public int maxFp;
    public int fp;
    public int coins;
    public int starPoints;
    public int level;
    public int bp;
    public int shineSprites;
    public int starPieces;
    public DateTime playtime;
    public string progress;

    public PlayerData getDefaults(){
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
        progress = "";
        return this;
    }
}
