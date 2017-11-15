using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Input")]
    public custom_inputs inputManager;
    public VirtualController controller;

    [Header("Player")]
    public MenuManager menuManager;
    public PlayerMachine playerMachine;
    private Backpack backpack;

    [Header("Rendering & UI")]
    public GameObject mainCamera;
    public RectTransform uiParent;
    public HUDController hudController;
    public FadeUIImage blackOverlay;
    private SmoothCameraMovement cameraController;

    public Backpack getBackpack(){
        if(backpack == null){
            backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
        }
        return backpack;
    }

    public SmoothCameraMovement getCameraController(){
        if(cameraController == null){
            cameraController = mainCamera.GetComponent<SmoothCameraMovement>();
        }
        return cameraController;
    }

    public T getShelfData<T>(string key, T defaultValue){
        object data;
        if(getBackpack().shelf.TryGetValue(key, out data)){
            try {
                return (T)data;
            } catch (Exception e) {
                Debug.LogError("Failed to cast entry " + key + " to requested type: " + e.Message + e.StackTrace);
            }
        };
        return defaultValue;
    }

    public void setShelfData(string key, object value) {
        object val;
        if (backpack.shelf.TryGetValue(key, out val)) {
            backpack.shelf[key] = value;
        } else {
            backpack.shelf.Add(key, value);
        }
    }

}
