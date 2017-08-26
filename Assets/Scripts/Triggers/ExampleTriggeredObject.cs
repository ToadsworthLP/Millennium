using UnityEngine;

public class ExampleTriggeredObject : MonoBehaviour, ITriggerable {

    public void triggerObject(string args) {
        print("Trigger activated with arguments "+args+" with target "+ gameObject.name);
        GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>().coins += 10;
        Destroy(gameObject);
    }

}
