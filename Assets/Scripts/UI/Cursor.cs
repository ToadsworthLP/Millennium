using System;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour {

    public CursorMode mode;
    public bool oneUse;
    public bool keepSelectedIndex;
    public float moveCooldown;
    public GameObject[] optionObjects;

    public AudioClip moveSound;
    public AudioClip okPressSound;
    public AudioClip cancelPressSound;

    public float movementTime;
    public bool enableStartAnimation;
    public bool enableIdleAnimation;
    [HideInInspector]
    public custom_inputs inputManager;

    [HideInInspector]
    public int selectedIndex;
    [HideInInspector]
    public int previousSelectedIndex;

    private ISelectable[] options;
    private AudioSource audioSource;

    private bool active;
    private float timeSinceLastMove;
    private Vector3 startPosition;

    private Vector3 animVelocity = Vector3.zero;

    private Vector3 targetPosition;

    void OnEnable() {
        if (optionObjects.Length > 1) {
            if(!keepSelectedIndex){
                selectedIndex = 0;
                previousSelectedIndex = 0;
            }
        } else {
            Debug.LogWarning("A cursor without at least two valid options was enabled! Disabling!");
            gameObject.SetActive(false);
        }
        PlayerMachine player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        audioSource = player.audioSource;
        inputManager = player.inputManager;

        options = new ISelectable[optionObjects.Length];
        for (int i = 0; i < optionObjects.Length; i++) {
            options[i] = optionObjects[i].GetComponent<ISelectable>();
            options[i].onCursorInit(this);
        }

        RectTransform targetTransform = optionObjects[selectedIndex].GetComponent<RectTransform>();
        targetPosition = options[selectedIndex].getGrabPoint();

        if(enableStartAnimation){
            if(startPosition == Vector3.zero){
                startPosition = transform.position;
            }
            transform.position = startPosition;
        } else {
            transform.position = targetPosition;
        }
        
        options[selectedIndex].onCursorSelect();

        active = true;
    }

    void Update() {
        if (active && isDelayOver()) {
            if (inputManager.isInputDown[4] && options[selectedIndex].getActive()) {
                audioSource.PlayOneShot(okPressSound);
                options[selectedIndex].onOKPressed();
                if (oneUse) { active = false; }
            } else if (inputManager.isInputDown[5] && options[selectedIndex].getActive()) {
                audioSource.PlayOneShot(cancelPressSound);
                options[selectedIndex].onCancelPressed();
                if (oneUse) { active = false; }
            } else if (mode == CursorMode.VERTICAL && inputManager.isInput[0] && selectedIndex > 0) {
                cursorMoved(-1);
                options[selectedIndex].onSideKeyPressed(Utils.EnumDirection.UP);
            } else if (mode == CursorMode.VERTICAL && inputManager.isInput[1] && selectedIndex < optionObjects.Length - 1) {
                cursorMoved(1);
                options[selectedIndex].onSideKeyPressed(Utils.EnumDirection.DOWN);
            } else if (mode == CursorMode.HORIZONTAL && inputManager.isInput[2] && selectedIndex > 0) {
                cursorMoved(-1);
                options[selectedIndex].onSideKeyPressed(Utils.EnumDirection.LEFT);
            } else if (mode == CursorMode.HORIZONTAL && inputManager.isInput[3] && selectedIndex < optionObjects.Length - 1) {
                cursorMoved(1);
                options[selectedIndex].onSideKeyPressed(Utils.EnumDirection.RIGHT);
            }
        }

        if (enableIdleAnimation) {
            float offset = Mathf.Sin(Time.fixedTime * 10) * 2;
            targetPosition = new Vector3(options[selectedIndex].getGrabPoint().x + offset, options[selectedIndex].getGrabPoint().y, options[selectedIndex].getGrabPoint().z);
        }

        if (transform.position != targetPosition) {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref animVelocity, movementTime);
        }

        if (moveCooldown > 0) {
            timeSinceLastMove += Time.deltaTime;
        }

    }

    void cursorMoved(int amount) {
        previousSelectedIndex = selectedIndex;

        selectedIndex += amount;
        options[selectedIndex] = options[selectedIndex];

        if (!enableIdleAnimation) {
            targetPosition = options[selectedIndex].getGrabPoint();
        }

        audioSource.PlayOneShot(moveSound);
        options[previousSelectedIndex].onCursorLeave();
        options[selectedIndex].onCursorSelect();

        if (moveCooldown > 0) {
            timeSinceLastMove = 0;
        }
    }

    bool isDelayOver() {
        if (timeSinceLastMove >= moveCooldown) {
            return true;
        } else {
            return false;
        }
    }

    public void setActivityStatus(bool active){
        this.active = active;
        GetComponent<Image>().enabled = active;
        enableIdleAnimation = active;
    }

}

public enum CursorMode{
    VERTICAL, HORIZONTAL
}
