using System;
using UnityEngine;
using UnityEngine.UI;

public class FancyNumberHandler : MonoBehaviour {

    public ImageToNumber first;
    public ImageToNumber second;
    public ImageToNumber third;

    void Start() {
        UpdateValue(0);
    }

    //This is so lame, copied from SM64 HD remake thing because I'm lazy
    public void UpdateValue(int value) {
        if(third != null){
            value = Mathf.Clamp(value, 0, 999);
        }else{
            value = Mathf.Clamp(value, 0, 99);
        }

        var stringValue = value.ToString();

        if (stringValue.Length == 1) {
            first.GetComponent<Image>().enabled = true;
            second.GetComponent<Image>().enabled = false;

            first.SetValue((int)Char.GetNumericValue(stringValue[0]));

            //This is uglier than everything on Pornhub together
            if (third != null){third.GetComponent<Image>().enabled = false; }

        } else if (stringValue.Length == 2) {
            first.GetComponent<Image>().enabled = true;
            second.GetComponent<Image>().enabled = true;

            first.SetValue((int)Char.GetNumericValue(stringValue[0]));
            second.SetValue((int)Char.GetNumericValue(stringValue[1]));

            //Uh...again
            if (third != null){third.GetComponent<Image>().enabled = false; }

        } else if (third != null) {
            first.GetComponent<Image>().enabled = true;
            second.GetComponent<Image>().enabled = true;
            third.GetComponent<Image>().enabled = true;

            first.SetValue((int)Char.GetNumericValue(stringValue[0]));
            second.SetValue((int)Char.GetNumericValue(stringValue[1]));
            third.SetValue((int)Char.GetNumericValue(stringValue[2]));
        }
    }
}
