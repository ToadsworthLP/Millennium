using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Item", menuName = "Items/Healing Item")]
public class HealingItem : UsableItem {

    public Utils.StatModifier[] statModifiers;
    public float statModificationDelay;

    public override IEnumerator OnOverworldUse(GameManager manager, Action<UsableItem> OnFinished) {
        foreach (Utils.StatModifier mod in statModifiers) {
            switch (mod.statToModify) {
                case Utils.StatType.HP:
                    manager.getBackpack().hp += mod.value;
                    break;

                case Utils.StatType.FP:
                    manager.getBackpack().fp += mod.value;
                    break;

                case Utils.StatType.SP:
                    manager.getBackpack().sp += mod.value;
                    break;
            }
            yield return new WaitForSeconds(statModificationDelay);
        }
    }

    public override IEnumerator OnBattleUse(GameManager manager, Action<UsableItem> OnFinished) {
        throw new NotImplementedException();
    }

}
