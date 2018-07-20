using System;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Map Data")]
    public IntersceneLoadingZone[] sceneEntrances;

    [Header("Input")]
    public custom_inputs inputManager;
    public PlayerGamepad playerGamepad;

    [Header("Player")]
    public MenuManager menuManager;
    public PlayerMachine playerMachine;
    public Animator playerAnimator;
    private Backpack backpack;

    [Header("Rendering & UI")]
    public GameObject mainCamera;
    public GameObject renderTextureParent;
    public RectTransform uiParent;
    public FadeUIImage blackOverlay;
    [SerializeField]
    private RenderTexture playerRenderTexture;
    private int renderTextureUses;
    private CameraController cameraController;

    public Backpack GetBackpack(){
        if(backpack == null){
            backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
        }
        return backpack;
    }

    public CameraController GetCameraController(){
        if(cameraController == null){
            cameraController = mainCamera.GetComponent<CameraController>();
        }
        return cameraController;
    }

    public RenderTexture GetPlayerRenderTexture(){
        renderTextureUses++;
        renderTextureParent.SetActive(true);
        return playerRenderTexture;
    }

    //Use this to tell the script that the render texture isn't needed anymore. If it isn't needed for anything, it won't be updated.
    public void ReleaseRenderTexture(){
        renderTextureUses--;
        if(renderTextureUses <= 0){
            renderTextureParent.SetActive(false);
        }
    }
}
