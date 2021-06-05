using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor
{

    //Set Params
    private SerializedProperty m_useWorldSpace = null;

    private SerializedProperty m_startPoint = null;
    private SerializedProperty m_startController = null;
    private SerializedProperty m_endController = null;
    private SerializedProperty m_endPoint = null;

    private SerializedProperty m_lineRenderer = null;
    private SerializedProperty m_lineQuality = null;

    private BezierCurve m_target = null;

    //Methods
    private void OnEnable()
    {
        m_target = target as BezierCurve;

        m_useWorldSpace = serializedObject.FindProperty("m_useWorldSpace");

        m_startPoint = serializedObject.FindProperty("m_startPoint");
        m_startController = serializedObject.FindProperty("m_startController");
        m_endController = serializedObject.FindProperty("m_endController");
        m_endPoint = serializedObject.FindProperty("m_endPoint");

        m_lineRenderer = serializedObject.FindProperty("m_lineRenderer");
        m_lineQuality = serializedObject.FindProperty("m_lineQuality");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_useWorldSpace);

        EditorGUILayout.PropertyField(m_startPoint, new GUIContent("Start Point"));
        EditorGUILayout.PropertyField(m_startController, new GUIContent("Start Controller"));
        EditorGUILayout.PropertyField(m_endController, new GUIContent("End Controller"));
        EditorGUILayout.PropertyField(m_endPoint, new GUIContent("End Point"));

        EditorGUILayout.PropertyField(m_lineRenderer);
        EditorGUILayout.PropertyField(m_lineQuality);

        m_target.ReCalculateBezierCurve();

        serializedObject.ApplyModifiedProperties();
    }
    public void OnSceneGUI()
    {
        Vector3 m_snap = Vector3.one * 0.25f;
        Vector3 m_lOWPosition = m_target.LocalOrWorldPosition;

        Vector3 m_startPoint = Handles.FreeMoveHandle(m_lOWPosition + m_target.StartPoint, Quaternion.identity, 0.2f, m_snap, Handles.CircleHandleCap);
        Vector3 m_startController = Handles.FreeMoveHandle(m_lOWPosition + m_target.StartController, Quaternion.identity, 0.1f, m_snap, Handles.CircleHandleCap);
        Vector3 m_endController = Handles.FreeMoveHandle(m_lOWPosition + m_target.EndController, Quaternion.identity, 0.1f, m_snap, Handles.CircleHandleCap);
        Vector3 m_endPoint = Handles.FreeMoveHandle(m_lOWPosition + m_target.EndPoint, Quaternion.identity, 0.2f, m_snap, Handles.CircleHandleCap);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Changed Vector Position");

            m_target.StartPoint = m_startPoint - m_lOWPosition;
            m_target.StartController = m_startController - m_lOWPosition;
            m_target.EndController = m_endController - m_lOWPosition;
            m_target.EndPoint = m_endPoint - m_lOWPosition;

            m_target.ReCalculateBezierCurve();
        }
    }
}