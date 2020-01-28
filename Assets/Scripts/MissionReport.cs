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

}
