using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// prendre en compte les voitures 
public class CheckPoint : MonoBehaviour
{
    public Transform nextCheckPoint;

    private void OnTriggerEnter(Collider other)
    {
        Agent agent = other.GetComponent<Agent>();
        if (agent)
        {
            if (agent.nextCheckPoint.transform == transform)
            {
                agent.CheckPointReached(nextCheckPoint);
            }
        }
    }
}
