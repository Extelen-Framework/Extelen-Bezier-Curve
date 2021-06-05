using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{

    //Enumerators

    //Structs

    //Set Params
    [Header("Values")]
    [SerializeField] private bool m_useWorldSpace = false;
    public bool UseWorldSpace
    {
        get => m_useWorldSpace;
        set
        {
            m_useWorldSpace = value;
            if (m_lineRenderer) m_lineRenderer.useWorldSpace = value;
        }
    }
    public Vector3 LocalOrWorldPosition
    {
        get => m_useWorldSpace ? Vector3.zero : transform.position;
    }

    [Header("Bezier Position Controllers")]
    [SerializeField] private Vector3 m_startPoint = Vector3.zero;
    public Vector3 StartPoint { get => m_startPoint; set => m_startPoint = value; }
    private Vector3 LOWStartPoint { get => LocalOrWorldPosition + StartPoint; }

    [SerializeField] private Vector3 m_startController = new Vector3(0.5f, 0.5f);
    public Vector3 StartController { get => m_startController; set => m_startController = value; }
    private Vector3 LOWStartController { get => LocalOrWorldPosition + StartController; }

    [SerializeField] private Vector3 m_endController = new Vector3(-0.5f, 1.5f);
    public Vector3 EndController { get => m_endController; set => m_endController = value; }
    private Vector3 LOWEndController { get => LocalOrWorldPosition + EndController; }

    [SerializeField] private Vector3 m_endPoint = Vector3.up * 2;
    public Vector3 EndPoint { get => m_endPoint; set => m_endPoint = value; }
    private Vector3 LOWEndPoint { get => LocalOrWorldPosition + EndPoint; }

    [Header("Line Settings")]
    [SerializeField] private LineRenderer m_lineRenderer = null;
    [SerializeField] private int m_lineQuality = 32;

    private List<Vector3> m_path = new List<Vector3>();

    //Methods
    private void OnValidate()
    {
        m_lineQuality = Mathf.Clamp(m_lineQuality, 3, 256);

        if (!m_lineRenderer)
        {
            TryGetComponent<LineRenderer>(out m_lineRenderer);
            ComponentUtility.MoveComponentUp(this);

            if (m_lineRenderer)
            {
                m_lineRenderer.widthMultiplier = 0.1f;
                m_lineRenderer.useWorldSpace = m_useWorldSpace;
            }
        }
        ReCalculateBezierCurve();
    }
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(LOWStartPoint, 0.2f);
        Gizmos.DrawWireSphere(LOWEndPoint, 0.2f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(LOWStartController, 0.1f);
        Gizmos.DrawWireSphere(LOWEndController, 0.1f);

        Gizmos.DrawLine(LOWStartPoint, LOWStartController);
        Gizmos.DrawLine(LOWEndPoint, LOWEndController);

        Gizmos.color = Color.blue;
        List<Vector3> m_path = CalculateBezierPath();

        for (int i = 1; i < m_path.Count; i++)
        {
            Gizmos.DrawLine(m_path[i - 1], m_path[i]);
        }
    }
    private List<Vector3> CalculateBezierPath()
    {
        m_path.Clear();

        for (int i = 0; i < m_lineQuality + 1; i++)
        {
            m_path.Add(CalculateBezier((float)i / m_lineQuality, LOWStartPoint, LOWStartController, LOWEndController, LOWEndPoint));
        }

        return m_path;
    }
    private Vector3 CalculateBezier(float t, Vector3 startPoint, Vector3 startController, Vector3 endController, Vector3 endPoint)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * startPoint;
        point += 3 * uu * t * startController;
        point += 3 * u * tt * endController;
        point += ttt * endPoint;

        return point;
    }

    public void ReCalculateBezierCurve()
    {
        if (m_lineRenderer == null)
        {
            Debug.LogError("Line renderer is not assigned.");
            return;
        }

        m_lineRenderer.positionCount = m_lineQuality + 1;
        m_lineRenderer.SetPositions(CalculateBezierPath().ToArray());
    }
}