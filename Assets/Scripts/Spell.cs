using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public float Timer{get;set;}
    public float Interval{get;set;}
    private List<Vector3> LinePoints;
    private bool IsHeld;
    private LineRenderer m_LineRenderer;

    void Start()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        
        LinePoints = new List<Vector3>();
        IsHeld = true;
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
            }
        }
    }

    private void OnDestroy() 
    {
        Debug.Log("Spell died");
    }
}
