using System.Text;
using UnityEngine;

public class Challenge {
    public string Name { get; private set; }
    public string Goal { get; private set; }
    public int MaxDuration { get; private set; }
    public Sprite Sprite { get; private set; }

    public Challenge(string name, Sprite sprite, string goal, int maxDuration) {
        Name = name;
        Sprite = sprite;
        Goal = goal;
        MaxDuration = maxDuration;
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