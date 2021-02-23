using System;
using UnityEngine;
using UnityEngine.UI;

public class FancyNumberHandler : MonoBehaviour {

    public ImageToNumber first;
    public ImageToNumber second;
    public ImageToNumber third;

    private Image firstImage;
    private Image secondImage;
    private Image thirdImage;

    // I'm trying really hard not to break compatibility, I know this still isn't great but it's better than what we had before
    public void UpdateValue(int value) {
        if (firstImage == null)
        {
            firstImage = first.GetComponent<Image>();
        }

        if (secondImage == null)
        {
            secondImage = second.GetComponent<Image>();
        }

        if(third != null && thirdImage == null)
        {
            thirdImage = third.GetComponent<Image>();
        }


        if (third != null)
        {
            value = Mathf.Clamp(value, 0, 999);
        }
        else
        {
            value = Mathf.Clamp(value, 0, 99);
        }

        int[] digits = new int[3];
        digits[0] = value % 10;
        digits[1] = Mathf.FloorToInt(value / 10) % 10;
        digits[2] = Mathf.FloorToInt(value / 100) % 10;

        if(value < 10)
        {
            first.SetValue(digits[0]);
        } else if(value < 100)
        {
            first.SetValue(digits[1]);
            second.SetValue(digits[0]);
        } else if(third != null)
        {
            first.SetValue(digits[2]);
            second.SetValue(digits[1]);
            third.SetValue(digits[0]);
        }

        secondImage.enabled = value >= 10;
        if (third != null) thirdImage.enabled = value >= 100;

        //var stringValue = value.ToString();

        //if (stringValue.Length == 1) {
        //    first.GetComponent<Image>().enabled = true;
        //    second.GetComponent<Image>().enabled = false;

        //    first.SetValue((int)Char.GetNumericValue(stringValue[0]));

        //    //This is worse than everything on Pornhub together
        //    if (third != null){third.GetComponent<Image>().enabled = false; }

        //} else if (stringValue.Length == 2) {
        //    first.GetComponent<Image>().enabled = true;
        //    second.GetComponent<Image>().enabled = true;

        //    first.SetValue((int)Char.GetNumericValue(stringValue[0]));
        //    second.SetValue((int)Char.GetNumericValue(stringValue[1]));

        //    //Uh...again
        //    if (third != null){third.GetComponent<Image>().enabled = false; }

        //} else if (third != null) {
        //    first.GetComponent<Image>().enabled = true;
        //    second.GetComponent<Image>().enabled = true;
        //    third.GetComponent<Image>().enabled = true;

        //    first.SetValue((int)Char.GetNumericValue(stringValue[0]));
        //    second.SetValue((int)Char.GetNumericValue(stringValue[1]));
        //    third.SetValue((int)Char.GetNumericValue(stringValue[2]));
        //}
    }
}
