using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartChallengeControl : MonoBehaviour {
    void Start() {
        Challenge challenge = GameState.ActiveChallenge;

        var textTransform = transform.Find("Header");
        var text = textTransform.gameObject.GetComponent<TextMeshProUGUI>();
        text.text = challenge.Name;

        var imageTransform = transform.Find("Image");
        var image = imageTransform.gameObject.GetComponent<Image>();
        image.sprite = challenge.Sprite;
    }

    void Update() {
        if (Input.anyKey) {
            StartCoroutine(UnityUtil.AsyncLoadScene("SampleScene"));
        }
    }
}
