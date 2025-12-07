using UnityEngine;

[ExecuteAlways]
public class SplinePathGizmo : MonoBehaviour
{
    public SplinePath spline;
    public Color lineColor = Color.cyan;
    public int resolution = 200;

    private void OnDrawGizmos()
    {
        if (spline == null || spline.controlPoints == null || spline.controlPoints.Count < 2)
            return;

        Gizmos.color = lineColor;

        Vector3 prevPos = spline.GetPoint(0f);

        float maxT = spline.controlPoints.Count - 1;

        for (int i = 1; i <= resolution; i++)
        {
            float t = Mathf.Lerp(0, maxT, i / (float)resolution);
            Vector3 currPos = spline.GetPoint(t);

            Gizmos.DrawLine(prevPos, currPos);
            prevPos = currPos;
        }

        // Control points 표시
        Gizmos.color = Color.yellow;
        foreach (var p in spline.controlPoints)
        {
            Gizmos.DrawSphere(p, 0.1f);
        }
    }
}