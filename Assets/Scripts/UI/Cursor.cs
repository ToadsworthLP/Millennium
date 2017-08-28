using UnityEngine;

public class Cursor : MonoBehaviour {

    public CursorMode mode;
    public GameObject[] options;
    public float movementTime;

    private custom_inputs inputManager;

    private int selectedIndex;
    private ISelectable selectedOption;
    private ISelectable prevOption;

    private Vector3 animVelocity = Vector3.zero;

    private Vector3 targetPosition;

    void Awake(){
        if(options.Length > 1) {
            selectedIndex = 0;
        }else{
            Debug.LogWarning("A cursor without at least two valid options was enabled! Disabling!");
            gameObject.SetActive(false);
        }
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<custom_inputs>();
        selectedOption = options[selectedIndex].GetComponent<ISelectable>();

        RectTransform targetTransform = options[selectedIndex].GetComponent<RectTransform>();
        targetPosition = selectedOption.getGrabPoint();
        transform.position = targetPosition;
        selectedOption.onCursorSelect();
    }

    void Update() {
        if(inputManager.isInputDown[4]){
            selectedOption.onOKPressed();
        } else if (inputManager.isInputDown[5]) {
            selectedOption.onCancelPressed();
        } else if(mode == CursorMode.VERTICAL && inputManager.isInputDown[0] && selectedIndex > 0){
            selectedIndex--;
            cursorMoved();
        }else if (mode == CursorMode.VERTICAL && inputManager.isInputDown[1] && selectedIndex < options.Length-1){
            selectedIndex++;
            cursorMoved();
        } else if(mode == CursorMode.HORIZONTAL && inputManager.isInputDown[2] && selectedIndex > 0){
            selectedIndex--;
            cursorMoved();
        }else if (mode == CursorMode.HORIZONTAL && inputManager.isInputDown[3] && selectedIndex<options.Length-1){
            selectedIndex++;
            cursorMoved();
        }

        if(transform.position != targetPosition){
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref animVelocity, movementTime);
        }
    }

    void cursorMoved(){
        prevOption = selectedOption;
        selectedOption = options[selectedIndex].GetComponent<ISelectable>();

        targetPosition = selectedOption.getGrabPoint();

        prevOption.onCursorLeave();
        selectedOption.onCursorSelect();
    }
}

public enum CursorMode{
    VERTICAL, HORIZONTAL
}
