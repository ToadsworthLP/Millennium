using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour {

	[TextArea]
	public string[] inputText;
	public float defaultSpeed;
	public Text textComponent;
	public custom_inputs inputManager;

    private Animator animator;

    private float speed;
	private bool isPageFinished;

	private int pageCount;
	private int pageProgress;

	void Start() {
		pageCount = inputText.Length;
		isPageFinished = true;
        speed = defaultSpeed;
        animator = GetComponent<Animator>();
        StartCoroutine(playInAnimation());
    }

	void Update(){
        if (inputManager.isInputDown[4] && isPageFinished) {
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
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
