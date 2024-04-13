using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.Il2Cpp;
using UnityEditor.PackageManager.UI;
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
    public readonly bool PointInEdge(float x, float y)
    {
        return (A * x + B * y + C) >= 0;
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

    private Vector2 OverallMinPoint, OverallMaxPoint;

    private float m_IntervalCooldown;

    Vector2 TargetSize;

    private Texture2D texture;
    
    void ResetInterval(){m_IntervalCooldown = Interval;}

    void OnGUI() 
    {
        GUI.DrawTexture(new Rect(0f,0f,Screen.width,Screen.height), texture);       
    }

    void Start()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        
        LinePoints = new List<Vector3>();
        IsHeld = true;
        m_Mesh = new();

        m_IntervalCooldown = 0;

        OverallMinPoint = new(float.MaxValue, float.MaxValue);
        OverallMaxPoint = new(float.MinValue, float.MinValue);

        TargetSize = new(128,128);

        texture = new Texture2D((int)TargetSize.x, (int)TargetSize.y);
        texture.wrapMode = TextureWrapMode.Clamp;
    }

    void Update()
    {
        if (IsHeld) // Drawing line
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (m_IntervalCooldown > 0)
                {
                    m_IntervalCooldown -= Time.deltaTime;
                }
                else
                {
                    LinePoints.Add(Camera.main.ScreenToWorldPoint(
                        new(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane))
                    );

                    m_LineRenderer.positionCount = LinePoints.Count;
                    m_LineRenderer.SetPositions(LinePoints.ToArray());
                    ResetInterval();
                }
            }
            
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                IsHeld = false;
                Destroy(gameObject, Timer);

                // Trigger the mask making/matching thing here
                m_LineRenderer.BakeMesh(m_Mesh, true);

                List<Vector3> vertices = new();
                List<int> indices = new();
                m_Mesh.GetVertices(vertices);
                m_Mesh.GetIndices(indices, 0);

                Debug.Log(vertices.Count);

                // Finding the overall minimum point
                foreach (Vector3 vert in vertices)
                {
                    OverallMinPoint.x = (vert.x < OverallMinPoint.x) ? vert.x : OverallMinPoint.x;
                    OverallMinPoint.y = (vert.y < OverallMinPoint.y) ? vert.y : OverallMinPoint.y;
                    OverallMaxPoint.x = (vert.x > OverallMaxPoint.x) ? vert.x : OverallMaxPoint.x;
                    OverallMaxPoint.y = (vert.y > OverallMaxPoint.y) ? vert.y : OverallMaxPoint.y;
                }

                float overallWidth = OverallMaxPoint.x - OverallMinPoint.x;
                float overallHeight = OverallMaxPoint.y - OverallMinPoint.y;

                for (int i = 0; i < indices.Count; i += 3)
                {
                    int i0 = indices[i];
                    int i1 = indices[i+1];
                    int i2 = indices[i+2];
                    int[] currIndices = new[]{i0, i1, i2};
                    
                    Vector2 minPoint = new(float.MaxValue, float.MaxValue);
                    Vector2 maxPoint = new(float.MinValue, float.MinValue);
                    for (int j = 0; j < 3; ++j)
                    {
                        Vector2 vert = vertices[currIndices[j]];

                        minPoint.x = (vert.x < minPoint.x) ? vert.x : minPoint.x;
                        minPoint.y = (vert.y < minPoint.y) ? vert.y : minPoint.y;
                        maxPoint.x = (vert.x > maxPoint.x) ? vert.x : maxPoint.x;
                        maxPoint.y = (vert.y > maxPoint.y) ? vert.y : maxPoint.y;
                    }

                    // Now using the AABB, iterate over each pixel, 
                    //  and determine the status of the pixel in the edge eqn
                    Edge e0 = new(vertices[i0], vertices[i1]);
                    Edge e1 = new(vertices[i1], vertices[i2]);
                    Edge e2 = new(vertices[i2], vertices[i0]);

                    float count = 0;
                    //count = (maxPoint.x - minPoint.x) * (maxPoint.y - minPoint.y);
                    float inter = 0.01f;
                    
                    Color[] pixels = texture.GetPixels();//new Color[(int)(TargetSize.x * TargetSize.y)];
                    for (float y = minPoint.y; y < maxPoint.y; y += inter)
                    {
                        for (float x = minPoint.x; x < maxPoint.x; x += inter)
                        {
                            
                            Vector2 relativePoint = new Vector2(
                                x - OverallMinPoint.x,
                                y - OverallMinPoint.y
                            );

                            relativePoint = new Vector2(
                                relativePoint.x / overallWidth * TargetSize.x,
                                relativePoint.y / overallHeight * TargetSize.y
                            );

                            int index = (int)(relativePoint.y) * (int)TargetSize.x + (int)(relativePoint.x);
                            
                            if (e0.PointInEdge(x, y) && e1.PointInEdge(x, y) && e2.PointInEdge(x, y))
                            {
                                count++;
                                pixels[index] = Color.black;
                            }  
                            else
                            {
                                pixels[index] = Color.white;
                            }
                            pixels[index] = Color.black;
                        }
                    }
                    //Debug.Log(count);
                    texture.SetPixels(pixels);
                    texture.Apply();
                }
            }
        }
    }

    private void OnDestroy() 
    {
        Debug.Log("Spell died");
    }
}
