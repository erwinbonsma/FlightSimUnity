using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientPlayer : MonoBehaviour {

    public float maxAngle = 20;

    PlayerMovement playerMovement;

    void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.onDirectionChange += onDirectionChange;
    }

    void onDirectionChange(PlayerMovement playerMovement) {
        float angle = maxAngle * -playerMovement.SideTurnAmount;

        Debug.Log("angle = " + angle);
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
