using UnityEngine;

public class ExampleTrigger : MonoBehaviour {

    [SerializeField]
    public GameObject target;
    public string message;

    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && target != null){
            target.GetComponent<ITriggerable>().triggerObject(message);
        }
    }

}
