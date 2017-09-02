using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour {

    public AudioClip collectSound;
    public Transform art;
    public float spinSpeed;
    public int value;

    private Backpack backpack;

    void Start() {
        backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
    }

    void Update() {
        art.rotation *= Quaternion.AngleAxis(spinSpeed * Time.deltaTime, transform.up);
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            backpack.fp += value;
            other.gameObject.GetComponent<PlayerMachine>().audioSource.PlayOneShot(collectSound);
            Destroy(gameObject);
        }
    }
}
