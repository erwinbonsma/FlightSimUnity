using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {
    public GameObject target;
    public bool isPositionFixed;

    void Update() {
        if (target == null) {
            return;
        }

        if (isPositionFixed) {
            Vector3 lookAt = target.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(lookAt);
        } else {
            transform.position = target.transform.position - target.transform.forward + target.transform.up * 0.3f;
            transform.rotation = target.transform.rotation;
        }
    }
}
