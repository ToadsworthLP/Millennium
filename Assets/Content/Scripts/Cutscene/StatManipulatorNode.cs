using System;

public class StatManipulatorNode : BaseCutsceneNode {

    public Stat statToModify;
    public int quantity;

    public enum Stat { HP, FP, BP, COINS, STAR_PIECES, SHINE_SPRITES, LEVEL, STAR_POINTS }

    public override void CallNode() {
        Backpack backpack = cutsceneManager.gameManager.GetBackpack();

        switch (statToModify) {
            case Stat.HP:
                backpack.legacyHP += quantity;
                break;
            case Stat.FP:
                backpack.legacyFP += quantity;
                break;
            case Stat.BP:
                backpack.legacyBP += quantity;
                break;
            case Stat.COINS:
                backpack.legacyCoins += quantity;
                break;
            case Stat.STAR_PIECES:
                backpack.legacyStarPieces += quantity;
                break;
            case Stat.SHINE_SPRITES:
                backpack.legacyShineSprites += quantity;
                break;
            case Stat.LEVEL:
                backpack.legacyLevel += quantity;
                break;
            case Stat.STAR_POINTS:
                backpack.legacyStarPoints += quantity;
                break;
        }

        CallOutputSlot("Next Node");
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

}
