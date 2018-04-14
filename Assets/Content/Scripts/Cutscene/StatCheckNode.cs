public class StatCheckNode : ConditionBaseNode
{

    public Stat statToCheck;
    public Operator operatorToCheckWith;
    public int valueToCheckFor;

    public enum Stat { HP, FP, BP, COINS, STAR_PIECES, SHINE_SPRITES, LEVEL, STAR_POINTS }
    public enum Operator { GREATER, EQUALS, LESS }

    public override bool Condition() {
        return CheckForStat(statToCheck, operatorToCheckWith);
    }

    private bool CheckForStat(Stat stat, Operator op){
        int statValue = GetValueForStat(stat);

        switch (op) {
            case Operator.GREATER:
                if (statValue > valueToCheckFor)
                    return true;
                break;
            case Operator.EQUALS:
                if (statValue == valueToCheckFor)
                    return true;
                break;
            case Operator.LESS:
                if (statValue < valueToCheckFor)
                    return true;
                break;
        }

        return false;
    }

    private int GetValueForStat(Stat stat){
        Backpack backpack = cutsceneManager.gameManager.GetBackpack();

        switch (stat) {
            case Stat.HP:
                return backpack.legacyHP;
                break;
            case Stat.FP:
                return backpack.legacyFP;
                break;
            case Stat.BP:
                return backpack.legacyBP;
                break;
            case Stat.COINS:
                return backpack.legacyCoins;
                break;
            case Stat.STAR_PIECES:
                return backpack.legacyStarPieces;
                break;
            case Stat.SHINE_SPRITES:
                return backpack.legacyShineSprites;
                break;
            case Stat.LEVEL:
                return backpack.legacyLevel;
                break;
            case Stat.STAR_POINTS:
                return backpack.legacyStarPoints;
                break;
        }

        return 0;
    }
}
