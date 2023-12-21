using System;
using UnityEditor;
using UnityEngine;

public class CrossProductEditor : CommonEditor, IUpdateSceneGUI
{
    public Vector3 A;
    public Vector3 B;
    public Vector3 AxB;

    private SerializedObject serializedObject;
    private SerializedProperty propertyA;
    private SerializedProperty propertyB;
    private SerializedProperty propertyAxB;
	private GUIStyle guiStyle = new GUIStyle();

    [MenuItem("Tools/Cross Product")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CrossProductEditor), true, "Cross Product");
    }

	private void OnEnable()
	{
		if (A == Vector3.zero &&  B == Vector3.zero)
		{
			SetDefaultValues();
		}

		serializedObject = new SerializedObject(this);
		propertyA = serializedObject.FindProperty("A");
		propertyB = serializedObject.FindProperty("B");
		propertyAxB = serializedObject.FindProperty("AxB");

		guiStyle.fontSize = 25;
		guiStyle.fontStyle = FontStyle.Bold;
		guiStyle.normal.textColor = Color.white;

		SceneView.duringSceneGui += SceneGUI;
		Undo.undoRedoPerformed += Repaint;
	}

	private void OnDisable()
	{
		SceneView.duringSceneGui -= SceneGUI;
		Undo.undoRedoPerformed -= Repaint;
	}

	private void OnGUI()
	{
		serializedObject.Update();

		DrawBlockGUI("A", propertyA);
		DrawBlockGUI("B", propertyB);
		DrawBlockGUI("AxB", propertyAxB);

		if (serializedObject.ApplyModifiedProperties())
		{
			SceneView.RepaintAll();
		}

		if (GUILayout.Button("Reset Values"))
		{
			SetDefaultValues();
		}
	}

	private void SetDefaultValues()
	{
		A = new Vector3(0f, 1f, 0f);
		B = new Vector3(1f, 0f, 0f);
		AxB = new Vector3(0f, 0f, 0f);
		SceneView.RepaintAll();
	}

	public void SceneGUI(SceneView view)
	{
		Vector3 a = Handles.PositionHandle(A, Quaternion.identity);
		Vector3 b = Handles.PositionHandle(B, Quaternion.identity);

		Handles.color = Color.blue;
		Vector3 axb = CrossProduct(a, b);
		Handles.DrawSolidDisc(axb, Vector3.forward, 0.05f);

		if (A != a ||  B != b)
		{
			Undo.RecordObject(this, "Tool Move");

			A = a;
			B = b;
			AxB = axb;

			Repaint();
		}

		DrawLineGUI(a, "A", Color.green);
		DrawLineGUI(b, "B", Color.red);
		DrawLineGUI(axb, "AxB", Color.blue);
	}

	private void DrawLineGUI(Vector3 position, string text, Color color)
	{
		Handles.color = color;
		Handles.Label(position, text, guiStyle);
		Handles.DrawAAPolyLine(3f, position, Vector3.zero);
	}

	//private Vector3 CrossProduct(Vector3 a, Vector3 b)
	//{
	//	float x = a.y * b.z - a.z * b.y;
	//	float y = a.z * b.x - a.x * b.z;
	//	float z = a.x * b.y - a.y * b.x;

	//	return new Vector3(x, y, z);
	//}

	private Vector3 CrossProduct(Vector3 a, Vector3 b)
	{
		Matrix4x4 matrix = new Matrix4x4();

		matrix[0, 0] = 0;
		matrix[0, 1] = b.z;
		matrix[0, 2] = -b.y;

		matrix[1, 0] = -b.z;
		matrix[1, 1] = 0;
		matrix[1, 2] = b.x;

		matrix[2, 0] = b.y;
		matrix[2, 1] = -b.x;
		matrix[2, 2] = 0;

		return matrix * a;
	}
}
