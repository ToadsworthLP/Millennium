using System;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    GameManager manager;

    public void useItemOnOverworld(UsableItem item){
        if(manager == null)
            manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        StartCoroutine(item.OnOverworldUse(manager));

        Backpack backpack = manager.getBackpack();
        backpack.items.Remove(item);
    }

    public void useItemInBattle(UsableItem item, Action<UsableItem> OnFinished) {
        if (manager == null)
            manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        StartCoroutine(item.OnBattleUse(manager, OnFinished));

        Backpack backpack = manager.getBackpack();
        backpack.items.Remove(item);
    }
}