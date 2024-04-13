using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.Il2Cpp;
using UnityEngine;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

struct Edge
{
    private float A, B, C;
    public Edge(Vector2 p0, Vector2 p1)
    {
        A = p0.y - p1.y;
        B = p1.x - p0.x;
        C = p0.x * p1.y - p1.x * p0.y;
    }

    // Given an arbitrary point, determine if point is on the inside plane of the edge
    public readonly bool PointInEdge(Vector2 point)
    {
        return (A * point.x + B * point.y + C) >= 0;
    }
}

public class Spell : MonoBehaviour
{
    public float Timer{get;set;}
    public float Interval{get;set;}
    private LineRenderer m_LineRenderer;
    private List<Vector3> LinePoints;
    private Mesh m_Mesh;
    private bool IsHeld;

    void Start()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        
        LinePoints = new List<Vector3>();
        IsHeld = true;
        m_Mesh = new();
    }

    void Update()
    {
        if (IsHeld) // Drawing line
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                LinePoints.Add(Camera.main.ScreenToWorldPoint(
                    new(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane))
                );

                m_LineRenderer.positionCount = LinePoints.Count;
                m_LineRenderer.SetPositions(LinePoints.ToArray());
            }
            
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                IsHeld = false;
                Destroy(gameObject, Timer);

                // Trigger the mask making/matching thing here
                m_LineRenderer.BakeMesh(m_Mesh, true);
                Debug.Log(m_Mesh.subMeshCount);
                Debug.Log(m_Mesh.GetTopology(0));

                List<Vector3> vertices = new();
                List<int> indices = new();
                m_Mesh.GetVertices(vertices);
                m_Mesh.GetIndices(indices, 0);

                for (int i = 0; i < indices.Count; i += 3)
                {
                    int i0 = indices[i];
                    int i1 = indices[i+1];
                    int i2 = indices[i+2];
                    int[] currIndices = new[]{i0, i1, i2};
                    
                    Vector2 minPoint = new(float.MinValue, float.MinValue);
                    Vector2 maxPoint = new(float.MaxValue, float.MaxValue);
                    for (int j = 0; j < 3; ++j)
                    {
                        Vector2 screenVert = Camera.main.WorldToScreenPoint(vertices[currIndices[j]]);

                        minPoint.x = (screenVert.x < minPoint.x) ? screenVert.x : minPoint.x;
                        minPoint.y = (screenVert.y < minPoint.y) ? screenVert.y : minPoint.y;
                        maxPoint.x = (screenVert.x > maxPoint.x) ? screenVert.x : maxPoint.x;
                        maxPoint.y = (screenVert.y > maxPoint.y) ? screenVert.y : maxPoint.y;
                    }

                    // Now using the AABB, iterate over each pixel, 
                    //  and determine the status of the pixel in the edge eqn
                    Edge e0 = new(vertices[i0], vertices[i1]);
                    Edge e1 = new(vertices[i1], vertices[i2]);
                    Edge e2 = new(vertices[i2], vertices[i0]);

                    
                }
                
                // Find the aabb window to iterate over (min max x and y positions)
                //Vector2 minPoint = new(), maxPoint = new();
                //minPoint = maxPoint = Camera.main.WorldToScreenPoint(new(vertices[0].x, vertices[0].y));
                //for (int i = 0; i < vertices.Count; ++i)
                //{
                //    Vector2 screenVert = Camera.main.WorldToScreenPoint(vertices[i]);
                //    minPoint.x = (screenVert.x < minPoint.x) ? screenVert.x : minPoint.x;
                //    minPoint.y = (screenVert.y < minPoint.y) ? screenVert.y : minPoint.y;
                //    maxPoint.x = (screenVert.x > maxPoint.x) ? screenVert.x : maxPoint.x;
                //    maxPoint.y = (screenVert.y > maxPoint.y) ? screenVert.y : maxPoint.y;
                //} 
                
                //Debug.Log(minPoint);
                //Debug.Log(maxPoint);
                //Debug.Log(Screen.height);
                //Debug.Log(Screen.width);
                // For each pixel 
            }
        }
    }

    private void OnDestroy() 
    {
        Debug.Log("Spell died");
    }
}
