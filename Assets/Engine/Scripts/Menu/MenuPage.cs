using System.Collections;
using UnityEngine;

public class MenuPage : MonoBehaviour {

    [HideInInspector]
    public Animator animator;

    public void FocusPage(){
        if(animator == null){
            animator = GetComponent<Animator>();
        }
    }

    public void UnfocusPage(float disableDelay){
        StartCoroutine(DeactivateAfterDelay(disableDelay));
    }

    IEnumerator DeactivateAfterDelay(float disableDelay){
        yield return new WaitForSeconds(disableDelay);
        gameObject.SetActive(false);
    }

}
