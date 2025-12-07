using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIStagePathBuilder))]
public class UIStagePathBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UIStagePathBuilder builder = (UIStagePathBuilder)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Path Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Rebuild Path"))
        {
            builder.RebuildPath();

            // 씬 갱신
            EditorUtility.SetDirty(builder);
            if (builder.spline != null)
                EditorUtility.SetDirty(builder.spline);

            SceneView.RepaintAll();
        }
    }
}