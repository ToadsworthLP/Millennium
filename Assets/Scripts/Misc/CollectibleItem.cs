using UnityEngine;

public class CollectibleItem : MonoBehaviour {

    public InventoryItem itemType;
    public AudioClip collectSound;
    public SpriteRenderer art;
    public GameObject itemPopup;

    private GameObject uiParent;
    private PlayerMachine player;

    void Awake() {
        art.sprite = itemType.icon;
        uiParent = GameObject.FindGameObjectWithTag("UIParent");
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            player = other.gameObject.GetComponent<PlayerMachine>();

            player.backpack.items.Add(itemType);
            ItemPopup popup = Instantiate(itemPopup, uiParent.transform).GetComponent<ItemPopup>();
            player.audioSource.PlayOneShot(collectSound);
            player.art.animator.SetBool("ItemGet", true);
            popup.player = player;
            popup.item = itemType;
            popup.startPopup(itemType, player);
            Destroy(gameObject);
        }
    }

}
