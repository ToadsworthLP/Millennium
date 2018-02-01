using UnityEngine;

public class HUDController : MonoBehaviour {

    public FancyNumberHandler hp;
    public FancyNumberHandler fp;
    public FancyNumberHandler maxHp;
    public FancyNumberHandler maxFp;
    public FancyNumberHandler coins;
    public FancyNumberHandler starPoints;

    public void SetData(PlayerData data, bool suppressAnimation = false){
        SetHP(data.hp, suppressAnimation);
        SetMaxHP(data.maxHp, suppressAnimation);
        SetFP(data.fp, suppressAnimation);
        SetMaxFP(data.fp, suppressAnimation);
        SetCoins(data.coins, suppressAnimation);
        SetStarPoints(data.starPoints, suppressAnimation);
    }

    public void SetHP(int amount, bool suppressAnimation = false) {
        hp.UpdateValue(amount);
        if(!suppressAnimation)
            hp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void SetMaxHP(int amount, bool suppressAnimation = false) {
        maxHp.UpdateValue(amount);
        if (!suppressAnimation)
            maxHp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void SetFP(int amount, bool suppressAnimation = false) {
        fp.UpdateValue(amount);
        if (!suppressAnimation)
            fp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void SetMaxFP(int amount, bool suppressAnimation = false) {
        maxFp.UpdateValue(amount);
        if (!suppressAnimation)
            maxFp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void SetCoins(int amount, bool suppressAnimation = false) {
        coins.UpdateValue(amount);
        if (!suppressAnimation)
            coins.GetComponentInParent<Animator>().SetTrigger("CoinsUpdated");
    }

    public void SetStarPoints(int amount, bool suppressAnimation = false) {
        starPoints.UpdateValue(amount);
        if (!suppressAnimation)
            starPoints.GetComponentInParent<Animator>().SetTrigger("StarPointsUpdated");
    }
}
