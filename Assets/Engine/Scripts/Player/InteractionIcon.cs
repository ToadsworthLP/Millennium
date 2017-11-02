using UnityEngine;

public class InteractionIcon : MonoBehaviour {

    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public void showIcon(){
        if(!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
    }

	public void hideIcon(){
        if(gameObject.activeInHierarchy)
            animator.SetTrigger("Disappear");
    }

    protected void disableIcon(){
        gameObject.SetActive(false);
    }

}
