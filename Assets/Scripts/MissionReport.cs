using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum CrossingType {
    LeftUnder = 0,
    LeftOver = 1,
    RightUnder = 2,
    RightOver = 3,
    Unknown = 4
}

class TrailCrossing {
    public int CrossingNumber1 { get; private set; }
    public int CrossingNumber2 { get; set; }

    public CrossingType Type { get; set; }

    public TrailCrossing(int crossingNumber) {
        CrossingNumber1 = crossingNumber;
        CrossingNumber2 = -1;
        Type = CrossingType.Unknown;
    }

    public override String ToString() {
        var sb = new StringBuilder();

        sb.Append(CrossingNumber1.ToString());
        sb.Append(",");
        sb.Append(CrossingNumber2.ToString());
        sb.Append("lLrR?"[(int)Type]);

        return sb.ToString();
    }
}

class SegmentCrossing {
    // The indices to the first (puff) index of a line segment. For trail
    public int Index1 { get; private set; }
    public int Index2 { get; private set; }

    public TrailCrossing TrailCrossing { get; private set; }

    public SegmentCrossing(int index1, int index2, TrailCrossing trailCrossing) {
        Index1 = index1;
        Index2 = index2;
        TrailCrossing = trailCrossing;
    }
}

public class MissionReport : MonoBehaviour {

    public GameObject fuelMeter;
    public GameObject trailManagerObject;
    public GameObject linkPrefab;
    public GameObject overlappingLinkPrefab;

    GameObject player;
    GameObject gameControl;

    Dictionary<int, SegmentCrossing> segmentCrossings = new Dictionary<int, SegmentCrossing>();
    int crossingCount = 0;
    List<TrailCrossing> trailCrossings = new List<TrailCrossing>();
    HashSet<int> overlaps = new HashSet<int>();

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
        DumpCrossings();

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
                if (!segmentCrossings.ContainsKey(prevPuff.Index)) {
                    ConnectPuffs(prevPuff, puff, overlaps.Contains(prevPuff.Index));
                }

                yield return new WaitForSeconds( 0.1f );
            }
            prevPuff = puff;
        }
    }

    TrailCrossing TrailCrossingFor(Puff p1, Puff q1) {
        TrailCrossing trailCrossing = null;

        // Check if a trail crossing already exists for this segment crossing
        if (segmentCrossings.ContainsKey(p1.Index - 1)) {
            if (Math.Abs(segmentCrossings[p1.Index - 1].Index2 - q1.Index) <= 1) {
                trailCrossing = segmentCrossings[p1.Index - 1].TrailCrossing;
            }
        }
        if (segmentCrossings.ContainsKey(q1.Index - 1)) {
            if (Math.Abs(segmentCrossings[q1.Index - 1].Index1 - p1.Index) <= 1) {
                if (trailCrossing != null) {
                    Debug.Assert(segmentCrossings[q1.Index - 1].TrailCrossing == trailCrossing);
                } else {
                    trailCrossing = segmentCrossings[q1.Index - 1].TrailCrossing;
                }
            }
        }

        if (trailCrossing == null) {
            trailCrossing = new TrailCrossing(++crossingCount);
            trailCrossings.Add(trailCrossing);
        }

        return trailCrossing;
    }

    CrossingType ClassifyCrossing(Puff p1, Puff p2, Puff q1, Puff q2, IntersectionResult result) {
        // TODO: Check which one is on top
        bool goesUnder = true;

        CrossingType type;
        if (result == IntersectionResult.IntersectingFromRight) {
            type = goesUnder ? CrossingType.RightUnder : CrossingType.RightOver;
        } else {
            type = goesUnder ? CrossingType.LeftUnder : CrossingType.LeftOver;
        }

        return type;
    }

    void RegisterCrossing(Puff p1, Puff p2, Puff q1, Puff q2, IntersectionResult result) {
        Debug.Log("Crossing between " + p1.Index + " and " + q1.Index);

        var trailCrossing = TrailCrossingFor(p1, q1);

        if (trailCrossing.Type == CrossingType.Unknown) {
            trailCrossing.Type = ClassifyCrossing(p1, p2, q1, q2, result);
        }

        var crossing = new SegmentCrossing(p1.Index, q1.Index, trailCrossing);
        if (!segmentCrossings.ContainsKey(p1.Index)) {
            segmentCrossings[p1.Index] = crossing;
        } else {
            overlaps.Add(p1.Index);
        }
        if (!segmentCrossings.ContainsKey(q1.Index)) {
            segmentCrossings[q1.Index] = crossing;
        } else {
            overlaps.Add(q1.Index);
        }
    }

    void RegisterOverlap(int index1, int index2) {
        Debug.Log("Overlap between " + index1 + " and " + index2);
        overlaps.Add(index1);
        overlaps.Add(index2);
    }

    void IdentifyCrossings(TrailManager trailManager) {
        float dist = trailManager.MinPuffDistance * 1.5f;

        Puff p0 = null;
        Vector2 pos_p0 = Vector2.zero; // To silence compiler warning
        foreach (Puff p1 in trailManager) {
            Vector2 pos_p1 = new Vector2(p1.Position.x, p1.Position.z);

            // Check if this segment is part of an already identified crossing. If so, then
            // update its crossing number. This can be done now that the number of crossings up
            // till now is known. Note, we still should check for other crossings to signal
            // possible (undesired) overlaps.
            if (segmentCrossings.ContainsKey(p1.Index)) {
                var trailCrossing = segmentCrossings[p1.Index].TrailCrossing;
                if (trailCrossing.CrossingNumber2 < 0) {
                    trailCrossing.CrossingNumber2 = ++crossingCount;
                }
            }

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

    void DumpCrossings() {
        var sb = new StringBuilder();
        foreach (var crossing in trailCrossings) {
            if (sb.Length > 0) {
                sb.Append(" ");
            }
            sb.Append(crossing.ToString());
        }
        Debug.Log(sb);
    }
}
