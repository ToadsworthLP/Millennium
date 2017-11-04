using UnityEngine;

public class ItemUtils : MonoBehaviour {

    private GameManager gameManager;

    public GameManager getGameManager(){
        if (gameManager == null){
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        }
        return gameManager;
    }
}
