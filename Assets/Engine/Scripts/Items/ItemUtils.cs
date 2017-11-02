using UnityEngine;

public class ItemUtils : MonoBehaviour {

    private GameManager gameManager { get{ if(gameManager == null){ GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>(); } return gameManager; } set{ gameManager = value; } }

    public Backpack getBackpack(){
        return gameManager.backpack;
    }

    public GameObject getPlayer(){
        return gameManager.playerMachine.gameObject;
    }
}
