using System.Collections;
using UnityEngine;

public class TitleScreenController : MonoBehaviour {

    void Start() {
        StartCoroutine(WaitThenShowMainMenu());
    }

    void Update() {
        if (Input.anyKey) {
            ShowMainMenu();
        }
    }

    IEnumerator WaitThenShowMainMenu() {
        yield return new WaitForSeconds(4f);
        ShowMainMenu();
    }

    void ShowMainMenu() {
        if (enabled) {
            StartCoroutine(UnityUtil.AsyncLoadScene("MainMenu"));
            enabled = false;
        }
    }
}
