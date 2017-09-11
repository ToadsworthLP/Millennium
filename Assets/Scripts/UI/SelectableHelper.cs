using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectableHelper : MonoBehaviour, ISelectable {

    public bool useColorHighlight;
    public float colorFadeTime;
    public Color defaultColor;
    public Color highlightColor;

    public bool useGameObjectHighlight;
    public GameObject highlightObject;

    public RectTransform handle;

    [HideInInspector]
    public Text textComponent;

    public virtual bool getActive() {
        return true;
    }

    public virtual void onCursorInit(Cursor cursor) {
        if (useColorHighlight) {
            textComponent = gameObject.GetComponent<Text>();
            textComponent.canvasRenderer.SetColor(defaultColor);
        }
    }

    public virtual Vector3 getGrabPoint() {
        return handle.position;
    }

    public virtual void onCancelPressed() { }

    public virtual void onCursorLeave() {
        if(useColorHighlight){
            textComponent.CrossFadeColor(defaultColor, colorFadeTime, false, false);
        }

        if(useGameObjectHighlight){
            highlightObject.SetActive(false);
        }
    }

    public virtual void onCursorSelect() {
        if (useColorHighlight) {
            textComponent.CrossFadeColor(highlightColor, colorFadeTime, false, false);
        }

        if (useGameObjectHighlight && highlightObject != null) {
            highlightObject.SetActive(true);
        }
    }

    public virtual void onOKPressed() { }

    public virtual void onSideKeyPressed(Utils.EnumDirection direction) { }
}
