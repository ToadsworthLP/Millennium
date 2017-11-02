using UnityEngine;

public class ExampleTriggeredObject : MonoBehaviour {

    public bool isPersistent;
    public string shelfKey;
    private GameManager gameManager;

    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (isPersistent){
            gameObject.SetActive(gameManager.getShelfData<bool>(shelfKey, false));
        }
    }

    [SerializeField]
    public void trigger(GameObject sender, object args) {
        print("Trigger activated with message " + args.ToString());
        gameManager.getBackpack().coins += 10;
        if(isPersistent)gameManager.setShelfData(shelfKey, !gameObject.activeInHierarchy);
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

}
