using UnityEngine;

public class VirtualController : MonoBehaviour {

    public bool updateInput = true;

    public Vector2 direction;
    public bool didDirectionChange;

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

            dir = Vector2.ClampMagnitude(dir, 1);
            didDirectionChange = !dir.Equals(direction);

            direction = dir;

            jumpPressed = inputManager.isInputDown[4];
            hammerPressed = inputManager.isInputDown[5];
        }
    }
}
