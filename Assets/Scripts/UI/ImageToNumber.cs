using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageToNumber : MonoBehaviour {

    public Sprite[] NumberImages;

    public void Start() {
        SetValue(0);
    }

    public void SetValue(int value) {
        value = Mathf.Clamp(value, 0, NumberImages.Length - 1);

        GetComponent<Image>().sprite = NumberImages[value];
    }

}
