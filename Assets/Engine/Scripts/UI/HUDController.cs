using UnityEngine;

public class HUDController : MonoBehaviour {

    public FancyNumberHandler hp;
    public FancyNumberHandler fp;
    public FancyNumberHandler maxHp;
    public FancyNumberHandler maxFp;
    public FancyNumberHandler coins;
    public FancyNumberHandler starPoints;

    public void setData(PlayerData data, bool suppressAnimation = false){
        setHP(data.hp, suppressAnimation);
        setMaxHP(data.maxHp, suppressAnimation);
        setFP(data.fp, suppressAnimation);
        setMaxFP(data.fp, suppressAnimation);
        setCoins(data.coins, suppressAnimation);
        setStarPoints(data.starPoints, suppressAnimation);
    }

    public void setHP(int amount, bool suppressAnimation = false) {
        hp.UpdateValue(amount);
        if(!suppressAnimation)
            hp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setMaxHP(int amount, bool suppressAnimation = false) {
        maxHp.UpdateValue(amount);
        if (!suppressAnimation)
            maxHp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setFP(int amount, bool suppressAnimation = false) {
        fp.UpdateValue(amount);
        if (!suppressAnimation)
            fp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setMaxFP(int amount, bool suppressAnimation = false) {
        maxFp.UpdateValue(amount);
        if (!suppressAnimation)
            maxFp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setCoins(int amount, bool suppressAnimation = false) {
        coins.UpdateValue(amount);
        if (!suppressAnimation)
            coins.GetComponentInParent<Animator>().SetTrigger("CoinsUpdated");
    }

    public void setStarPoints(int amount, bool suppressAnimation = false) {
        starPoints.UpdateValue(amount);
        if (!suppressAnimation)
            starPoints.GetComponentInParent<Animator>().SetTrigger("StarPointsUpdated");
    }
}
