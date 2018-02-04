using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ItemManipulatorNode : BaseCutsceneNode
{
    public BaseItem targetItem;
    public ManipulationMode manipulationMode;
    public int quantity;

    [HideInInspector]
    public GameObject itemGotPopupPrefab;
    [HideInInspector]
    public GameObject uiParent;
    [HideInInspector]
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

#if UNITY_EDITOR
[CustomEditor(typeof(ItemManipulatorNode))]
public class ItemManipulatorNodeEditor : BaseCutsceneNodeEditor
{

    public ItemManipulatorNode itemManipulatorNode;

    SerializedProperty itemPopupProp;
    SerializedProperty uiParentProp;

    private void OnEnable() {
        itemManipulatorNode = (ItemManipulatorNode)target;

        itemPopupProp = serializedObject.FindProperty("itemGotPopupPrefab");
        uiParentProp = serializedObject.FindProperty("uiParent");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(itemManipulatorNode.manipulationMode.Equals(ItemManipulatorNode.ManipulationMode.ADD)){
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(itemPopupProp, new GUIContent("Item Got! Popup Prefab"));
            EditorGUILayout.PropertyField(uiParentProp, new GUIContent("UI Parent"));

            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif
