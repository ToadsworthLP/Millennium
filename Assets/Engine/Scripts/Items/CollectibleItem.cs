using UnityEngine;

public class CollectibleItem : MonoBehaviour {

    public BaseItem itemType;
    public AudioClip collectSound;
    public SpriteRenderer art;
    public GameObject itemPopup;

    private GameObject uiParent;
    private PlayerMachine player;

    void Awake() {
        art.sprite = itemType.icon;
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            player = other.gameObject.GetComponent<PlayerMachine>();

            player.gameManager.GetBackpack().items.Add(itemType);
            ItemPopup popup = Instantiate(itemPopup, player.gameManager.uiParent.transform).GetComponent<ItemPopup>();
            player.audioSource.PlayOneShot(collectSound);
            player.art.animator.SetBool("ItemGet", true);
            popup.player = player;
            popup.item = itemType;
            popup.StartPopup(itemType, player);
            Destroy(gameObject);
        }
    }

}
