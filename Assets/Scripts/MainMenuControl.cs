using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControl : MonoBehaviour {

    public void StartPractice() {
        GameState.ActiveChallenge = null;
        StartCoroutine(UnityUtil.AsyncLoadScene("SampleScene"));
    }

    public void StartChallenge() {
        StartCoroutine(UnityUtil.AsyncLoadScene("ChallengesScreen"));
    }

    public void ShowHelp() {
        StartCoroutine(UnityUtil.AsyncLoadScene("HelpScreen"));
    }
}
