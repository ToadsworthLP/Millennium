using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour {

    [TextArea]
	public string[] inputText;
	public float defaultSpeed;
    public float waitDelay;
	public Text textComponent;

    public AudioClip talkSound;
    public AudioClip skipSound;
    public Image bubbleImage;
    public Text bubbleText;

    public delegate void PageFinished(int page);
    public PageFinished OnPageFinished;

    public delegate void BubbleClosed();
    public BubbleClosed OnBubbleClosed;

    private custom_inputs inputManager;
    private PlayerMachine player;
    private Animator animator;

    private float speed;
	private bool isPageFinished;

	private int pageCount;
	private int pageProgress;

    void Awake() {
		isPageFinished = true;
        speed = defaultSpeed;
        animator = GetComponent<Animator>();
        StartCoroutine(playInAnimation());

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<custom_inputs>();
    }

    void Update(){
        if (inputManager.isInputDown[4] && isPageFinished) {
            player.audioSource.PlayOneShot(skipSound);
            if (pageProgress < pageCount-1){
                pageProgress++;
                printPage(pageProgress);
            }else{
                StartCoroutine(playOutAnimation());
            }
        }else if(inputManager.isInputDown[4] && !isPageFinished) {
            StopAllCoroutines();
            textComponent.text = inputText[pageProgress].Replace("|", string.Empty);
            isPageFinished = true;
            animator.SetBool("Printing", false);
            if (OnPageFinished != null) { OnPageFinished(pageProgress); }
        }
	}

    public void StartWriting(string[] text){
        inputText = text;
        pageCount = inputText.Length;
        gameObject.SetActive(true);
    }
	
	void printPage (int pageNumber) {
        animator.SetBool("Printing", true);
        StartCoroutine(printText (inputText[pageNumber]));
	}

	IEnumerator printText(string text){
		char[] textArray = text.ToCharArray();
        char spaceChar = " ".ToCharArray()[0];
        char waitChar = "|".ToCharArray()[0];

        int textLength = text.Length;
        int progress = 0;

        isPageFinished = false;
		textComponent.text = "";

		while (progress < textLength) {
            if (textArray[progress] != waitChar) {
                if (textArray[progress] != spaceChar) {
                    player.audioSource.PlayOneShot(talkSound);
                }
                textComponent.text += textArray[progress];
                progress++;
                yield return new WaitForSeconds(speed);
            }else{
                progress++;
                yield return new WaitForSeconds(waitDelay);
            }
		}

		isPageFinished = true;
        animator.SetBool("Printing", false);
        if (OnPageFinished != null) { OnPageFinished(pageProgress); }
    }

    IEnumerator playInAnimation(){
        yield return new WaitForSeconds(0.05f);
        printPage(0);
    }

    IEnumerator playOutAnimation() {
        animator.SetTrigger("Close");
        if (OnBubbleClosed != null) { OnBubbleClosed(); }
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
