using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientRollTracker : MonoBehaviour {

    float maxAngle = 20.0f;

    PlayerMovement playerMovement;

    void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.onRollChange += onRollChange;
    }

    void onRollChange(PlayerMovement playerMovement) {
        float angle = maxAngle * -playerMovement.RollTurnAmount;

        transform.localEulerAngles = new Vector3(0, 0, angle);
    }

}
