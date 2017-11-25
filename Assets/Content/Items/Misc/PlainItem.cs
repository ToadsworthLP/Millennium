using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plain Item", menuName = "Items/Plain Item")]
public class PlainItem : UsableItem {

    public override IEnumerator OnBattleUse(GameManager manager, Action<UsableItem> OnFinished) {
        OnFinished(this);
        return null;
    }

    public override IEnumerator OnOverworldUse(GameManager manager, Action<UsableItem> OnFinished) {
        return null;
    }

}
