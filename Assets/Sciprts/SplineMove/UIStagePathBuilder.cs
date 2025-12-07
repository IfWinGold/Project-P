using System;
using System.Collections.Generic;
using UnityEngine;

public class UIStagePathBuilder : MonoBehaviour
{
    [Header("UI Buttons in Path Order")]
    public RectTransform[] stageButtons;
    [SerializeField] private SplinePathFollower follower;

    [Header("Canvas Reference")]
    public Canvas canvas;

    [Header("Spline Target")]
    public SplinePath spline;

    public Camera uiCamera; // Canvas가 Screen Space - Camera이면 필요
    

    public void ReBuild()
    {
        RebuildPath();
        follower.ApplyPositionByT();
    }

    /// <summary>
    /// 버튼 눌러서 path 재생성
    /// </summary>
    public void RebuildPath()
    {
        if (stageButtons == null || stageButtons.Length == 0)
        {
            Debug.LogWarning("[UIStagePathBuilder] Stage buttons가 없습니다.");
            return;
        }

        if (spline == null)
        {
            Debug.LogWarning("[UIStagePathBuilder] spline이 없습니다.");
            return;
        }

        List<Vector3> worldPoints = new List<Vector3>();

        foreach (var btn in stageButtons)
        {
            worldPoints.Add(UIToWorld(btn));
        }

        spline.controlPoints = worldPoints;

        Debug.Log("[UIStagePathBuilder] Path 재생성 완료 (" + worldPoints.Count + " points)");
    }

    Vector3 UIToWorld(RectTransform ui)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, ui.position);

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(ray, out float dist))
            return ray.GetPoint(dist);

        return Vector3.zero;
    }
}