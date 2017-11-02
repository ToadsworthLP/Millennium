using UnityEngine;

public class ExampleTriggeredObject : MonoBehaviour {

    [SerializeField]
    public void trigger(GameObject sender, object args) {
        print("Trigger activated with message " + args.ToString());
        GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>().coins += 10;
        Destroy(gameObject);
    }

}
