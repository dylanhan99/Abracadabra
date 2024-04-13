using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spellcast : MonoBehaviour
{
    [SerializeField] private Material iLineMaterial;
    [SerializeField] private float iLineWidth;
    [SerializeField] private float iTimer;
    [SerializeField] private float iInterval;
    
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateLineObject();
        }
    }
    

    void CreateLineObject()
    {
        GameObject newLine = new("Line Object");
        
        LineRenderer lr = newLine.AddComponent<LineRenderer>();
        lr.startWidth = iLineWidth;
        lr.endWidth = iLineWidth;
        lr.material = iLineMaterial;
        lr.enabled = true;

        Spell sp = newLine.AddComponent<Spell>();
        sp.Timer = iTimer;
        sp.Interval = iInterval;
    }
}
