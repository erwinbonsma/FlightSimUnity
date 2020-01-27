using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionReport : MonoBehaviour {

    public GameObject fuelMeter;

    void Start() {
        fuelMeter.GetComponent<FuelMeter>().onFuelEmpty += OnFuelEmpty;
    }

    void OnFuelEmpty(FuelMeter fuelMeter) {
        SceneManager.LoadSceneAsync("MainMenu");
    }

}
