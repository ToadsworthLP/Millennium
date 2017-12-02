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

    public Image hammerIcon;

    public GameManager gameManager;
    private Backpack backpack;

    void Awake() {
        backpack = gameManager.getBackpack();
    }

    void OnEnable() {
        DateTime playtime = backpack.playtime;

        playerNameText.text = backpack.playerName;
        levelText.text = "Lvl.  " + backpack.level;
        rankText.text = backpack.starRank.getName();

        hpText.text = backpack.hp + " / " + backpack.maxHp;
        fpText.text = backpack.fp + " / " + backpack.maxFp;
        bpText.text = backpack.bp.ToString();

        advancedNumberText.text = backpack.starPoints + Utils.newLine() +
            backpack.coins + Utils.newLine() +
            backpack.starPieces + Utils.newLine() +
            backpack.shineSprites + Utils.newLine() +
            playtime.Hour + " : " + playtime.Minute;

        hammerIcon.sprite = backpack.currentHammer.hammerIcon;
        gameManager.getPlayerRenderTexture();
    }

    void OnDisable() {
        gameManager.releaseRenderTexture();
    }

}
