using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Input")]
    public custom_inputs inputManager;
    public VirtualController controller;

    [Header("Player")]
    public Backpack backpack;
    public MenuManager menuManager;
    public PlayerMachine playerMachine;

    [Header("Rendering")]
    public GameObject mainCamera;
    public RectTransform uiParent;

}
