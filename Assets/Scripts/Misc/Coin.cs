using UnityEngine;

public class Coin : MonoBehaviour {

    public AudioClip collectSound;
    public Transform art;
    public float spinSpeed;
    public int value;

    void Update () {
        art.rotation *= Quaternion.AngleAxis(spinSpeed * Time.deltaTime, transform.up);
    }

    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            Backpack backpack = other.GetComponent<PlayerMachine>().backpack;
            backpack.coins += value;
            other.gameObject.GetComponent<PlayerMachine>().audioSource.PlayOneShot(collectSound);
            Destroy(gameObject);
        }
    }
}
