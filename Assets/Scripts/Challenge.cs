using System.Text;
using UnityEngine;

public class Challenge {
    public int Index { get; private set; }
    public string Name { get; private set; }
    public string Goal { get; private set; }
    public int MaxDuration { get; private set; }
    public Sprite Sprite { get; private set; }
    public bool IsCompleted { get; private set; }

    static int numChallenges = 0;

    public Challenge(string name, Sprite sprite, string goal, int maxDuration) {
        Index = numChallenges++;
        Name = name;
        Sprite = sprite;
        Goal = goal;
        MaxDuration = maxDuration;
        IsCompleted = false;
    }

    public void MarkCompleted() {
        IsCompleted = true;
    }

    public override string ToString() {
        var sb = new StringBuilder();

        sb.Append(Name);
        sb.Append(", goal = ");
        sb.Append(Goal);
        sb.Append(", duration = ");
        sb.Append(MaxDuration);
        sb.Append(", image = ");
        sb.Append(Sprite.name);

        return sb.ToString();
    }
}