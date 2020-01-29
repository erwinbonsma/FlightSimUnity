using UnityEngine;

enum Orientation {
    Colinear,
    Clockwise,
    CounterClockwise
}

public enum IntersectionResult {
    NotTouching,
    IntersectingFromRight,
    IntersectingFromLeft,
    Overlapping
}

public class MathUtil {

    const float eps = 0.0001f;

    // Don't be precise when considering line segments co-lineair (we want "nice" crossings)
    const float orientationEps = 0.1f;

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

        if (Mathf.Abs(val) < orientationEps) {
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
            // Account for one of the orientations being co-lineair
            if (o1 == Orientation.Clockwise || o2 == Orientation.CounterClockwise) {
                return IntersectionResult.IntersectingFromRight;
            } else {
                return IntersectionResult.IntersectingFromLeft;
            }
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

    // Returns the intersection point between the line going through p1 and q1 and the line
    // passing through p2 and q2.
    public static Vector2 IntersectionPoint(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2) {
        // See https://www.geeksforgeeks.org/program-for-point-of-intersection-of-two-lines/

        // Line p1-q1 represented as a1 * x + b1 * y = c1
        float a1 = q1.y - p1.y;
        float b1 = p1.x - q1.x;
        float c1 = a1 * (p1.x) + b1 * (p1.y);

        // Line p2-q2 represented as a2 * x + b2 * y = c2
        float a2 = q2.y - p2.y;
        float b2 = p2.x - q2.x;
        float c2 = a2 * (p2.x) + b2 * (p2.y);

        float determinant = a1 * b2 - a2 * b1;
        if (Mathf.Abs(determinant) == 0) {
            return Vector3.positiveInfinity;
        }

        float x = (b2 * c1 - b1 * c2) / determinant;
        float y = (a1 * c2 - a2 * c1) / determinant;
        //Debug.Log("Intersection of " + p1 + " - " + q1 + " with " + p2 + " - " + q2 + " => " + new Vector2(x, y));
        return new Vector2(x, y);
    }

    // Returns the relative distance of R with respect to P and Q. It assumes all three points
    // are co-lineair. It returns 0 when R equals P and 1 when R equals Q. It returns values
    // inbetween when R is somewhere on the line segment PQ.
    public static float RelativeDistance(Vector2 p, Vector2 q, Vector2 r) {
        float d = Vector2.Distance(p, q);
        float dp = Vector2.Distance(p, r);
        float dq = Vector2.Distance(q, r);
        //Debug.Log("d = " + d + ", dp = " + dp + ", dq = " + dq);

        if (Mathf.Abs(d - dp - dq) < eps) {
            // R lies on segment PQ
            return dp / d;
        }

        if (dp < dq) {
            Debug.Assert(Mathf.Abs(dq - d - dp) < eps);
            // R lies nearer to P, so return a negative value
            return -dp / d;
        } else {
            Debug.Assert(Mathf.Abs(dp - d - dq) < eps);
            // R lies neader to Q, so return a value larger than one
            return dp / d;
        }
    }
}