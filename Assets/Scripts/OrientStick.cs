using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientStick : MonoBehaviour {

    float maxAngle = 20.0f;

    PlayerMovement playerMovement;

    void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.onDirectionChange += onDirectionChange;
    }

    void onDirectionChange(PlayerMovement playerMovement) {
        float angleZ = maxAngle * -playerMovement.SideTurnAmount;
        float angleX = maxAngle * -playerMovement.UpTurnAmount;

        transform.localEulerAngles = new Vector3(angleX, 0, angleZ);
    }
}
