using System.Collections;
using UnityEngine;

public class HUDController : MonoBehaviour {

    public FancyNumberHandler hp;
    public FancyNumberHandler fp;
    public FancyNumberHandler maxHp;
    public FancyNumberHandler maxFp;
    public FancyNumberHandler coins;
    public FancyNumberHandler starPoints;

    private int hpValue;
    private int fpValue;
    private int maxHpValue;
    private int maxFpValue;
    private int coinsValue;
    private int starPointsValue;

    //Used to test the HUD functions
    IEnumerator test(){
        yield return new WaitForSeconds(1f);
        setMaxHP(10);
        setMaxFP(5);
        while (true){
            yield return new WaitForSeconds(1f);
            setHP(5);
            yield return new WaitForSeconds(1f);
            setHP(0);
            yield return new WaitForSeconds(1f);
            setFP(5);
            yield return new WaitForSeconds(1f);
            setFP(0);
            yield return new WaitForSeconds(1f);
            setCoins(123);
            yield return new WaitForSeconds(1f);
            setCoins(0);
            yield return new WaitForSeconds(1f);
            setStarPoints(5);
            yield return new WaitForSeconds(1f);
            setStarPoints(0);
            yield return new WaitForSeconds(1f);
        }
    }

    public void setHP(int amount) {
        hpValue = Mathf.Clamp(amount, 0, maxHpValue);
        hp.UpdateValue(hpValue);
        hp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setMaxHP(int amount) {
        maxHpValue = amount;
        maxHp.UpdateValue(maxHpValue);
        maxHp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setFP(int amount) {
        fpValue = Mathf.Clamp(amount, 0, maxFpValue);
        fp.UpdateValue(fpValue);
        fp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setMaxFP(int amount) {
        maxFpValue = amount;
        maxFp.UpdateValue(maxFpValue);
        maxFp.GetComponentInParent<Animator>().SetTrigger("Updated");
    }

    public void setCoins(int amount) {
        coinsValue = amount;
        coins.UpdateValue(coinsValue);
        coins.GetComponentInParent<Animator>().SetTrigger("CoinsUpdated");
    }

    public void setStarPoints(int amount) {
        starPointsValue = amount;
        starPoints.UpdateValue(starPointsValue);
        starPoints.GetComponentInParent<Animator>().SetTrigger("StarPointsUpdated");
    }
}
