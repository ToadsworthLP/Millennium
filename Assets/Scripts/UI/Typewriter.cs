using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour {

	[TextArea]
	public string inputText;

	public float speed;

	private Text textComponent;
	private char[] textArray;
	private int textLength;
	private int progress;

	void Start () {
		progress = 0;
		textArray = inputText.ToCharArray ();
		textLength = inputText.Length;
		textComponent = GetComponent<Text> ();
		printPage ();
	}
	
	void printPage () {
		StartCoroutine (printText ());
	}

	IEnumerator printText(){
		yield return new WaitForSeconds (3.0f);
		while (progress < textLength) {
			textComponent.text += textArray [progress];
			progress++;
			yield return new WaitForSeconds (speed);
		}
	}
}
