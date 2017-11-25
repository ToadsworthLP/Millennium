using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPage : MonoBehaviour {

    public TextWithShadow playerNameText;
    public TextWithShadow levelText;
    public Text rankText;

    public Text hpText;
    public Text fpText;
    public Text bpText;
    public Text advancedNumberText;

    public Image hammerIcon;

    public GameManager gameManager;
    private Backpack backpack;

    void Awake() {
        backpack = gameManager.getBackpack();
    }

    void OnEnable() {
        DateTime playtime = backpack.playtime;

        playerNameText.updateText(backpack.playerName);
        levelText.updateText("Lvl.  " + backpack.level);
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
