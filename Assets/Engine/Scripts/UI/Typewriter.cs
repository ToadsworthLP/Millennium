using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour {

	[TextArea]
	public string[] inputText;
	public float defaultSpeed;
    public float waitDelay;
	public TextMeshProUGUI textComponent;

    public AudioClip talkSound;
    public AudioClip skipSound;
    public Image bubbleImage;

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

    private bool outAnimationPlaying;

    private Coroutine typewriterCoroutine;

	void Awake() {
		isPageFinished = true;
        speed = defaultSpeed;
        animator = GetComponent<Animator>();
        StartCoroutine(PlayInAnimation());

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<custom_inputs>();
    }

    void Update(){
        if (inputManager.isInputDown[4] && isPageFinished) {
            player.audioSource.PlayOneShot(skipSound);
            if (pageProgress < pageCount-1){
                pageProgress++;
                PrintPage(pageProgress);
            }else{
                if(!outAnimationPlaying)
                    StartCoroutine(PlayOutAnimation());
            }
        }else if(inputManager.isInputDown[4] && !isPageFinished) {
            StopCoroutine(typewriterCoroutine);
            textComponent.maxVisibleCharacters = textComponent.text.Length;
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
	
	void PrintPage (int pageNumber) {
        animator.SetBool("Printing", true);
        typewriterCoroutine = StartCoroutine(PrintText(inputText[pageNumber]));
	}

	IEnumerator PrintText(string text){
		char[] textArray = Regex.Replace(text, "<.*?>", string.Empty).ToCharArray();
        char spaceChar = " ".ToCharArray()[0];
        char waitChar = "|".ToCharArray()[0];

        int textLength = textArray.Length;
        int progress = 0;

        isPageFinished = false;
		textComponent.text = text.Replace("|", string.Empty);

		while (progress < textLength) {
            if (textArray[progress] != waitChar) {
                if (textArray[progress] != spaceChar) {
                    player.audioSource.PlayOneShot(talkSound);
                }
                textComponent.maxVisibleCharacters = progress+1;
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

    IEnumerator PlayInAnimation(){
        yield return new WaitForSeconds(0.05f);
        PrintPage(0);
    }

    IEnumerator PlayOutAnimation() {
        outAnimationPlaying = true;
        animator.SetTrigger("Close");
        if (OnBubbleClosed != null) { OnBubbleClosed(); }
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
