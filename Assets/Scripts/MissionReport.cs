using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionReport : MonoBehaviour {

    public GameObject fuelMeter;
    public GameObject trailManagerObject;

    GameObject player;
    GameObject gameControl;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        gameControl = GameObject.FindGameObjectWithTag("GameControl");

        fuelMeter.GetComponent<FuelMeter>().onFuelEmpty += OnFuelEmpty;
    }

    void OnFuelEmpty(FuelMeter fuelMeter) {
        Destroy(player);
        TrailManager trailManager = trailManagerObject.GetComponent<TrailManager>();
        trailManager.Enabled = false;

        CameraControl cameraControl = gameControl.GetComponent<CameraControl>();
        cameraControl.EnableOverheadCamera();

        //SceneManager.LoadSceneAsync("MainMenu");
    }

}
