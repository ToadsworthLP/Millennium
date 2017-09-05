using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public Text descriptionBox;

    /*public custom_inputs inputManager;

    public GameObject[] pages;

    private int currentPageIndex;
    private int previousPageIndex;
    private IMenuPage currentPage;

    void Awake() {
        updatePage(0);
        previousPageIndex = 0;
    }

    void Update() {
        if (inputManager.isInputDown[4]) { //Action pressed
            pages[currentPageIndex].GetComponent<IMenuPage>().onEnter();
        }else if(inputManager.isInputDown[2] && currentPageIndex > 0){ //Left pressed
            updatePage(currentPageIndex);
        } else if (inputManager.isInputDown[3] && currentPageIndex < pages.Length){ //Right pressed
            updatePage(currentPageIndex);
        }
    }

    void updatePage(int page){
        previousPageIndex = currentPageIndex;
        currentPageIndex = page;

        pages[currentPageIndex].SetActive(true);

        if (previousPageIndex > currentPageIndex){
            pages[currentPageIndex].GetComponent<Animator>().SetTrigger("TurnLeft");
        } else {
            pages[currentPageIndex].GetComponent<Animator>().SetTrigger("TurnRight");
        }
        pages[previousPageIndex].GetComponent<IMenuPage>().onClose();
        pages[currentPageIndex].GetComponent<IMenuPage>().onOpen();
    }*/

}
