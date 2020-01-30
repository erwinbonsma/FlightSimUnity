using UnityEngine;

public class HelpMenuController : MonoBehaviour {

    void Update() {
        if (Input.anyKey) {
            StartCoroutine(UnityUtil.AsyncLoadScene("MainMenu"));
        }
    }
}
