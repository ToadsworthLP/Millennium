using Millennium.Containers;
using SavePort.Types;
using UnityEngine;

public class ExampleTriggeredObject : MonoBehaviour {

    public bool isPersistent;
    public string shelfKey;
    public ShelfContainer shelf;
    public IntContainer playerCoins;

    private void Start() {
        gameObject.SetActive(shelf.GetShelfData<bool>(shelfKey, true));
    }

    [SerializeField]
    public void Trigger(GameObject sender, object args) {
        print("Trigger activated with message " + args.ToString());
        playerCoins.Value += 10;
        if(isPersistent)shelf.SetShelfData(shelfKey, !gameObject.activeInHierarchy);
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

}
