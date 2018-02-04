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
                return backpack.hp;
                break;
            case Stat.FP:
                return backpack.fp;
                break;
            case Stat.BP:
                return backpack.bp;
                break;
            case Stat.COINS:
                return backpack.coins;
                break;
            case Stat.STAR_PIECES:
                return backpack.starPieces;
                break;
            case Stat.SHINE_SPRITES:
                return backpack.shineSprites;
                break;
            case Stat.LEVEL:
                return backpack.level;
                break;
            case Stat.STAR_POINTS:
                return backpack.starPoints;
                break;
        }

        return 0;
    }
}
