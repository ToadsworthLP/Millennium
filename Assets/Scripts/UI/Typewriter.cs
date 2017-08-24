using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour {

	[TextArea]
	public string[] inputText;
	public float defaultSpeed;
	public Text textComponent;

	private custom_inputs inputManager;
    private PlayerMachine player;
    private Animator animator;

    public AudioClip talkSound;
    public AudioClip skipSound;

    private float speed;
	private bool isPageFinished;

	private int pageCount;
	private int pageProgress;

	void Awake() {
		pageCount = inputText.Length;
		isPageFinished = true;
        speed = defaultSpeed;
        animator = GetComponent<Animator>();
        StartCoroutine(playInAnimation());

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<custom_inputs>();

        player.allowMovement = false;
    }

    void Update(){
        if (inputManager.isInputDown[4] && isPageFinished) {
            player.getAudioSource().PlayOneShot(skipSound);
            if (pageProgress < pageCount-1){
                pageProgress++;
                printPage(pageProgress);
            }else{
                StartCoroutine(playOutAnimation());
            }
        }
	}
	
	void printPage (int pageNumber) {
        animator.SetBool("Printing", true);
        StartCoroutine(printText (inputText[pageNumber]));
	}

	IEnumerator printText(string text){
		char[] textArray = text.ToCharArray();
		int textLength = text.Length;
        int progress = 0;

        isPageFinished = false;
		textComponent.text = "";

		while (progress < textLength) {
            player.getAudioSource().PlayOneShot(talkSound);
            textComponent.text += textArray [progress];
			progress++;
			yield return new WaitForSeconds (speed);
		}

		isPageFinished = true;
        animator.SetBool("Printing", false);
    }

    IEnumerator playInAnimation(){
        yield return new WaitForSeconds(0.05f);
        printPage(0);
    }

    IEnumerator playOutAnimation() {
        animator.SetTrigger("Close");
        player.allowMovement = true;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
