using System.Collections.Generic;
using UnityEngine;

public class SplinePath : MonoBehaviour
{
    public List<Vector3> controlPoints = new List<Vector3>();

    public Vector3 GetPoint(float t)
    {
        int count = controlPoints.Count;
        if (count == 0) return Vector3.zero;
        if (count == 1) return controlPoints[0];

        // 📌 t 를 "인덱스 공간"으로 사용: 0 ~ (count-1)
        //    0   → 0번 포인트
        //    1.0 → 1번 포인트
        //    1.5 → 1~2 사이 곡선
        //    ...
        t = Mathf.Clamp(t, 0f, count - 1f);

        int i = Mathf.FloorToInt(t); // 기준 인덱스
        float localT = t - i;        // 세그먼트 내 0~1

        int p0 = Mathf.Clamp(i - 1, 0, count - 1);
        int p1 = Mathf.Clamp(i,     0, count - 1);
        int p2 = Mathf.Clamp(i + 1, 0, count - 1);
        int p3 = Mathf.Clamp(i + 2, 0, count - 1);

        return CatmullRom(controlPoints[p0], controlPoints[p1], controlPoints[p2], controlPoints[p3], localT);
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t);
    }
}

