using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleTrigger : MonoBehaviour {

    [SerializeField]
    public GameObject target;

    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && target != null){
            target.GetComponent<ITriggerable>().triggerObject("Test");
        }
    }

}
