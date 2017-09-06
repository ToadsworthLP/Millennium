using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public Text descriptionBox;
    public float animationLength;
    public MenuPage[] pages;

    public MenuPage getPageOfIndex(int index){
        return pages[index];
    }

}
