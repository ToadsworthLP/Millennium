using UnityEngine;
using UnityEngine.UI;

public class SelectableHelper : MonoBehaviour, ISelectable {

    public Color defaultColor;
    public Color highlightColor;
    public RectTransform handle;

    [HideInInspector]
    public Text textComponent;

    public virtual bool getActive() {
        return true;
    }

    public virtual Vector3 getGrabPoint() {
        return handle.position;
    }

    public virtual void onCancelPressed() { }

    public virtual void onCursorLeave() {
        textComponent.CrossFadeColor(defaultColor, 0.2f, false, false);
    }

    public virtual void onCursorSelect() {
        if(textComponent == null) {
            textComponent = gameObject.GetComponent<Text>();
        }
        textComponent.CrossFadeColor(highlightColor, 0.2f, false, false);
    }

    public virtual void onOKPressed() { }

    private void Awake() {
        textComponent = gameObject.GetComponent<Text>();
        textComponent.canvasRenderer.SetColor(defaultColor);
    }

}
