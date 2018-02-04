using System;
using UnityEngine;

public class GameManager : MonoBehaviour {

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
    public HUDController hudController;
    public FadeUIImage blackOverlay;
    [SerializeField]
    private RenderTexture playerRenderTexture;
    private int renderTextureUses;
    private SmoothCameraMovement cameraController;

    public Backpack GetBackpack(){
        if(backpack == null){
            backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
        }
        return backpack;
    }

    public SmoothCameraMovement GetCameraController(){
        if(cameraController == null){
            cameraController = mainCamera.GetComponent<SmoothCameraMovement>();
        }
        return cameraController;
    }

    public RenderTexture GetPlayerRenderTexture(){
        renderTextureUses++;
        renderTextureParent.SetActive(true);
        return playerRenderTexture;
    }

    public void UpdateHammer(HammerAsset hammerType){
        playerMachine.hammer.hammer = hammerType;
        playerMachine.hammer.hammerSpriteRenderer.sprite = hammerType.hammerSprite;
    }

    //Use this to tell the script that the render texture isn't needed anymore. If it isn't needed for anything, it won't be updated.
    public void ReleaseRenderTexture(){
        renderTextureUses--;
        if(renderTextureUses <= 0){
            renderTextureParent.SetActive(false);
        }
    }

    public T GetShelfData<T>(string key, T defaultValue){
        object data;
        if(GetBackpack().shelf.TryGetValue(key, out data)){
            try {
                return (T)data;
            } catch (Exception e) {
                Debug.LogError("Failed to cast entry " + key + " to requested type: " + e.Message + e.StackTrace);
            }
        };
        return defaultValue;
    }

    public void SetShelfData(string key, object value) {
        object val;
        if (backpack.shelf.TryGetValue(key, out val)) {
            backpack.shelf[key] = value;
        } else {
            backpack.shelf.Add(key, value);
        }
    }

}
