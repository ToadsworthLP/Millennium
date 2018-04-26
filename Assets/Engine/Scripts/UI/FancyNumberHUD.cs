using UnityEngine;

public class FancyNumberHUD : MonoBehaviour {

    public IntReference value;
    public FancyNumberHandler numberHandler;
    public Animator animator;
    public Utils.AnimationModifier animationModifier;

    private void Start () {
        value.AddUpdateListener(UpdateDisplay);
	}
	
	public void UpdateDisplay(){
        numberHandler.UpdateValue(value);
        animationModifier.SetProperty(animator);
    }
}
