using System;
using System.Collections;
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
    private Action<ItemPopup> customPopupCloseHandler;

    private bool closeDelayExpired = false;
    private bool isClosing;

    private const float closeDelay = 0.2f;

    private bool modifyCutsceneModeState;

    public void StartPopup(BaseItem itemType, PlayerMachine player, int quantity = 1, Action<ItemPopup> customPopupCloseHandler = null, bool modifyCutsceneModeState = true){
        this.item = itemType;
        this.player = player;
        this.customPopupCloseHandler = customPopupCloseHandler;
        this.modifyCutsceneModeState = modifyCutsceneModeState;
        item = itemType;

        if(modifyCutsceneModeState)
            player.SetCutsceneMode(true);

        player.SetFrozenStatus(true);

        if (quantity > 1){
            nameText.text = "You got "+quantity+"x <color=red>" + itemType.itemName + "</color>!";
        } else{
            if (itemType.name.StartsWith("A") || itemType.name.StartsWith("E") || itemType.name.StartsWith("I") || itemType.name.StartsWith("O") || itemType.name.StartsWith("U")) {
                nameText.text = "You got an <color=red>" + itemType.itemName + "</color>!";
            } else {
                nameText.text = "You got a <color=red>" + itemType.itemName + "</color>!";
            }
        }


        descText.text = itemType.description;

        popup = Instantiate(iconPopup, player.transform).GetComponent<ItemIconPopup>();
        animators.Add(popup.gameObject.GetComponent<Animator>());
        popup.Show(item.icon);
        StartCoroutine(waitBeforeAllowingClose(closeDelay));
    }

    private IEnumerator waitBeforeAllowingClose(float delay){
        yield return new WaitForSeconds(delay);
        transform.localScale = new Vector3(1, 1); //Failsafe if the animation fails to set the scale properly (which it does sometimes)
        closeDelayExpired = true;
    }

    void LateUpdate() {
        if((player.gameManager.inputManager.isInputDown[4] || player.gameManager.inputManager.isInputDown[5]) && closeDelayExpired && !isClosing){
            isClosing = true;

            foreach (Animator a in animators){
                a.SetTrigger("Close");
            }
            player.art.animator.SetBool("ItemGet", false);
            Destroy(popup.gameObject, destructionDelay);
            Destroy(gameObject, destructionDelay);

            player.SetFrozenStatus(false);

            if (customPopupCloseHandler == null){
                if(modifyCutsceneModeState)
                    player.SetCutsceneMode(false);
            } else{
                customPopupCloseHandler.Invoke(this);
            }
        }
    }

}
