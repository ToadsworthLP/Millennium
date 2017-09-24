using UnityEngine;

public class VirtualDPad : MonoBehaviour {

    public bool updateInput;

    public Vector2 direction;
    public bool didStateChange;
    public bool didXChange;
    public bool didYChange;

    private custom_inputs inputManager;

    void Start () {
        inputManager = GetComponent<custom_inputs>();
        direction = new Vector2();
	}

    void Update() {
        if(updateInput){
            Vector2 dir = new Vector2();

            if (inputManager.isInput[0]) { dir.y += 1; }
            if (inputManager.isInput[1]) { dir.y -= 1; }
            if (inputManager.isInput[2]) { dir.x -= 1; }
            if (inputManager.isInput[3]) { dir.x += 1; }

            didXChange = !dir.x.Equals(direction.x);
            didYChange = !dir.y.Equals(direction.y);
            didStateChange = (didXChange || didYChange);

            direction = dir;
        }
        
    }
}
