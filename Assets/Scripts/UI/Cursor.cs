using UnityEngine;

public class Cursor : MonoBehaviour {

    public CursorMode mode;
    public bool oneUse;
    public GameObject[] options;

    public AudioClip moveSound;
    public AudioClip okPressSound;
    public AudioClip cancelPressSound;

    public float movementTime;
    public bool enableIdleAnimation;

    private custom_inputs inputManager;
    private AudioSource audioSource;

    private bool active;
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
        PlayerMachine player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        audioSource = player.audioSource;
        inputManager = player.inputManager;
        selectedOption = options[selectedIndex].GetComponent<ISelectable>();

        RectTransform targetTransform = options[selectedIndex].GetComponent<RectTransform>();
        targetPosition = selectedOption.getGrabPoint();
        transform.position = targetPosition;
        selectedOption.onCursorSelect();

        active = true;
    }

    void Update() {
        if(active){
            if (inputManager.isInputDown[4] && selectedOption.getActive()) {
                audioSource.PlayOneShot(okPressSound);
                selectedOption.onOKPressed();
                if(oneUse){ active = false; }
            } else if (inputManager.isInputDown[5] && selectedOption.getActive()) {
                audioSource.PlayOneShot(cancelPressSound);
                selectedOption.onCancelPressed();
                if (oneUse) { active = false; }
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

            if(enableIdleAnimation){
                float offset = Mathf.Sin(Time.fixedTime * 10)*2;
                targetPosition = new Vector3(selectedOption.getGrabPoint().x + offset, selectedOption.getGrabPoint().y, selectedOption.getGrabPoint().z);
            }

            if (transform.position != targetPosition){
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref animVelocity, movementTime);
            }
        }
        
    }

    void cursorMoved(){
        prevOption = selectedOption;
        selectedOption = options[selectedIndex].GetComponent<ISelectable>();

        if(!enableIdleAnimation){
            targetPosition = selectedOption.getGrabPoint();
        }

        audioSource.PlayOneShot(moveSound);
        prevOption.onCursorLeave();
        selectedOption.onCursorSelect();
    }
}

public enum CursorMode{
    VERTICAL, HORIZONTAL
}
