using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadCamera : MonoBehaviour {

    TrailManager trailManager;

    void Start() {
        //transform.rotation.SetLookRotation(Vector3.down, Vector3.right);

        trailManager = GameObject.FindGameObjectWithTag("TrailManager").GetComponent<TrailManager>();
    }

    void Update() {
        Vector3 min = trailManager.MinBound;
        Vector3 max = trailManager.MaxBound;

        Debug.Log("Min = " + min);
        Debug.Log("Max = " + max);

        transform.position = new Vector3((min.x + max.x) / 2, max.y + 10, (min.z + max.z) / 2);
    }
}
