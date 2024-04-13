using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject g_FPS;
    private float[] DTArray;
    private int LastFrameIndex;
    
    void Start()
    {
        DTArray = new float[512];
        LastFrameIndex = 0;
    }

    void Update()
    {
        DTArray[LastFrameIndex] = Time.deltaTime;
        ++LastFrameIndex;
        LastFrameIndex %= DTArray.Length;
        
        g_FPS.GetComponent<TMPro.TextMeshProUGUI>().text = Mathf.RoundToInt(CalculateFPS()).ToString();
    }

    private float CalculateFPS()
    {
        return DTArray.Length / DTArray.Sum();
    }
}
