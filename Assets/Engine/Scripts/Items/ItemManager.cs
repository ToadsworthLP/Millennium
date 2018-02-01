using System;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    GameManager manager;

    public void UseItemOnOverworld(UsableItem item){
        if(manager == null)
            manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        StartCoroutine(item.OnOverworldUse(manager, OnFinishUse));

        Backpack backpack = manager.GetBackpack();
        backpack.items.Remove(item);
    }

    public void UseItemInBattle(UsableItem item, Action<UsableItem> OnFinished) {
        if (manager == null)
            manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        StartCoroutine(item.OnBattleUse(manager, OnFinished));

        Backpack backpack = manager.GetBackpack();
        backpack.items.Remove(item);
    }

    public void OnFinishUse(UsableItem item){

    }
}