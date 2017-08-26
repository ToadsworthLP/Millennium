using UnityEngine;

public class Coin : MonoBehaviour {

    public AudioClip collectSound;
    public Transform art;
    public float spinSpeed;
    public int value;

    private Backpack backpack;

    void Start() {
        backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
    }

    void Update () {
        art.rotation *= Quaternion.AngleAxis(spinSpeed * Time.deltaTime, transform.up);
    }

    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            backpack.coins += value;
            other.gameObject.GetComponent<PlayerMachine>().getAudioSource().PlayOneShot(collectSound);
            Destroy(gameObject);
        }
    }
}
