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

    public float turnSpeed = 0.1f;

    public float updateEps = 0.05f;

    public float speed;

    public event Action<PlayerMovement> onTargetDirectionChange;
    public event Action<PlayerMovement> onDirectionChange;
    public event Action<PlayerMovement> onRollChange;

    public float SideTurnAmount {
        get { return sideTurn / numSideTurnSteps; }
    }

    public float TargetSideTurnAmount {
        get { return targetSideTurn / (float)numSideTurnSteps; }
    }
    public float TargetUpTurnAmount {
        get { return targetUpTurn / (float)numUpTurnSteps; }
    }
    public float TargetRollTurnAmount {
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
    float rollTurn;

    // Fires when the steering controls changed (based on player input)
    void FireTargetDirectionChange() {
        if (onTargetDirectionChange != null) {
            onTargetDirectionChange(this);
        }
    }

    // Fires when the direction delta vector changed. This happens for several updates after a
    // change of target direction, due to inertia.
    //
    // Note: When this stops firing, the player may still be changing direction. This happens when
    // the delta direction vector it fixed but non-zero.
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
                FireTargetDirectionChange();
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            if (targetSideTurn < numSideTurnSteps) {
                targetSideTurn++;
                FireTargetDirectionChange();
            }
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            if (targetUpTurn < numUpTurnSteps) {
                targetUpTurn++;
                FireTargetDirectionChange();
            }
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            if (targetUpTurn >= 0) {
                targetUpTurn--;
                FireTargetDirectionChange();
            }
        }
    }

    void UpdateRotation() {
        float turnAngle;
        bool directionChanged = false;

        if (Math.Abs(targetSideTurn - sideTurn) > updateEps) {
            sideTurn = Mathf.Lerp(sideTurn, targetSideTurn, turnSpeed);
            directionChanged = true;
        }
        if (Math.Abs(targetUpTurn - upTurn) > updateEps) {
            upTurn = Mathf.Lerp(upTurn, targetUpTurn, turnSpeed);
            directionChanged = true;
        }
        if (Math.Abs(targetRollTurn - rollTurn) > updateEps) {
            rollTurn = Mathf.Lerp(rollTurn, targetRollTurn, turnSpeed);
            directionChanged = true;
        }

        if (directionChanged) {
            FireDirectionChange();
        }

        turnAngle = sideTurn * maxSideTurnAngle / numSideTurnSteps;
        transform.RotateAround(transform.position, transform.up, turnAngle);

        turnAngle = upTurn * maxUpTurnAngle / numUpTurnSteps;
        transform.RotateAround(transform.position, transform.right, -turnAngle);

        turnAngle = rollTurn * maxRollTurnAngle / numRollTurnSteps;
        transform.RotateAround(transform.position, transform.forward, -turnAngle);
    }

    void FixedUpdate() {
        ProcessInput();
        UpdateRotation();

        transform.position += transform.forward * Time.deltaTime * speed;
    }
}
