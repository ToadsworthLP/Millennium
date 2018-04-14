using System;
using UnityEngine;
using UnityEngine.UI;

public class BPDotDisplay : MonoBehaviour {

    public int displayedValue;
    public int displayedMaxValue;
    public BPDotRow[] dotRows;
    public TMPro.TextMeshProUGUI moreText;

    [Serializable]
    public struct BPDotRow{
        public RectTransform fullRowTransform;
        public RectTransform emptyRowTransform;
    }

    private void Update() {
        UpdateDisplay(displayedValue, displayedMaxValue);
    }

    public void UpdateDisplay(int value, int maxValue){
        if (value > maxValue){
            Debug.LogError("The maximum value was set greater than the actual value! This is not allowed!");
            return;
        }

        int emptyRowsShown = Mathf.FloorToInt(maxValue / 10);
        int fullRowsShown = Mathf.FloorToInt(value / 10);

        int additionalEmptyDots = maxValue - emptyRowsShown * 10;
        int additionalFullDots = value - fullRowsShown * 10;

        GameObject moreTextGameObject = moreText.gameObject;
        if (emptyRowsShown >= dotRows.Length) {
            int showableMaximum = dotRows.Length * 10;

            if(!moreTextGameObject.activeInHierarchy)
                moreTextGameObject.SetActive(true);

            if(value > showableMaximum){
                moreText.text = "..." + (value - showableMaximum) + " of " + (maxValue - showableMaximum) + " more used";
                fullRowsShown = showableMaximum;
                additionalFullDots = 0;
            } else {
                moreText.text = "..." + (maxValue - showableMaximum) + " more left";
            }

            emptyRowsShown = showableMaximum;
            additionalEmptyDots = 0;
        }else if(moreTextGameObject.activeInHierarchy){
            moreTextGameObject.SetActive(false);
        }

        for (int i = 0; i < dotRows.Length; i++) {
            if(i < emptyRowsShown){
                dotRows[i].emptyRowTransform.sizeDelta = new Vector2(640, 64);
            }else{
                dotRows[i].emptyRowTransform.sizeDelta = Vector2.zero;
            }

            if (i < fullRowsShown) {
                dotRows[i].fullRowTransform.sizeDelta = new Vector2(640, 64);
            } else {
                dotRows[i].fullRowTransform.sizeDelta = Vector2.zero;
            }
        }

        if(additionalEmptyDots > 0)
            dotRows[emptyRowsShown].emptyRowTransform.sizeDelta = new Vector2(64 * additionalEmptyDots, 64);

        if(additionalFullDots > 0)
            dotRows[fullRowsShown].fullRowTransform.sizeDelta = new Vector2(64 * additionalFullDots, 64);
    }

}
