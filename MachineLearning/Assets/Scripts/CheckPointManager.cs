using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager instance;

    public List<CheckPoint> allCheckPoints = new List<CheckPoint>();
    public CheckPoint firstCheckPoint;

    public float checkPointCount;
    private void Awake()
    {
        instance = this;
        Init();
    }

    [ContextMenu("init")]
    void Init()
    {
        allCheckPoints.Clear();
        foreach (Transform child in transform)
        {
            allCheckPoints.Add(child.GetComponent<CheckPoint>());
            
        }

        checkPointCount = allCheckPoints.Count;

        firstCheckPoint = allCheckPoints[0];

        for (int i = 0; i < allCheckPoints.Count - 1; i++)
        {
            allCheckPoints[i].nextCheckPoint = allCheckPoints[i + 1];
            allCheckPoints[i].index =  i; 
        
        }
     
        allCheckPoints[^1].nextCheckPoint = allCheckPoints[0];
        allCheckPoints[^1].index = allCheckPoints.Count - 1;
    }
}