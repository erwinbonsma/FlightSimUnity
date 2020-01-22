using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public int numSideTurnSteps;
    public int numUpTurnSteps;

    public float maxSideTurnAngle;
    public float maxUpTurnAngle;
    public float maxTwistTurnAngle;

    public float speed;

    // Ranges from [-numSideTurnSteps, numSideTurnSteps]
    int sideTurn;

    // Ranges from [-1, maxUpTurnSteps]
    int upTurn;

    // Ranges from [-1, 1]
    int twistTurn;

    void ProcessInput() {
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKeyDown(KeyCode.A)) {
            if (shiftPressed) {
                if (twistTurn >= 0) {
                    twistTurn--;
                }
            } else {
                if (sideTurn > -numSideTurnSteps) {
                    sideTurn--;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            if (shiftPressed) {
                if (twistTurn <= 0) {
                    twistTurn++;
                }
            } else {
                if (sideTurn < numSideTurnSteps) {
                    sideTurn++;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.W) && upTurn < numUpTurnSteps) {
            upTurn++;
        }
        if (Input.GetKeyDown(KeyCode.S) && upTurn >= 0) {
            upTurn--;
        }
    }

    void UpdateRotation() {
        if (sideTurn != 0) {
            float turnAngle = sideTurn * maxSideTurnAngle / numSideTurnSteps;
            transform.RotateAround(transform.position, transform.up, turnAngle);
            Debug.Log("sideTurn = " + sideTurn);
        }
        if (upTurn != 0) {
            float turnAngle = upTurn * maxUpTurnAngle / numUpTurnSteps;
            transform.RotateAround(transform.position, transform.right, -turnAngle);
            Debug.Log("upTurn = " + upTurn);
        }
        if (twistTurn != 0) {
            float turnAngle = twistTurn * maxTwistTurnAngle;
            transform.RotateAround(transform.position, transform.forward, -turnAngle);
            Debug.Log("twistTurn = " + twistTurn);
        }
    }

    void FixedUpdate() {
        ProcessInput();
        UpdateRotation();

        transform.position += transform.forward * Time.deltaTime * speed;
    }
}
