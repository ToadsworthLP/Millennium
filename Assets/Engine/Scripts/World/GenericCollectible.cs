using UnityEngine;

public class GenericCollectible : MonoBehaviour {

    public IntReference targetContainer;
    public int value;

    public AudioClip collectSound;
    public bool destroyAfterCollected = true;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            targetContainer.Value += value;
            other.gameObject.GetComponent<PlayerMachine>().audioSource.PlayOneShot(collectSound);
            
            if(destroyAfterCollected)
                Destroy(gameObject);
        }
    }

}
