using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientStick : MonoBehaviour {

    public float maxAngle = 20.0f;

    PlayerMovement playerMovement;

    void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.onTargetDirectionChange += onTargetDirectionChange;
    }

    void onTargetDirectionChange(PlayerMovement playerMovement) {
        float angleZ = maxAngle * -playerMovement.TargetSideTurnAmount;
        float angleX = maxAngle * -playerMovement.TargetUpTurnAmount;

        transform.localEulerAngles = new Vector3(angleX, 0, angleZ);
    }
}
