using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionReport : MonoBehaviour {

    public GameObject fuelMeter;
    public GameObject trailManagerObject;
    public GameObject linkPrefab;

    GameObject player;
    GameObject gameControl;

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

        StartCoroutine(DrawTrail(trailManager));

        IdentifyCrossings(trailManager);

        //SceneManager.LoadSceneAsync("MainMenu");
    }

    void ConnectPuffs(Puff p1, Puff p2) {
        GameObject newPuffObject = Instantiate(linkPrefab, p1.Position, Quaternion.identity);

        float dist = Vector3.Distance(p1.Position, p2.Position);
        newPuffObject.transform.localScale = new Vector3(1, 1, dist);

        Quaternion rotation = Quaternion.LookRotation(p2.Position - p1.Position);
        newPuffObject.transform.rotation = rotation;
    }

    IEnumerator DrawTrail(TrailManager trailManager) {
        Puff prevPuff = null;

        foreach (Puff puff in trailManager) {
            if (prevPuff != null) {
                ConnectPuffs(prevPuff, puff);

                yield return new WaitForSeconds( 0.1f );
            }
            prevPuff = puff;
        }
    }

    void IdentifyCrossings(TrailManager trailManager) {
        float dist = trailManager.MinPuffDistance * 1.5f;

        Puff p0 = null;
        Vector2 pos_p0 = Vector2.zero; // To silence compiler warning
        foreach (Puff p1 in trailManager) {
            Vector2 pos_p1 = new Vector2(p1.Position.x, p1.Position.z);
            if (p0 != null) {
                Vector3 midP = (p1.Position + p0.Position) / 2;

                int q0_index = -1;
                Vector2 pos_q0 = Vector2.zero; // To silence compiler warning

                var nearbyEnum = trailManager.GetNearbyPuffs(midP, dist, p1.Index + 1);
                while (nearbyEnum.MoveNext()) {
                    Puff q1 = nearbyEnum.Current;
                    Vector2 pos_q1 = new Vector2(q1.Position.x, q1.Position.z);
                    if (q0_index + 1 == q1.Index) {
                        IntersectionResult result = MathUtil.DoLineSegmentsIntersect(
                            pos_p0, pos_p1, pos_q0, pos_q1
                        );
                        if (result != IntersectionResult.NotTouching) {
                            if (result == IntersectionResult.Intersecting) {
                                Debug.Log("Intersection between " + (p1.Index - 1) + " and " + q0_index);
                            } else {
                                Debug.Log("Overlap between " + (p1.Index - 1) + " and " + q0_index);
                            }
                        }
                    }
                    q0_index = q1.Index;
                    pos_q0 = pos_q1;
                }
            }
            p0 = p1;
            pos_p0 = pos_p1;
        }
    }
}
