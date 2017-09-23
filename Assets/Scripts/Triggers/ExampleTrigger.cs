using UnityEngine;
using UnityEngine.Events;

public class ExampleTrigger : MonoBehaviour {

    [SerializeField]
    public MilleniumEvent testEvent;

    void OnTriggerEnter(Collider other) {
        if(testEvent != null){
            testEvent.Invoke(gameObject, "Test");
        }
    }

}
