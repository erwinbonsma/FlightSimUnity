using UnityEngine;

public class Puff {
    Vector3 _pos;

    public Vector3 Position { get { return _pos; } }
    public int Index { get; private set; }

    public Puff(Vector3 pos, int index) {
        _pos = pos;
        Index = index;
    }
}