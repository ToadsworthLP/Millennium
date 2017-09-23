using UnityEngine;

public class ExampleTriggeredObject : MonoBehaviour {

    [SerializeField]
    public void trigger(GameObject sender, string args) {
        print("Trigger activated with message " + args);
        GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>().coins += 10;
        Destroy(gameObject);
    }

}
