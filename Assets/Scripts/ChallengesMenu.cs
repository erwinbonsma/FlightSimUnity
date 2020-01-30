using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChallengesMenu : MonoBehaviour {

    public Button[] buttons;

    void Start() {
        foreach (var button in buttons) {
            button.onClick.AddListener(delegate { OnButtonClick(button); });
        }
    }

    void OnButtonClick(Button button) {
        var textTransform = button.transform.Find("Title");
        var text = textTransform.gameObject.GetComponent<TextMeshProUGUI>();

        var imageTransform = button.transform.Find("Image");
        var image = imageTransform.gameObject.GetComponent<Image>();

        var goal = button.GetComponent<ChallengeGoal>();

        var challenge = new Challenge(text.text, image.sprite, goal.goal);
        Debug.Log("Starting challenge " + challenge);

        GameState.ActiveChallenge = challenge;
        StartCoroutine(UnityUtil.AsyncLoadScene("StartChallengeScreen"));
    }
}
