using UnityEngine;
using UnityEngine.UI;

public class TextWithShadow : MonoBehaviour {

    private Text[] texts;

    public void updateText(string text){
        if(texts == null){
            texts = GetComponentsInChildren<Text>();
        }
        foreach (Text t in texts){
            t.text = text;
        }
    }

}
