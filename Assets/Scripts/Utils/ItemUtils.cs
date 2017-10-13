using UnityEngine;

public class ItemUtils : MonoBehaviour {

    private Backpack backpack;
    private GameObject player;

    public Backpack getBackpack(){
        if (backpack == null) {
            backpack = FindObjectOfType<Backpack>();
        }
        return backpack;
    }

    public GameObject getPlayer(){
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        return player;
    }
}
