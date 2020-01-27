using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelMeter : MonoBehaviour {

    public float fuelConsumption = 0.01f;

    public event Action<FuelMeter> onFuelEmpty;

    float fuelLevel;

    void Start() {
        fuelLevel = 1f;
    }

    void FixedUpdate() {
        fuelLevel -= fuelConsumption * Time.deltaTime;

        if (fuelLevel < 0) {
            fuelLevel = 0;
            if (onFuelEmpty != null) {
                onFuelEmpty(this);
            }
        }

        transform.localEulerAngles = new Vector3(0, 0, 90 - fuelLevel * 180);
    }
}
