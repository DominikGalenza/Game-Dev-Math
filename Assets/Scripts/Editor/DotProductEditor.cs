using System;
using UnityEditor;
using UnityEngine;

public class DotProductEditor : EditorWindow
{
    public Vector3 Point0;
    public Vector3 Point1;
    public Vector3 CentralPoint;

    private SerializedObject serializedObject;
    private SerializedProperty propertyPoint0;
    private SerializedProperty propertyPoint1;
    private SerializedProperty propertyCentralPoint;
    private GUIStyle guiStyle = new GUIStyle();
    
    [MenuItem("Tools/Dot Product")]
    public static void ShowWindow()
    {
        DotProductEditor window = (DotProductEditor)GetWindow(typeof(DotProductEditor), true, "Dot Product");
        window.Show();
    }

	private void OnEnable()
	{
        if (Point0 == Vector3.zero && Point1 == Vector3.zero)
        {
            Point0 = new Vector3(0f, 1f, 0f);
            Point1 = new Vector3(0.5f, 0.5f, 0f);
            CentralPoint = Vector3.zero;
        }

        serializedObject = new SerializedObject(this);
        propertyPoint0 = serializedObject.FindProperty("Point0");
        propertyPoint1 = serializedObject.FindProperty("Point1");
        propertyCentralPoint = serializedObject.FindProperty("CentralPoint");

        guiStyle.fontSize = 25;
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.normal.textColor = Color.white;

        SceneView.duringSceneGui += SceneGUI;
	}

	private void OnDisable()
	{
		SceneView.duringSceneGui -= SceneGUI;
	}

	private void OnGUI()
	{
		serializedObject.Update();

        DrawBlockGUI("Point 0", propertyPoint0);
        DrawBlockGUI("Point 1", propertyPoint1);
        DrawBlockGUI("Central point", propertyCentralPoint);

        if (serializedObject.ApplyModifiedProperties())
        {
            SceneView.RepaintAll();
        }
	}

	private void DrawBlockGUI(string label, SerializedProperty property)
	{
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(label, GUILayout.Width(100));
        EditorGUILayout.PropertyField(property, GUIContent.none);
        EditorGUILayout.EndHorizontal();
	}

	private void SceneGUI(SceneView view)
	{
        Handles.color = Color.red;
        Vector3 p0 = SetMovePoint(Point0);
        Handles.color = Color.green;
        Vector3 p1 = SetMovePoint(Point1);
        Handles.color = Color.white;
        Vector3 c = SetMovePoint(CentralPoint);

        if (Point0 != p0 || Point1 != p1 || CentralPoint != c)
        {
            Point0 = p0;
            Point1 = p1;
            CentralPoint = c;

            Repaint();
        }

        DrawLabel(p0, p1, c);
	}

	private Vector3 SetMovePoint(Vector3 position)
	{
        float size = HandleUtility.GetHandleSize(Vector3.zero) * 0.15f;
        return Handles.FreeMoveHandle(position, Quaternion.identity, size, Vector3.zero, Handles.SphereHandleCap);
	}

    private float DotProduct(Vector3 point0, Vector3 point1, Vector3 centralPoint)
    {
        Vector3 a = (point0 - centralPoint).normalized;
        Vector3 b = (point1 - centralPoint).normalized;

        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
    }

    private void DrawLabel(Vector3 point0, Vector3 point1, Vector3 centralPoint)
    {
        Handles.Label(centralPoint, DotProduct(point0, point1, centralPoint).ToString("F1"), guiStyle);
        Handles.color = Color.black;

        Vector3 leftToCentralPoint = WorldRotation(point0, centralPoint, new Vector3(0f, 1f, 0f));
        Vector3 rightToCentralPoint = WorldRotation(point0, centralPoint, new Vector3(0f, -1f, 0f));

        Handles.DrawAAPolyLine(3f, point0, centralPoint);
        Handles.DrawAAPolyLine(3f, point1, centralPoint);
        Handles.DrawAAPolyLine(3f, centralPoint, leftToCentralPoint);
        Handles.DrawAAPolyLine(3f, centralPoint, rightToCentralPoint);
    }

    private Vector3 WorldRotation(Vector3 point, Vector3 centralPoint, Vector3 position)
    {
        Vector2 direction = (point - centralPoint).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        return centralPoint + rotation * position;
    }
}
