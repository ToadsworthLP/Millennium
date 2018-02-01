using System;
using TMPro;
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
    public TextMeshProUGUI textComponent;

    public virtual bool GetActive() {
        return true;
    }

    public virtual void OnCursorInit(Cursor cursor) {
        if (useColorHighlight) {
            textComponent = gameObject.GetComponent<TextMeshProUGUI>();
            textComponent.canvasRenderer.SetColor(defaultColor);
        }
    }

    public virtual Vector3 GetGrabPoint() {
        return handle.position;
    }

    public virtual void OnCancelPressed() { }

    public virtual void OnCursorLeave() {
        if(useColorHighlight){
            textComponent.CrossFadeColor(defaultColor, colorFadeTime, false, false);
        }

        if(useGameObjectHighlight){
            highlightObject.SetActive(false);
        }
    }

    public virtual void OnCursorSelect() {
        if (useColorHighlight) {
            textComponent.CrossFadeColor(highlightColor, colorFadeTime, false, false);
        }

        if (useGameObjectHighlight && highlightObject != null) {
            highlightObject.SetActive(true);
        }
    }

    public virtual void OnOKPressed() { }

    public virtual void OnSideKeyPressed(Utils.EnumDirection direction) { }
}
