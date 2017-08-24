using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleTriggeredObject : MonoBehaviour, ITriggerable {

    public void triggerObject(string args) {
        print("Trigger activated with arguments "+args+" with target "+ gameObject.name);
        Destroy(gameObject);
    }

}
