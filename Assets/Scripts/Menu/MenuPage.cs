using System.Collections;
using UnityEngine;

public class MenuPage : MonoBehaviour {

    public GameObject pageCursor;

    [HideInInspector]
    public Animator animator;

    public void focusPage(){
        if(animator == null){
            animator = GetComponent<Animator>();
        }
    }

    public void unfocusPage(float disableDelay){
        StartCoroutine(deactivateAfterDelay(disableDelay));
    }

    IEnumerator deactivateAfterDelay(float disableDelay){
        yield return new WaitForSeconds(disableDelay);
        gameObject.SetActive(false);
    }

}
