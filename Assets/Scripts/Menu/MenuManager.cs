using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public Text descriptionBox;
    public GameObject tabCursor;
    public float animationLength;
    public MenuPage[] pages;

    private bool active;
    private PlayerMachine player;
    private Animator animator;

    public void openMenu(){
        if(animator == null){
            animator = GetComponent<Animator>();
        }
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        }

        foreach(MenuPage p in pages){
            p.gameObject.SetActive(false);
        }
        pages[0].gameObject.SetActive(true);

        gameObject.SetActive(true);
        player.allowMovement = false;
        player.art.animator.SetFloat("normalizedSpeed", 0);
        player.art.animator.SetFloat("side", 0);
    }

    public void closeMenu() {
        if(active){
            foreach (MenuPage p in pages) {
                p.gameObject.SetActive(true);
            }
            active = false;
            animator.SetTrigger("CloseMenu");
        }
        player.allowArtUpdate = true;
    }

    public void closeAnimFinished(){
        player.allowMovement = true;
        gameObject.SetActive(false);
    }

    public void openAnimFinished(){
        tabCursor.SetActive(true);
        active = true;
    }

    public MenuPage getPageOfIndex(int index){
        return pages[index];
    }

}
