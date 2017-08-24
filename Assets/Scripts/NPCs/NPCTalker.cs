using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalker : MonoBehaviour {

    [TextArea]
    public string[] text;
    public GameObject talkIcon;
    public GameObject speechBubble;

    public AudioClip talkSound;
    public AudioClip skipSound;

    private RectTransform uiParent;
    private custom_inputs inputManager;
    private PlayerMachine player;
    private GameObject currentBubble;

    // Use this for initialization
    void Start () {
        uiParent = GameObject.FindGameObjectWithTag("UIParent").GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
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
            GameObject bubble = Instantiate(speechBubble, uiParent);
            Typewriter writer = bubble.GetComponent<Typewriter>();
            writer.inputText = text;
            writer.talkSound = this.talkSound;
            writer.skipSound = this.skipSound;
            bubble.active = true;
            currentBubble = bubble;
        }
    }
}
