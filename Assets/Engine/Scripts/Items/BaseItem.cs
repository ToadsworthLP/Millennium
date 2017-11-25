using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class BaseItem : ScriptableObject {

    public string itemName;
    [TextArea]
    public string description;
    public Sprite icon;

}

[Serializable]
public abstract class UsableItem : BaseItem {

    public bool isImportantItem;
    public bool usableOnOverworld;
    public bool usableOnBattle;

    public abstract IEnumerator OnOverworldUse(GameManager manager, Action<UsableItem> OnFinished);
    public abstract IEnumerator OnBattleUse(GameManager manager, Action<UsableItem> OnFinished);

}

[Serializable]
public abstract class BadgeItem : BaseItem {

    public abstract IEnumerator OnEquip(GameManager manager);
    public abstract IEnumerator OnUnequip(GameManager manager);

    public string battleAbilityName;
    public int requiredFP;
    public abstract IEnumerator OnBattleAbilityUse();

}
