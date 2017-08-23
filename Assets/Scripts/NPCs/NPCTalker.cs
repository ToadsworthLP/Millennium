using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalker : MonoBehaviour {

    [TextArea]
    public string[] text;
    public GameObject talkIcon;
    public GameObject speechBubble;
    public RectTransform hudParent;

    private custom_inputs inputManager;
    private PlayerMovement player;
    private GameObject currentBubble;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<custom_inputs>();
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            talkIcon.active = true;
            player.allowJumping = false;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            talkIcon.active = false;
            player.allowJumping = true;
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player") && inputManager.isInput[4] && currentBubble == null) {
            GameObject bubble = Instantiate(speechBubble, hudParent);
            bubble.GetComponent<Typewriter>().inputText = text;
            bubble.active = true;
            currentBubble = bubble;
        }
    }
}
