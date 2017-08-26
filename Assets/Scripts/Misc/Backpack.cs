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
            hudController.setHP(_maxHp);
        }
    }
    public int fp
    {
        get {
            return _fp;
        }

        set {
            _fp = value;
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
        _hp = PlayerPrefs.GetInt("hp", 10);
        _maxHp = PlayerPrefs.GetInt("maxHp", 10);
        _fp = PlayerPrefs.GetInt("fp", 5);
        _maxFp = PlayerPrefs.GetInt("maxFp", 5);
        _coins = PlayerPrefs.GetInt("coins", 0);
        _starPoints = PlayerPrefs.GetInt("starPoints", 0);
        _level = PlayerPrefs.GetInt("level", 1);
        _bp = PlayerPrefs.GetInt("bp", 1);
        _progress = PlayerPrefs.GetString("progress", startSceneName+",0");
    }

}
