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

    public void PlayOutAnimation() {
        gameObject.SetActive(true);
        StartCoroutine(OutAnimation());
    }

    IEnumerator OutAnimation() {
        animator.SetTrigger("Disappear");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

}
