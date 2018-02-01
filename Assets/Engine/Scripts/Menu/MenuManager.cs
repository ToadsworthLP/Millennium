using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public TextMeshProUGUI descriptionBox;
    public GameObject tabCursor;
    public float animationLength;
    public float playerControlDelay;
    public MenuPage[] pages;
    public PlayerMachine player;
    public Animator animator;

    private bool active;

    public void OpenMenu(){
        foreach(MenuPage p in pages){
            p.gameObject.SetActive(false);
        }
        pages[0].gameObject.SetActive(true);

        gameObject.SetActive(true);
        tabCursor.GetComponent<Cursor>().SetActivityStatus(true);
        player.SetCutsceneMode(true);
        player.art.animator.SetFloat("normalizedSpeed", 0);
        player.art.animator.SetFloat("side", 0);
    }

    public void CloseMenu() {
        if(active){
            foreach (MenuPage p in pages) {
                p.gameObject.SetActive(true);
            }
            active = false;
            animator.SetTrigger("CloseMenu");
            StartCoroutine(DelayPlayerControl());
        }
    }

    public void CloseAnimFinished(){
        player.SetCutsceneMode(false);
        gameObject.SetActive(false);
    }

    public void OpenAnimFinished(){
        tabCursor.SetActive(true);
        active = true;
    }

    public MenuPage GetPageOfIndex(int index){
        return pages[index];
    }

    IEnumerator DelayPlayerControl(){
        yield return new WaitForSeconds(playerControlDelay);
        player.SetCutsceneMode(false);
        player.allowArtUpdate = true;
        player.SetFrozenStatus(false);
    }

}
