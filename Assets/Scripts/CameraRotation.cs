using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {
    public GameObject target;

    void Update() {
        if (target != null) {
            Vector3 lookAt = target.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(lookAt);
        }
    }
}
