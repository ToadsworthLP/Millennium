using UnityEngine;

public class ExampleTriggeredObject : MonoBehaviour {

    public bool isPersistent;
    public string shelfKey;
    private GameManager gameManager;

    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (isPersistent){
            gameObject.SetActive(gameManager.GetShelfData<bool>(shelfKey, false));
        }
    }

    [SerializeField]
    public void Trigger(GameObject sender, object args) {
        print("Trigger activated with message " + args.ToString());
        gameManager.GetBackpack().coins += 10;
        if(isPersistent)gameManager.SetShelfData(shelfKey, !gameObject.activeInHierarchy);
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

}
