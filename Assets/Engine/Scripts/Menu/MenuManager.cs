using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public Text descriptionBox;
    public GameObject tabCursor;
    public float animationLength;
    public float playerControlDelay;
    public MenuPage[] pages;
    public PlayerMachine player;
    public Animator animator;

    private bool active;

    public void openMenu(){
        foreach(MenuPage p in pages){
            p.gameObject.SetActive(false);
        }
        pages[0].gameObject.SetActive(true);

        gameObject.SetActive(true);
        tabCursor.GetComponent<Cursor>().setActivityStatus(true);
        player.setCutsceneMode(true);
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
            StartCoroutine(delayPlayerControl());
        }
    }

    public void closeAnimFinished(){
        player.setCutsceneMode(false);
        gameObject.SetActive(false);
    }

    public void openAnimFinished(){
        tabCursor.SetActive(true);
        active = true;
    }

    public MenuPage getPageOfIndex(int index){
        return pages[index];
    }

    IEnumerator delayPlayerControl(){
        yield return new WaitForSeconds(playerControlDelay);
        player.setCutsceneMode(false);
        player.allowArtUpdate = true;
        player.setFrozenStatus(false);
    }

}
