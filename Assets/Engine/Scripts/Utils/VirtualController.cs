using UnityEngine;

public class VirtualController : MonoBehaviour {

    public bool updateInput;

    public Vector2 direction;
    public bool didStateChange;
    public bool didXChange;
    public bool didYChange;

    public bool jumpPressed;
    public bool hammerPressed;

    private custom_inputs inputManager;

    void Start () {
        inputManager = GetComponent<custom_inputs>();
        direction = new Vector2();
	}

    void FixedUpdate() {
        if(updateInput){
            Vector2 dir = new Vector2();

            if (inputManager.isInput[0]) { dir.y += 1; }
            if (inputManager.isInput[1]) { dir.y -= 1; }
            if (inputManager.isInput[2]) { dir.x -= 1; }
            if (inputManager.isInput[3]) { dir.x += 1; }

            didXChange = !dir.x.Equals(direction.x);
            didYChange = !dir.y.Equals(direction.y);
            didStateChange = (didXChange || didYChange);

            direction = Vector2.ClampMagnitude(dir, 1);

            jumpPressed = inputManager.isInputDown[4];
            hammerPressed = inputManager.isInputDown[5];
        }
        
    }
}
