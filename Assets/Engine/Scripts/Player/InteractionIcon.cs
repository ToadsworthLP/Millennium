using UnityEngine;

public class InteractionIcon : MonoBehaviour {

    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public void ShowIcon(){
        if(!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
    }

	public void HideIcon(){
        if(gameObject.activeInHierarchy)
            animator.SetTrigger("Disappear");
    }

    protected void DisableIcon(){
        gameObject.SetActive(false);
    }

}
