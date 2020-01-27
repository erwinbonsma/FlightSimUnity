using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    Camera firstPersonCamera;
    Camera overheadCamera;

    void Start() {
        firstPersonCamera = Camera.main;
        overheadCamera = GameObject.FindGameObjectWithTag("OverheadCamera").GetComponent<Camera>();

        EnableFirstPersonCamera();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            ToggleCamera();
        }
    }

    public void EnableFirstPersonCamera() {
        firstPersonCamera.enabled = true;
        overheadCamera.enabled = false;
    }

    public void EnableOverheadCamera() {
        firstPersonCamera.enabled = false;
        overheadCamera.enabled = true;
    }

    public void ToggleCamera() {
        if (firstPersonCamera.enabled) {
            EnableOverheadCamera();
        } else {
            EnableFirstPersonCamera();
        }
    }
}
