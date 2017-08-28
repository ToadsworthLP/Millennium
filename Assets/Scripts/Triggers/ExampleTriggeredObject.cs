using UnityEngine;

public class ExampleTriggeredObject : MonoBehaviour, ITriggerable {

    public void triggerObject<T>(T args) {
        if(args.GetType() == typeof(string)){
            print("Trigger activated with message "+args);
            GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>().coins += 10;
            Destroy(gameObject);
        }
    }

}
