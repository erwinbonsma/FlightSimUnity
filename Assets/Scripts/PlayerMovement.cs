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

    const float eps = 0.0001f;
    const float eps2 = 0.01f;

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

    bool UpdateIfNeeded(ref float value, float targetValue) {
        float delta = Math.Abs(targetValue - value);

        if (delta < eps) {
            // No update needed
            return false;
        }

        if (delta > eps2) {
            value = Mathf.Lerp(value, targetValue, turnSpeed);
        } else {
            value = targetValue;
        }

        return true;
    }

    void UpdateRotation() {
        float turnAngle;
        bool directionChanged = false;

        directionChanged |= UpdateIfNeeded(ref sideTurn, targetSideTurn);
        directionChanged |= UpdateIfNeeded(ref upTurn, targetUpTurn);
        directionChanged |= UpdateIfNeeded(ref rollTurn, targetRollTurn);

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
