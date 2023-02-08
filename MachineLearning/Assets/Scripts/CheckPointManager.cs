using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager instance;

    public CheckPoint firstCheckPoint;
    
    private void Awake()
    {
        instance = this;
        Init();
    }

    [ContextMenu("init")]
    void Init()
    {
        var allCheckPoint = new List<CheckPoint>();
        foreach (Transform child in transform)
        {
            allCheckPoint.Add(child.GetComponent<CheckPoint>());
        }

        firstCheckPoint = allCheckPoint[0];
        
        for (int i = 0; i < allCheckPoint.Count-1; i++)
        {
            allCheckPoint[i].nextCheckPoint = allCheckPoint[i + 1].transform;
        }

        allCheckPoint[^1].nextCheckPoint = allCheckPoint[0].transform;
        
    }
}
