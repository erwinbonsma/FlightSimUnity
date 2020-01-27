using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelMeter : MonoBehaviour {

    public float fuelConsumption = 0.01f;

    float fuelLevel;

    // Start is called before the first frame update
    void Start() {
        fuelLevel = 1f;
    }

    // Update is called once per frame
    void FixedUpdate() {
        fuelLevel -= fuelConsumption * Time.deltaTime;
        Debug.Log("Fuel level = " + fuelLevel);
        transform.localEulerAngles = new Vector3(0, 0, 90 - fuelLevel * 180);
    }
}
