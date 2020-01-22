using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public int numSideTurnSteps;
    public float maxSideTurnAngle;
    public float speed;

    // Ranges from [-numSideTurnSteps, numSideTurnSteps]
    int sideTurn = 0;

    // Start is called before the first frame update
    void Start() {
    }

    void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.A) && sideTurn > -numSideTurnSteps) {
            sideTurn--;
            Debug.Log("sideTurn = " + sideTurn);
        }
        if (Input.GetKeyDown(KeyCode.D) && sideTurn < numSideTurnSteps) {
            sideTurn++;
            Debug.Log("sideTurn = " + sideTurn);
        }

        if (sideTurn != 0) {
            float turnAngle = sideTurn * maxSideTurnAngle;

            Vector3 rotV = transform.localEulerAngles;
            rotV.y += turnAngle * Time.deltaTime;
            transform.localEulerAngles = rotV;
        }
        // Debug.Log("forward = " + transform.forward);

        transform.position += transform.forward * Time.deltaTime * speed;
    }
}
