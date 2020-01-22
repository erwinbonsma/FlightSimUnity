using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public int numSideTurnSteps;
    public int numUpTurnSteps;

    public float maxSideTurnAngle;
    public float maxUpTurnAngle;
    public float maxTwistTurnAngle;

    public float turnSpeed = 0.4f;

    public float speed;

    // Ranges from [-numSideTurnSteps, numSideTurnSteps]
    int targetSideTurn;
    float sideTurn;

    // Ranges from [-1, maxUpTurnSteps]
    int targetUpTurn;
    float upTurn;

    // Ranges from [-1, 1]
    int targetTwistTurn;
    float twistTurn;

    void ProcessInput() {
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKeyDown(KeyCode.A)) {
            if (shiftPressed) {
                if (targetTwistTurn >= 0) {
                    targetTwistTurn--;
                }
            } else {
                if (targetSideTurn > -numSideTurnSteps) {
                    targetSideTurn--;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            if (shiftPressed) {
                if (targetTwistTurn <= 0) {
                    targetTwistTurn++;
                }
            } else {
                if (targetSideTurn < numSideTurnSteps) {
                    targetSideTurn++;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            if (targetUpTurn < numUpTurnSteps) {
                targetUpTurn++;
            }
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            if (targetUpTurn >= 0) {
                targetUpTurn--;
            }
        }
    }

    void UpdateRotation() {
        float turnAngle;

        sideTurn = Mathf.Lerp(sideTurn, targetSideTurn, turnSpeed);
        upTurn = Mathf.Lerp(upTurn, targetUpTurn, turnSpeed);
        twistTurn = Mathf.Lerp(twistTurn, targetTwistTurn, turnSpeed);

        turnAngle = sideTurn * maxSideTurnAngle / numSideTurnSteps;
        transform.RotateAround(transform.position, transform.up, turnAngle);

        turnAngle = upTurn * maxUpTurnAngle / numUpTurnSteps;
        transform.RotateAround(transform.position, transform.right, -turnAngle);

        turnAngle = twistTurn * maxTwistTurnAngle;
        transform.RotateAround(transform.position, transform.forward, -turnAngle);
    }

    void FixedUpdate() {
        ProcessInput();
        UpdateRotation();

        transform.position += transform.forward * Time.deltaTime * speed;
    }
}
