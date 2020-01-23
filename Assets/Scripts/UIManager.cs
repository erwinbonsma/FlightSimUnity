using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public void StartGame() {
        Debug.Log("Starting Game");

        StartCoroutine(AsyncLoadScene("SampleScene"));
    }

    IEnumerator AsyncLoadScene(string sceneName) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}
