using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum CrossingType {
    LeftUnder,
    LeftOver,
    RightUnder,
    RightOver
}

class TrailCrossing {
    public int Index1 { get; private set; }
    public int Index2 { get; private set; }
    public CrossingType Type { get; private set; }

    public TrailCrossing(int index1, int index2, CrossingType type) {
        Index1 = index1;
        Index2 = index2;
        Type = type;
    }
}

public class MissionReport : MonoBehaviour {

    public GameObject fuelMeter;
    public GameObject trailManagerObject;
    public GameObject linkPrefab;
    public GameObject overlappingLinkPrefab;

    GameObject player;
    GameObject gameControl;

    Dictionary<int, TrailCrossing> crossings = new Dictionary<int, TrailCrossing>();
    HashSet<int> overlap = new HashSet<int>();

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        gameControl = GameObject.FindGameObjectWithTag("GameController");

        fuelMeter.GetComponent<FuelMeter>().onFuelEmpty += OnFuelEmpty;
    }

    void OnFuelEmpty(FuelMeter fuelMeter) {
        Destroy(player);
        TrailManager trailManager = trailManagerObject.GetComponent<TrailManager>();
        trailManager.Enabled = false;

        CameraControl cameraControl = gameControl.GetComponent<CameraControl>();
        cameraControl.EnableOverheadCamera();

        IdentifyCrossings(trailManager);

        StartCoroutine(DrawTrail(trailManager));

        //SceneManager.LoadSceneAsync("MainMenu");
    }

    void ConnectPuffs(Puff p1, Puff p2, bool overlaps) {
        GameObject prefab = overlaps ? overlappingLinkPrefab : linkPrefab;
        GameObject newPuffObject = Instantiate(prefab, p1.Position, Quaternion.identity);

        float dist = Vector3.Distance(p1.Position, p2.Position);
        newPuffObject.transform.localScale = new Vector3(1, 1, dist);

        Quaternion rotation = Quaternion.LookRotation(p2.Position - p1.Position);
        newPuffObject.transform.rotation = rotation;
    }

    IEnumerator DrawTrail(TrailManager trailManager) {
        Puff prevPuff = null;

        foreach (Puff puff in trailManager) {
            if (prevPuff != null) {
                if (!crossings.ContainsKey(prevPuff.Index)) {
                    ConnectPuffs(prevPuff, puff, overlap.Contains(prevPuff.Index));
                }

                yield return new WaitForSeconds( 0.1f );
            }
            prevPuff = puff;
        }
    }

    void RegisterCrossing(Puff p1, Puff p2, Puff q1, Puff q2, IntersectionResult result) {
        Debug.Log("Crossing between " + p1.Index + " and " + q1.Index);

        // TODO: Check which one is on top
        bool goesUnder = true;

        CrossingType type;
        if (result == IntersectionResult.IntersectingFromRight) {
            type = goesUnder ? CrossingType.RightUnder : CrossingType.RightOver;
        } else {
            type = goesUnder ? CrossingType.LeftUnder : CrossingType.LeftOver;
        }

        var crossing = new TrailCrossing(p1.Index, q1.Index, type);

        crossings[p1.Index] = crossing;
        crossings[q1.Index] = crossing;
    }

    void RegisterOverlap(int index1, int index2) {
        Debug.Log("Overlap between " + index1 + " and " + index2);
        overlap.Add(index1);
        overlap.Add(index2);
    }

    void IdentifyCrossings(TrailManager trailManager) {
        float dist = trailManager.MinPuffDistance * 1.5f;

        Puff p0 = null;
        Vector2 pos_p0 = Vector2.zero; // To silence compiler warning
        foreach (Puff p1 in trailManager) {
            Vector2 pos_p1 = new Vector2(p1.Position.x, p1.Position.z);
            if (p0 != null) {
                Vector3 midP = (p1.Position + p0.Position) / 2;

                Puff q0 = null;
                Vector2 pos_q0 = Vector2.zero; // To silence compiler warning

                var nearbyEnum = trailManager.GetNearbyPuffs(midP, dist, p1.Index + 1);
                while (nearbyEnum.MoveNext()) {
                    Puff q1 = nearbyEnum.Current;
                    Vector2 pos_q1 = new Vector2(q1.Position.x, q1.Position.z);
                    if (q0 != null && q0.Index + 1 == q1.Index) {
                        IntersectionResult result = MathUtil.DoLineSegmentsIntersect(
                            pos_p0, pos_p1, pos_q0, pos_q1
                        );
                        if (result != IntersectionResult.NotTouching) {
                            if (result == IntersectionResult.Overlapping) {
                                RegisterOverlap(p0.Index, q0.Index);
                            } else {
                                RegisterCrossing(p0, p1, q0, q1, result);
                            }
                        }
                    }
                    q0 = q1;
                    pos_q0 = pos_q1;
                }
            }
            p0 = p1;
            pos_p0 = pos_p1;
        }
    }
}
