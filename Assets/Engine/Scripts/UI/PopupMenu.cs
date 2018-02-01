using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour {

    public Cursor cursor;
    public GameObject optionPrefab;
    public Transform optionParent;
    public RectTransform backgroundPanel;
    public Vector2 optionOrigin;
    private PopupMenuSettings[] options;

    public void SetupPopupMenu (List<PopupMenuSettings> options){
        this.options = options.ToArray();
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        int i = 0;
        foreach(PopupMenuSettings option in options){
            GameObject obj = Instantiate(optionPrefab, optionParent);
            PopupMenuOption objOption = obj.GetComponent<PopupMenuOption>();

            objOption.label = option.label;
            objOption.action = option.action;
            objOption.parentCanvasRenderer = canvasGroup;
            
            if(option.highlightColor != null){
                objOption.useColorHighlight = true;
                objOption.highlightColor = option.highlightColor;
            }

            cursor.optionObjects.Add(obj);
            i++;
        }

        cursor.gameObject.SetActive(true);
    }
}

[Serializable]
public struct PopupMenuSettings{
    public string label;
    public Color highlightColor;
    public Action<PopupMenuOption> action;

    public PopupMenuSettings(string label, Color highlightColor, Action<PopupMenuOption> action){
        this.label = label;
        this.highlightColor = highlightColor;
        this.action = action;
    }
}
