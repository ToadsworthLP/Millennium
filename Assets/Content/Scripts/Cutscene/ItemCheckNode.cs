using UnityEngine;

public class ItemCheckNode : ConditionBaseNode {

    [Tooltip("The item to check for")]
    public BaseItem item;

    public override bool Condition() {
        return cutsceneManager.gameManager.GetBackpack().items.Contains(item);
    }

}
