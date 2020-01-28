using UnityEngine;

enum Orientation {
    Colinear,
    Clockwise,
    CounterClockwise
}

public enum IntersectionResult {
    NotTouching,
    Intersecting,
    Overlapping
}

public class MathUtil {

    const float eps = 0.00001f;

    // Given three co-linear points, returns "true" if point q lies on line segment p-r
    static private bool OnSegment(Vector2 p, Vector2 q, Vector2 r) {
        return (
            q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
            q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y)
        );
    }

    static private Orientation GetOrientation(Vector2 p, Vector2 q, Vector2 r) {
        // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ for details.
        float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

        if (Mathf.Abs(val) < eps) {
            return Orientation.Colinear;
        }

        return (val > 0)? Orientation.Clockwise : Orientation.CounterClockwise;
    }

    // Checks whether line segment p1-q1 and p2-q2 intersect
    public static IntersectionResult DoLineSegmentsIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2) {
        // Find the four orientations needed for general and special cases
        var o1 = GetOrientation(p1, q1, p2);
        var o2 = GetOrientation(p1, q1, q2);
        var o3 = GetOrientation(p2, q2, p1);
        var o4 = GetOrientation(p2, q2, q1);

        // General case
        if (o1 != o2 && o3 != o4) {
            return IntersectionResult.Intersecting;
        }

        // Special Cases
        if (
            o1 == Orientation.Colinear && OnSegment(p1, p2, q1) ||
            o2 == Orientation.Colinear && OnSegment(p1, q2, q1) ||
            o3 == Orientation.Colinear && OnSegment(p2, p1, q2) ||
            o4 == Orientation.Colinear && OnSegment(p2, q1, q2)
        ) {
            return IntersectionResult.Overlapping;
        }

        return IntersectionResult.NotTouching;
    }
}