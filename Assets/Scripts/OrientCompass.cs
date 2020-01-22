using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientCompass : MonoBehaviour {
    void Update() {
        // Always keep same orientation wrt to the world
        transform.rotation = Quaternion.identity;
    }
}
