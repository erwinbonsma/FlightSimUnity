using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

enum CrossingHeight {
    Level = 0, // Both crossings are of equal height
    Underneath = 1, // First crossing goes under the next
    Overhead = 2, // First crossing goes above the next
    Unknown = 3
}

enum CrossingDirection {
    Left = 0,
    Right = 1,
    Unknown = 2
}

class TrailCrossing {
    public int CrossingNumber1 { get; private set; }
    public int CrossingNumber2 { get; set; }

    public CrossingHeight Height { get; set; }
    public CrossingDirection Direction { get; set; }

    public TrailCrossing(int crossingNumber) {
        CrossingNumber1 = crossingNumber;
        CrossingNumber2 = -1;
        Height = CrossingHeight.Unknown;
        Direction = CrossingDirection.Unknown;
    }

    public String ToString(int maxCrossingNumber) {
        if (CrossingNumber1 > maxCrossingNumber) {
            return "";
        }

        var sb = new StringBuilder();

        sb.Append(CrossingNumber1.ToString());
        sb.Append(",");
        if (CrossingNumber2 >= 0 && CrossingNumber2 <= maxCrossingNumber) {
            sb.Append(CrossingNumber2.ToString());
        } else {
            sb.Append("?");
        }
        sb.Append("LR"[(int)Direction]);
        if (Height != CrossingHeight.Level) {
            sb.Append("=UO?"[(int)Height]);
        }

        return sb.ToString();
    }

    public override String ToString() {
        return ToString(int.MaxValue);
    }
}

class SegmentCrossing {
    // The indices to the first (puff) Index of a line segment. For trail
    public int Index1 { get; private set; }
    public int Index2 { get; private set; }

    public TrailCrossing TrailCrossing { get; private set; }

    public SegmentCrossing(int index1, int index2, TrailCrossing trailCrossing) {
        Index1 = index1;
        Index2 = index2;
        TrailCrossing = trailCrossing;
    }

    public bool IsUnder(int index) {
        if (index == Index1) {
            return TrailCrossing.Height == CrossingHeight.Underneath;
        }
        Debug.Assert(index == Index2);
        return TrailCrossing.Height == CrossingHeight.Overhead;
    }

    public bool IsFirst(int index) {
        return index == Index1;
    }

    public override String ToString() {
        var sb = new StringBuilder();

        sb.Append(Index1.ToString());
        sb.Append("-");
        sb.Append(Index2.ToString());

        return sb.ToString();
    }
}

public class MissionReport : MonoBehaviour {

    public GameObject fuelMeter;
    public GameObject trailManagerObject;
    public GameObject hud;

    public GameObject linkPrefab;
    public GameObject overlappingLinkPrefab;

    public float minHeightDelta = 0.5f;

    GameObject player;
    GameObject gameControl;

    TextMeshProUGUI actualText;

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

        var challenge = GameState.ActiveChallenge;
        if (challenge != null) {
            hud.SetActive(true);
            var panelTransform = hud.transform.Find("Panel");
            var targetTransform = panelTransform.transform.Find("Target");
            var targetText = targetTransform.gameObject.GetComponent<TextMeshProUGUI>();
            targetText.text = challenge.Goal;

            var actualTransform = panelTransform.transform.Find("Actual");
            actualText = actualTransform.gameObject.GetComponent<TextMeshProUGUI>();
            actualText.text = "";
        }

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
                bool drawSegment = true;
                if (segmentCrossings.ContainsKey(prevPuff.Index)) {
                    var segmentCrossing = segmentCrossings[prevPuff.Index];

                    if (segmentCrossing.IsUnder(prevPuff.Index)) {
                        drawSegment = false;
                    }

                    int maxCrossingNumber = segmentCrossing.IsFirst(prevPuff.Index)
                        ? segmentCrossing.TrailCrossing.CrossingNumber1
                        : segmentCrossing.TrailCrossing.CrossingNumber2;
                    actualText.text = ActualCrossingString(maxCrossingNumber);
                }

                if (drawSegment) {
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

    CrossingHeight ClassifyCrossingHeight(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2) {
        Vector2 p1_2d = new Vector2(p1.x, p1.z), p2_2d = new Vector2(p2.x, p2.z);
        Vector2 q1_2d = new Vector2(q1.x, q1.z), q2_2d = new Vector2(q2.x, q2.z);

        Vector2 intersection_2d = MathUtil.IntersectionPoint(p1_2d, p2_2d, q1_2d, q2_2d);

        Vector3 intersection_p = Vector3.LerpUnclamped(
            p1, p2, MathUtil.RelativeDistance(p1_2d, p2_2d, intersection_2d)
        );
        Vector3 intersection_q = Vector3.LerpUnclamped(
            q1, q2, MathUtil.RelativeDistance(q1_2d, q2_2d, intersection_2d)
        );

        if (Mathf.Abs(intersection_p.y - intersection_q.y) < minHeightDelta) {
            return CrossingHeight.Level;
        }
        if (intersection_p.y < intersection_q.y) {
            return CrossingHeight.Underneath;
        } else {
            return CrossingHeight.Overhead;
        }
    }

    void RegisterSegmentCrossing(int segmentIndex, SegmentCrossing crossing) {
        if (!segmentCrossings.ContainsKey(segmentIndex)) {
            segmentCrossings[segmentIndex] = crossing;
        } else {
            if (segmentCrossings[segmentIndex].TrailCrossing != crossing.TrailCrossing) {
                overlaps.Add(segmentIndex);
            }
        }
    }

    void RegisterCrossing(Puff p1, Puff p2, Puff q1, Puff q2, IntersectionResult result) {
        Debug.Log("Crossing between " + p1.Index + " and " + q1.Index);

        var trailCrossing = TrailCrossingFor(p1, q1);

        if (trailCrossing.Height == CrossingHeight.Unknown) {
            trailCrossing.Height = ClassifyCrossingHeight(
                p1.Position, p2.Position, q1.Position, q2.Position
            );
        }
        if (trailCrossing.Direction == CrossingDirection.Unknown) {
            trailCrossing.Direction = (result == IntersectionResult.IntersectingFromRight)
                ? CrossingDirection.Right
                : CrossingDirection.Left;
        }

        var crossing = new SegmentCrossing(p1.Index, q1.Index, trailCrossing);
        RegisterSegmentCrossing(p1.Index, crossing);
        RegisterSegmentCrossing(q1.Index, crossing);
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
                Vector2 midP = (pos_p0 + pos_p1) / 2;

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

    string ActualCrossingString(int maxCrossingNumber) {
        var sb = new StringBuilder();

        foreach (var crossing in trailCrossings) {
            if (sb.Length > 0) {
                sb.Append(" ");
            }
            sb.Append(crossing.ToString(maxCrossingNumber));
        }

        return sb.ToString();
    }
}
