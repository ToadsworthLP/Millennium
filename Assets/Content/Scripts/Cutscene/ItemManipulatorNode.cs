using System;
using UnityEngine;

public class ItemManipulatorNode : BaseCutsceneNode
{
    public BaseItem targetItem;
    public ManipulationMode manipulationMode;
    public int quantity;

    [Space]
    [Header("If adding items")]
    public GameObject itemGotPopupPrefab;
    public GameObject uiParent;
    public AudioClip collectSound;

    public enum ManipulationMode { ADD, REMOVE }

    public override void CallNode() {
        GameManager gameManager = cutsceneManager.gameManager;

        switch (manipulationMode) {
            case ManipulationMode.ADD:
                for (int i = 0; i < quantity; i++) {
                    gameManager.GetBackpack().items.Add(targetItem);
                }

                ItemPopup popup = Instantiate(itemGotPopupPrefab, gameManager.uiParent.transform).GetComponent<ItemPopup>();
                gameManager.playerMachine.audioSource.PlayOneShot(collectSound);
                gameManager.playerMachine.art.animator.SetBool("ItemGet", true);
                popup.StartPopup(targetItem, gameManager.playerMachine, quantity, ItemGotPopupClosed, false);

                break;
            case ManipulationMode.REMOVE:
                try{
                    for (int i = 0; i < quantity; i++) {
                        gameManager.GetBackpack().items.Remove(targetItem);
                    }
                    CallOutputSlot("Next Node");
                } catch(Exception){
                    CallOutputSlot("On Error");
                }
                break;
        }
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
        SetOutputSlot("On Error");
    }

    private void ItemGotPopupClosed(ItemPopup popup){
        CallOutputSlot("Next Node");
    }
}
