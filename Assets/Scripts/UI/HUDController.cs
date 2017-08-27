using System.Collections;
using UnityEngine;

public class HUDController : MonoBehaviour {

    public FancyNumberHandler hp;
    public FancyNumberHandler fp;
    public FancyNumberHandler maxHp;
    public FancyNumberHandler maxFp;
    public FancyNumberHandler coins;
    public FancyNumberHandler starPoints;

    public void setData(PlayerData data){
        setHP(data.hp);
        setMaxHP(data.maxHp);
        setFP(data.fp);
        setMaxFP(data.fp);
        setCoins(data.coins);
        setStarPoints(data.starPoints);
    }

    public void setHP(int amount) {
        hp.UpdateValue(amount);
        hp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setMaxHP(int amount) {
        maxHp.UpdateValue(amount);
        maxHp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setFP(int amount) {
        fp.UpdateValue(amount);
        fp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setMaxFP(int amount) {
        maxFp.UpdateValue(amount);
        maxFp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setCoins(int amount) {
        coins.UpdateValue(amount);
        coins.GetComponentInParent<Animator>().SetTrigger("CoinsUpdated");
    }

    public void setStarPoints(int amount) {
        starPoints.UpdateValue(amount);
        starPoints.GetComponentInParent<Animator>().SetTrigger("StarPointsUpdated");
    }
}
