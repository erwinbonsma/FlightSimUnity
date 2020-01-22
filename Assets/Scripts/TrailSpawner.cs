using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailSpawner : MonoBehaviour {
    public GameObject player;
    public GameObject puffPrefab;
    public int maxPuffs = 100;
    public float spawnPeriod = 0.5f;

    GameObject[] puffs;
    int firstPuff = 0;
    int lastPuff = 0;
    float timeSinceLastPuff = 0;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        maxPuffs = Math.Max(maxPuffs, 10);

        puffs = new GameObject[maxPuffs];
    }

    void FixedUpdate() {
        timeSinceLastPuff += Time.deltaTime;

        if (timeSinceLastPuff > spawnPeriod) {
            timeSinceLastPuff = 0;

            int prevPuff = lastPuff;
            lastPuff = (lastPuff + 1) % maxPuffs;
            if (lastPuff == firstPuff) {
                Destroy(puffs[firstPuff]);
                firstPuff = (firstPuff + 1) % maxPuffs;
            }

            GameObject newPuffObject = Instantiate(puffPrefab, player.transform.position, Quaternion.identity);
            puffs[lastPuff] = newPuffObject;
        }
    }
}
