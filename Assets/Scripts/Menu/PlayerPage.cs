using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPage : MonoBehaviour {

    public Text hpText;
    public Text fpText;
    public Text bpText;

    public Text advancedNumberText;

    private Backpack backpack;

    void Awake() {
        backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
    }

    void OnEnable() {
        DateTime playtime = backpack.playtime;

        hpText.text = backpack.hp + " / " + backpack.maxHp;
        fpText.text = backpack.fp + " / " + backpack.maxFp;
        bpText.text = backpack.bp.ToString();

        advancedNumberText.text = backpack.starPoints + Utils.newLine() +
            backpack.coins + Utils.newLine() +
            backpack.starPieces + Utils.newLine() +
            backpack.shineSprites + Utils.newLine() +
            playtime.Hour + " : " + playtime.Minute;
    }

}
