using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
                
                // Find the aabb window to iterate over (min max x and y positions)
                Vector2 minPoint = new(), maxPoint = new();
                minPoint = maxPoint = Camera.main.WorldToScreenPoint(new(vertices[0].x, vertices[0].y));
                for (int i = 0; i < vertices.Count; ++i)
                {
                    Vector2 screenVert = Camera.main.WorldToScreenPoint(vertices[i]);

                    minPoint.x = (screenVert.x < minPoint.x) ? screenVert.x : minPoint.x;
                    minPoint.y = (screenVert.y < minPoint.y) ? screenVert.y : minPoint.y;
                    maxPoint.x = (screenVert.x > maxPoint.x) ? screenVert.x : maxPoint.x;
                    maxPoint.y = (screenVert.y > maxPoint.y) ? screenVert.y : maxPoint.y;
                }
                
                Debug.Log(minPoint);
                Debug.Log(maxPoint);
                Debug.Log(Screen.height);
                Debug.Log(Screen.width);
                // For each pixel 
            }
        }
    }

    private void OnDestroy() 
    {
        Debug.Log("Spell died");
    }
}
