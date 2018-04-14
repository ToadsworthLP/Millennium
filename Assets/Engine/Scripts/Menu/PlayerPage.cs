using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPage : MonoBehaviour {

    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI rankText;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI fpText;
    public TextMeshProUGUI bpText;
    public TextMeshProUGUI advancedNumberText;

    public Sprite noHammerSprite;
    public Image hammerIcon;

    public GameManager gameManager;
    private Backpack backpack;

    void Awake() {
        backpack = gameManager.GetBackpack();
    }

    void OnEnable() {
        DateTime playtime = backpack.GetPlaytime();

        playerNameText.text = backpack.playerName;
        levelText.text = "Lvl.  " + backpack.level;
        rankText.text = backpack.starRank.GetName();

        hpText.text = backpack.HP + " / " + backpack.maxHP;
        fpText.text = backpack.FP + " / " + backpack.maxFP;
        bpText.text = backpack.BP.ToString();

        advancedNumberText.text = backpack.legacyStarPoints + Utils.NewLine() +
            backpack.coins + Utils.NewLine() +
            backpack.starPieces + Utils.NewLine() +
            backpack.shineSprites + Utils.NewLine() +
            playtime.Hour + " : " + playtime.Minute;

        if(backpack.currentHammer == null){
            hammerIcon.sprite = noHammerSprite;
        } else{
            hammerIcon.sprite = backpack.currentHammer.hammerIcon;
        }

        gameManager.GetPlayerRenderTexture();
    }

    void OnDisable() {
        gameManager.ReleaseRenderTexture();
    }

}
