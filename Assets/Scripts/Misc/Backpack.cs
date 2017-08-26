using System.Collections;
using UnityEngine;

public class Backpack : MonoBehaviour {

    public HUDController hudController;
    public string startSceneName;

    private int _hp;
    private int _maxHp;
    private int _fp;
    private int _maxFp;
    private int _coins;
    private int _starPoints;
    private int _level;
    private int _bp;
    private string _progress;

    void Start() {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(initializeHUD());
    }

    IEnumerator initializeHUD(){
        yield return new WaitForEndOfFrame();
        loadData();
    }

    //Getters and setters
    public int hp
    {
        get {
            return _hp;
        }

        set {
            _hp = Mathf.Clamp(value, 0, maxHp);
            hudController.setHP(_hp);
        }
    }
    public int maxHp
    {
        get {
            return _maxHp;
        }

        set {
            _maxHp = Mathf.Clamp(value, 0, 99);
            hudController.setMaxHP(_maxHp);
        }
    }
    public int fp
    {
        get {
            return _fp;
        }

        set {
            _fp = Mathf.Clamp(value, 0, maxFp);
            hudController.setFP(_fp);
        }
    }
    public int maxFp
    {
        get {
            return _maxFp;
        }

        set {
            _maxFp = Mathf.Clamp(value, 0, 99);
            hudController.setMaxFP(_maxFp);
        }
    }
    public int coins
    {
        get {
            return _coins;
        }

        set {
            _coins = Mathf.Clamp(value, 0, 999);
            hudController.setCoins(_coins);
        }
    }
    public int starPoints
    {
        get {
            return _starPoints;
        }

        set {
            _starPoints = Mathf.Clamp(value, 0, 99);
            hudController.setStarPoints(_starPoints);
        }
    }
    public int level
    {
        get {
            return _level;
        }

        set {
            _level = Mathf.Clamp(value, 0, 99);
        }
    }
    public int bp
    {
        get {
            return _bp;
        }

        set {
            _bp = Mathf.Clamp(value, 0, 99);
        }
    }
    public string progress
    {
        get {
            return _progress;
        }

        set {
            _progress = value;
        }
    }

    //Save system stuff
    public void saveData(){
        PlayerPrefs.SetInt("hp",_hp);
        PlayerPrefs.SetInt("maxHp", _maxHp);
        PlayerPrefs.SetInt("fp", _fp);
        PlayerPrefs.SetInt("maxFp", _maxFp);
        PlayerPrefs.SetInt("coins", _coins);
        PlayerPrefs.SetInt("starPoints", _starPoints);
        PlayerPrefs.SetInt("level", _level);
        PlayerPrefs.SetInt("bp", _bp);
        PlayerPrefs.SetString("progress", _progress);
        PlayerPrefs.Save();
    }

    public void loadData() {
        hp = PlayerPrefs.GetInt("hp", 5);
        maxHp = PlayerPrefs.GetInt("maxHp", 10);
        fp = PlayerPrefs.GetInt("fp", 0);
        maxFp = PlayerPrefs.GetInt("maxFp", 5);
        coins = PlayerPrefs.GetInt("coins", 0);
        starPoints = PlayerPrefs.GetInt("starPoints", 0);
        level = PlayerPrefs.GetInt("level", 1);
        bp = PlayerPrefs.GetInt("bp", 1);
        progress = PlayerPrefs.GetString("progress", startSceneName+",0");
    }

}
