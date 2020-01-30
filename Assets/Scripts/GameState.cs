using System.Collections;
using System.Collections.Generic;

public static class GameState {
    public static Challenge ActiveChallenge { get; set; }
    public static List<Challenge> Challenges { get; private set; }

    static GameState() {
        Challenges = new List<Challenge>();
    }

    public static void NextChallenge() {
        if (ActiveChallenge == null) {
            ActiveChallenge = Challenges[0];
        } else {
            if (ActiveChallenge.Index + 1 <= Challenges.Count) {
                ActiveChallenge = Challenges[ActiveChallenge.Index + 1];
            } else {
                ActiveChallenge = null;
            }
        }
    }
}