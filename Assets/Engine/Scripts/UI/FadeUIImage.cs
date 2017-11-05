using UnityEngine;
using UnityEngine.UI;

public class FadeUIImage : MonoBehaviour {

    public Image targetImage;
    public float fadeDuration;

    public void FadeIn(){
        targetImage.canvasRenderer.SetAlpha(0);
        targetImage.CrossFadeAlpha(1, fadeDuration, false);
    }

    public void FadeOut(){
        targetImage.canvasRenderer.SetAlpha(1);
        targetImage.CrossFadeAlpha(0, fadeDuration, false);
    }

}
