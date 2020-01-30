using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class UnityUtil {
    public static IEnumerator AsyncLoadScene(string sceneName) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}