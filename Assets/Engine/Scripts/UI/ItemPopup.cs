using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopup : MonoBehaviour {

    public BaseItem item;
    public GameObject iconPopup;
    public Text nameText;
    public Text descText;
    public List<Animator> animators;
    public float destructionDelay;
    [HideInInspector]
    public PlayerMachine player;

    private ItemIconPopup popup;

    public void StartPopup(BaseItem itemType, PlayerMachine player){
        this.player = player;
        item = itemType;

        player.SetCutsceneMode(true);
        player.SetFrozenStatus(true);

        if (itemType.name.StartsWith("A") || itemType.name.StartsWith("E") || itemType.name.StartsWith("I") || itemType.name.StartsWith("O") || itemType.name.StartsWith("U")) {
            nameText.text = "You got an <color=red>" + itemType.itemName + "</color>!";
        } else {
            nameText.text = "You got a <color=red>" + itemType.itemName + "</color>!";
        }

        descText.text = itemType.description;

        popup = Instantiate(iconPopup, player.transform).GetComponent<ItemIconPopup>();
        animators.Add(popup.gameObject.GetComponent<Animator>());
        popup.Show(item.icon);
    }

    void LateUpdate() {
        if(player.gameManager.inputManager.isInputDown[4] || player.gameManager.inputManager.isInputDown[5]){
            foreach(Animator a in animators){
                a.SetTrigger("Close");
            }
            player.SetCutsceneMode(false);
            player.SetFrozenStatus(false);
            player.art.animator.SetBool("ItemGet", false);
            Destroy(popup.gameObject, destructionDelay);
            Destroy(gameObject, destructionDelay);
        }
    }

}
