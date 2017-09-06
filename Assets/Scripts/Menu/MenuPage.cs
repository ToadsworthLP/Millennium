using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPage : MonoBehaviour {

    public GameObject defaultCursor;
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

    public void enterPage(){
        //TODO enable default cursor object
    }

    IEnumerator deactivateAfterDelay(float disableDelay){
        yield return new WaitForSeconds(disableDelay);
        gameObject.SetActive(false);
    }

}
