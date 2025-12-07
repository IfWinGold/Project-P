using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class SplinePathFollower : MonoBehaviour
{
    public SplinePath spline;

    // currentT는 항상 인덱스 기반 (0 = 첫 포인트, 1 = 두 번째 포인트)
    public float currentT = 0f;
    [SerializeField] private Vector3 upVector;

    // --- 실행 중이든 아니든 currentT 바뀌면 바로 씬뷰에서 반영 ---
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (spline == null || spline.controlPoints == null || spline.controlPoints.Count == 0)
            return;

        ApplyPositionByT();
        
// #if UNITY_EDITOR
//         UnityEditor.SceneView.RepaintAll();
// #endif
    }
    #endif
    


    // --- 외부에서 currentT를 변경할 때 호출하면 됨 ---
    public void ApplyPositionByT()
    {
        int count = spline.controlPoints.Count;
        if (count < 2) return;

        float maxT = count - 1;

        // currentT는 인덱스 기반이라 그대로 사용
        currentT = Mathf.Clamp(currentT, 0f, maxT);

        Vector3 pos = spline.GetPoint(currentT);
        transform.position = pos;

        float nextT = Mathf.Min(currentT + 0.01f, maxT);
        Vector3 nextPos = spline.GetPoint(nextT);

        //Vector3 corss = Vector3.Cross(upVector, (nextPos - pos).normalized);
        //transform.LookAt((nextPos - pos).normalized,upVector);
        //transform.forward = corss;
        //transform.forward = (nextPos - pos).normalized;
        
        transform.rotation = Quaternion.LookRotation(nextPos - pos,upVector);
    }
}