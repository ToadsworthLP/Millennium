using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkIcon : MonoBehaviour {

    public Sprite icon;

    private Animator animator;

    void Awake() {
        gameObject.GetComponent<SpriteRenderer>().sprite = icon;
        animator = GetComponent<Animator>();
    }

    public void playInAnimation(){

    }

    public void playOutAnimation() {
        StartCoroutine(outAnimation());
    }

    IEnumerator outAnimation() {
        animator.SetTrigger("Disappear");
        yield return new WaitForSeconds(0.8f);
        gameObject.SetActive(false);
    }

}
