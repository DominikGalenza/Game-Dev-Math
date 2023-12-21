using UnityEditor;
using UnityEngine;

public class CommonEditor : EditorWindow
{
    public virtual void DrawBlockGUI(string label, SerializedProperty property)
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(label, GUILayout.Width(100));
        EditorGUILayout.PropertyField(property, GUIContent.none);
        EditorGUILayout.EndHorizontal();
    }
}
