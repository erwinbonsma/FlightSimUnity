using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public int numSideTurnSteps;
    public int numUpTurnSteps;
    public int numRollTurnSteps;

    public float maxSideTurnAngle;
    public float maxUpTurnAngle;
    public float maxRollTurnAngle;

    public float turnSpeed = 0.4f;

    public float speed;

    public event Action<PlayerMovement> onDirectionChange;
    public event Action<PlayerMovement> onRollChange;

    public float SideTurnAmount {
        get { return targetSideTurn / (float)numSideTurnSteps; }
    }
    public float UpTurnAmount {
        get { return targetUpTurn / (float)numUpTurnSteps; }
    }
    public float RollTurnAmount {
        get { return targetRollTurn / (float)numRollTurnSteps; }
    }

    // Ranges from [-numSideTurnSteps, numSideTurnSteps]
    int targetSideTurn;
    float sideTurn;

    // Ranges from [-1, maxUpTurnSteps]
    int targetUpTurn;
    float upTurn;

    // Ranges from [-1, 1]
    int targetRollTurn;
    float RollTurn;

    void FireDirectionChange() {
        if (onDirectionChange != null) {
            onDirectionChange(this);
        }
    }

    void FireRollChange() {
        if (onRollChange != null) {
            onRollChange(this);
        }
    }

    void ProcessInput() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (targetRollTurn > -numRollTurnSteps) {
                targetRollTurn--;
                FireRollChange();
            }
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            if (targetRollTurn < numRollTurnSteps) {
                targetRollTurn++;
                FireRollChange();
            }
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            if (targetSideTurn > -numSideTurnSteps) {
                targetSideTurn--;
                FireDirectionChange();
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            if (targetSideTurn < numSideTurnSteps) {
                targetSideTurn++;
                FireDirectionChange();
            }
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            if (targetUpTurn < numUpTurnSteps) {
                targetUpTurn++;
                FireDirectionChange();
            }
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            if (targetUpTurn >= 0) {
                targetUpTurn--;
                FireDirectionChange();
            }
        }
    }

    void UpdateRotation() {
        float turnAngle;

        sideTurn = Mathf.Lerp(sideTurn, targetSideTurn, turnSpeed);
        upTurn = Mathf.Lerp(upTurn, targetUpTurn, turnSpeed);
        RollTurn = Mathf.Lerp(RollTurn, targetRollTurn, turnSpeed);

        turnAngle = sideTurn * maxSideTurnAngle / numSideTurnSteps;
        transform.RotateAround(transform.position, transform.up, turnAngle);

        turnAngle = upTurn * maxUpTurnAngle / numUpTurnSteps;
        transform.RotateAround(transform.position, transform.right, -turnAngle);

        turnAngle = RollTurn * maxRollTurnAngle / numRollTurnSteps;
        transform.RotateAround(transform.position, transform.forward, -turnAngle);
    }

    void FixedUpdate() {
        ProcessInput();
        UpdateRotation();

        transform.position += transform.forward * Time.deltaTime * speed;
    }
}
