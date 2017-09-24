using System.Collections;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    private Backpack backpack;
    private GameObject player;

    public void Start() {
        backpack = gameObject.GetComponent<Backpack>();
    }

    public void useItem(GameMode mode, InventoryItem item){
        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if(item.callFunctionOnUse)
            StartCoroutine(item.functionName, new ItemParameter(mode, item.functionArgs));

        if(item.statModifiers.Length > 0){
            foreach (StatModifier mod in item.statModifiers) {
                switch(mod.statToModify){
                    case StatType.HP:
                        backpack.hp += mod.value;
                        break;

                    case StatType.FP:
                        backpack.fp += mod.value;
                        break;

                    case StatType.SP:
                        backpack.sp += mod.value;
                        break;
                }
            }
        }

        backpack.items.Remove(item);
    }

    IEnumerator poison(ItemParameter args){
        if (args.parameters.Length == 2){
            int damage = int.Parse(args.parameters[0]);
            int duration = int.Parse(args.parameters[1]);

            for (int i = 0; i < duration; i++) {
                yield return new WaitForSeconds(1);
                backpack.hp -= damage;
            }
        }
    }
}
