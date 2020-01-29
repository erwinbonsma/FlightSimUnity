using System.Text;
using UnityEngine;

public class Challenge {
    public string Name { get; private set; }
    public string Goal { get; private set; }
    public Sprite Sprite { get; private set; }

    public Challenge(string name, Sprite sprite, string goal) {
        Name = name;
        Sprite = sprite;
        Goal = goal;
    }

    public override string ToString() {
        var sb = new StringBuilder();

        sb.Append(Name);
        sb.Append(", goal = ");
        sb.Append(Goal);
        sb.Append(", image = ");
        sb.Append(Sprite.name);

        return sb.ToString();
    }
}