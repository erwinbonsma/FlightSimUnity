using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadCamera : MonoBehaviour {

    TrailManager trailManager;
    Camera camera;
    Vector3 targetPosition;

    void Start() {
        trailManager = GameObject.FindGameObjectWithTag("TrailManager").GetComponent<TrailManager>();
        trailManager.onPuffAdded += OnPuffAdded;

        camera = GetComponent<Camera>();
    }

    void OnPuffAdded(GameObject puff) {
        Vector3 min = trailManager.MinBound;
        Vector3 max = trailManager.MaxBound;

        float w = Mathf.Max(1, max.x - min.x);
        float h = Mathf.Max(1, max.z - min.z);
        float targetFrustumHeight = (h * camera.aspect > w) ? h : w / camera.aspect;
        var distance = 1.1f * targetFrustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

        targetPosition.Set((min.x + max.x) / 2, max.y + distance, (min.z + max.z) / 2);
    }

    void FixedUpdate() {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
    }
}
