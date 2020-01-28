using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour {
    public GameObject player;
    public GameObject puffPrefab;
    public int maxPuffs = 100;
    public float spawnPeriod = 0.5f;

    public event Action<GameObject> onPuffAdded;

    Vector3 _minBound;
    Vector3 _maxBound;

    GameObject[] puffs;
    int firstPuff = 0;
    int lastPuff = 0;
    float timeSinceLastPuff = 0;

    Vector3 prevPos;
    float minPuffDist = float.MaxValue;

    // Number of puffs currently alive
    public int NumPuffs {
        get;
        private set;
    }

    public float MinPuffDistance { get { return minPuffDist; } }

    public bool Enabled { get; set; }

    public Vector3 MinBound { get { return _minBound; } }
    public Vector3 MaxBound { get { return _maxBound; } }

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        maxPuffs = Math.Max(maxPuffs, 10);

        puffs = new GameObject[maxPuffs];
        NumPuffs = 0;
        firstPuff = 0;
        lastPuff = maxPuffs - 1;

        Enabled = true;

        _minBound = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        _maxBound = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    }

    void FixedUpdate() {
        timeSinceLastPuff += Time.deltaTime;

        if (Enabled && timeSinceLastPuff > spawnPeriod) {
            timeSinceLastPuff = 0;

            // Clear oldest entry in cyclic array when it's full
            if (NumPuffs == maxPuffs) {
                Destroy(puffs[firstPuff]);
                firstPuff = (firstPuff + 1) % maxPuffs;
            } else {
                NumPuffs++;
            }

            // Create new entry
            GameObject newPuffObject = Instantiate(puffPrefab, player.transform.position, Quaternion.identity);
            lastPuff = (lastPuff + 1) % maxPuffs;
            puffs[lastPuff] = newPuffObject;
            UpdateBounds(newPuffObject.transform.position);
            UpdateMinDist(newPuffObject.transform.position);
            if (onPuffAdded != null) {
                onPuffAdded(newPuffObject);
            }
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

    // Tracks the minimum distance between subsequent puffs. This is used by GetNearbyPuffs to
    // speed up iteration.
    void UpdateMinDist(Vector3 pos) {
        if (NumPuffs > 1) {
            minPuffDist = Mathf.Min(minPuffDist, Vector3.Distance(pos, prevPos));
        }
        prevPos = pos;
    }

    // Returns the index for the puff at the pointed position in the array.
    // The index is 0 for the first puff and NumPuffs - 1 for the last one.
    int IndexOfPuff(int puffP) {
        if (NumPuffs < maxPuffs) {
            return puffP;
        }
        int index = NumPuffs - (lastPuff - puffP);
        return (puffP <= lastPuff) ? index : index - maxPuffs;
    }

    int PointerForPuff(int puffIndex) {
        if (NumPuffs < maxPuffs) {
            return puffIndex;
        } else {
            return (lastPuff + puffIndex) % maxPuffs;
        }
    }

    public IEnumerator<Puff> GetEnumerator() {
        if (NumPuffs == 0) {
            yield break;
        }

        int puffP = firstPuff;
        while (true) {
            yield return new Puff(puffs[puffP].transform.position, IndexOfPuff(puffP));
            if (puffP == lastPuff) {
                yield break;
            }

            if (++puffP == maxPuffs) {
                puffP = 0;
            }
        }
    }

    public IEnumerator<Puff> GetNearbyPuffs(Vector3 pos, float maxDist, int fromIndex) {
        if (fromIndex >= NumPuffs) {
            yield break;
        }

        // Pointer, but does not wrap (so facilitate end of loop detection)
        int puffP = PointerForPuff(fromIndex);
        Debug.Assert(IndexOfPuff(puffP) == fromIndex, "Mismatch");

        // Maximum pointer. Never smaller than initial pointer
        int maxPuffP = PointerForPuff(lastPuff);
        if (maxPuffP < puffP) {
            maxPuffP += maxPuffs;
        }

        while (true) {
            Vector3 puffPos = puffs[puffP % maxPuffs].transform.position;
            float dist = Vector3.Distance(pos, puffPos);
            int delta = 1;

            if (dist <= maxDist) {
                yield return new Puff(puffPos, IndexOfPuff(puffP % maxPuffs));
            } else {
                delta = Mathf.Max(1, Mathf.FloorToInt((dist - maxDist) / minPuffDist));
            }

            puffP += delta;

            if (puffP > maxPuffP) {
                yield break;
            }
        }
    }
}
