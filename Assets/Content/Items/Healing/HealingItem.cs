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
                    manager.GetBackpack().legacyHP += mod.value;
                    break;

                case Utils.StatType.FP:
                    manager.GetBackpack().legacyFP += mod.value;
                    break;

                case Utils.StatType.SP:
                    manager.GetBackpack().legacySP += mod.value;
                    break;
            }
            yield return new WaitForSeconds(statModificationDelay);
        }
    }

    public override IEnumerator OnBattleUse(GameManager manager, Action<UsableItem> OnFinished) {
        throw new NotImplementedException();
    }

}
