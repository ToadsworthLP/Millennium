using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory Item")]
public class InventoryItem : ScriptableObject {
    [Header("Properties")]
    public string itemName = "New Item";
    [TextArea]
    public string itemDescription = null;
    public Sprite icon = null;

    public bool isImportantItem;
    public bool usableOnOverworld = true;
    public bool usableInBattle = true;

    [Header("Stat Modifiers")]
    public StatModifier[] statModifiers;

    [Header("Custom Function")]
    public bool callFunctionOnUse;
    public string functionName;
    public string[] functionArgs;
}

[System.Serializable]
public class StatModifier{
    public StatType statToModify;
    public int value;
}

[System.Serializable]
public enum StatType{
    HP, FP, SP, ATTACK, DEFENSE
}
