using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory Item", order = 1)]
public class InventoryItem : ScriptableObject {
    public string itemName = "New Item";
    [TextArea]
    public string itemDescription = null;
    public Sprite icon = null;

    public bool isImportantItem;
    public bool usableOnOverworld = true;
    public bool usableInBattle = true;

    public StatModifier[] statModifiers;

    public GameObject spawnOnOverworldUse;
    public GameObject spawnOnBattleUse;
}

[System.Serializable]
public class StatModifier{
    public StatType statToModify;
    public int value;
}

public enum StatType
{
    HP, FP, SP, ATTACK, DEFENSE
}