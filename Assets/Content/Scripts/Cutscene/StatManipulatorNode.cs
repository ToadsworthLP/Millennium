using System;

public class StatManipulatorNode : BaseCutsceneNode {

    public Stat statToModify;
    public int quantity;

    public enum Stat { HP, FP, BP, COINS, STAR_PIECES, SHINE_SPRITES, LEVEL, STAR_POINTS }

    public override void CallNode() {
        Backpack backpack = cutsceneManager.gameManager.GetBackpack();

        switch (statToModify) {
            case Stat.HP:
                backpack.hp += quantity;
                break;
            case Stat.FP:
                backpack.fp += quantity;
                break;
            case Stat.BP:
                backpack.bp += quantity;
                break;
            case Stat.COINS:
                backpack.coins += quantity;
                break;
            case Stat.STAR_PIECES:
                backpack.starPieces += quantity;
                break;
            case Stat.SHINE_SPRITES:
                backpack.shineSprites += quantity;
                break;
            case Stat.LEVEL:
                backpack.level += quantity;
                break;
            case Stat.STAR_POINTS:
                backpack.starPoints += quantity;
                break;
        }

        CallOutputSlot("Next Node");
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

}
