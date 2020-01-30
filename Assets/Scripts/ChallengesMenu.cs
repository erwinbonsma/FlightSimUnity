using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChallengesMenu : MonoBehaviour {

    public Button[] buttons;
    public GameObject tickmarkPrefab;

    void Start() {
        bool initChallenges = GameState.Challenges.Count == 0;

        int index = 0;
        foreach (var button in buttons) {
            if (initChallenges) {
                GameState.Challenges.Add(ChallengeForButton(button));
            }

            if (GameState.Challenges[index].IsCompleted) {
                Instantiate(tickmarkPrefab, button.transform);
            }
            var challengeIndex = index++;
            button.onClick.AddListener(delegate { OnButtonClick(challengeIndex); });
        }
    }

    Challenge ChallengeForButton(Button button) {
        var textTransform = button.transform.Find("Title");
        var text = textTransform.gameObject.GetComponent<TextMeshProUGUI>();

        var imageTransform = button.transform.Find("Image");
        var image = imageTransform.gameObject.GetComponent<Image>();

        var spec = button.GetComponent<ChallengeSpec>();

        return new Challenge(text.text, image.sprite, spec.goal, spec.maxDuration);
    }

    void OnButtonClick(int challengeIndex) {
        GameState.ActiveChallenge = GameState.Challenges[challengeIndex];
        StartCoroutine(UnityUtil.AsyncLoadScene("StartChallengeScreen"));
    }
}
