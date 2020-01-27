using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour {
    public GameObject player;
    public GameObject puffPrefab;
    public int maxPuffs = 100;
    public float spawnPeriod = 0.5f;

    Vector3 _minBound;
    Vector3 _maxBound;

    GameObject[] puffs;
    int firstPuff = 0;
    int lastPuff = 0;
    float timeSinceLastPuff = 0;

    public int NumPuffs {
        get;
        private set;
    }

    public bool Enabled { get; set; }

    public Vector3 MinBound { get { return _minBound; } }
    public Vector3 MaxBound { get { return _maxBound; } }

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        maxPuffs = Math.Max(maxPuffs, 10);

        puffs = new GameObject[maxPuffs];
        NumPuffs = 0;

        Enabled = true;

        _minBound = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        _maxBound = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    }

    void FixedUpdate() {
        timeSinceLastPuff += Time.deltaTime;

        if (Enabled && timeSinceLastPuff > spawnPeriod) {
            timeSinceLastPuff = 0;

            int prevPuff = lastPuff;
            lastPuff = (lastPuff + 1) % maxPuffs;

            if (lastPuff == firstPuff) {
                Destroy(puffs[firstPuff]);
                firstPuff = (firstPuff + 1) % maxPuffs;
            } else {
                NumPuffs++;
            }

            GameObject newPuffObject = Instantiate(puffPrefab, player.transform.position, Quaternion.identity);
            puffs[lastPuff] = newPuffObject;
            UpdateBounds(newPuffObject.transform.position);
        }
    }

    void UpdateBounds(Vector3 pos) {
        _minBound.x = Math.Min(_minBound.x, pos.x);
        _minBound.y = Math.Min(_minBound.y, pos.y);
        _minBound.z = Math.Min(_minBound.z, pos.z);

        _maxBound.x = Math.Max(_maxBound.x, pos.x);
        _maxBound.y = Math.Max(_maxBound.y, pos.y);
        _maxBound.z = Math.Max(_maxBound.z, pos.z);
    }
}
