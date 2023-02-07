using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.Mathematics;
using UnityEditor.Timeline;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField]
    private int populuationSize = 100;

    [SerializeField] private float trainingDuration;

    [SerializeField] private float mutationRate = 5;
    [SerializeField] private float mutationPower = 5;
    [SerializeField] private Agent agentPrefab;
    [SerializeField] private Transform agentGroup;

    private Agent agent; 
    List<Agent> agents= new ();
    [SerializeField] private CameraController cameraController;
    private void Start()
    {
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        StartNewGeneration();
        Focus();
        yield return new WaitForSeconds(trainingDuration);
        StartCoroutine(Loop());
    }

    private void Focus()
    {
        cameraController.target = agents[0].transform;
    } 

    private void StartNewGeneration()
    {
        agents = agents.OrderByDescending(a=> a.fitness).ToList();
        AddOrRemoveAgent();

        Mutate();
        ResetAgents();
        SetDefaultMaterials();
    }


    private void Mutate()
    {
        for (int i = agents.Count/2; i < agents.Count; i++)
        {
            agents[i].net.CopyNet(agents[i-(agents.Count/2)].net);
            agents[i].net.Mutate(mutationRate, mutationPower);
            agents[i].SetMutatedMaterial();
        }
    }

    private void AddOrRemoveAgent()
    {
        if (agents.Count != populuationSize)
        {
            int dif = populuationSize - agents.Count;
            if (dif > 0)
            {
                for (int i = 0; i < dif; i++)
                {
                    AddAgent();
                }
            }
            else
            {
                for (int i = 0; i < -dif; i++)
                {
                    RemoveAgent();
                }
            }
        }
    }
    private void AddAgent()
    {
        agent = Instantiate(agentPrefab, Vector3.zero, quaternion.identity, agentGroup);
        agent.net = new NeuralNetwork(agentPrefab.net.layers);
        agents.Add(agent);
    }

    void RemoveAgent()
    {
        Destroy(agents[^1].gameObject);
        agents.RemoveAt(agents.Count-1);
    }
    private void ResetAgents()
    {
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].ResetAgent();
        }
    }

    void SetDefaultMaterials()
    {
        for (int i = 1; i < agents.Count/2; i++)
        {
            agents[i].SetDefaultMaterial();
        }
        agents[0].SetFirstMaterial();
    }

    public void Save()
    {
        List<NeuralNetwork> nets = new List<NeuralNetwork>();
        for (int i = 0; i < agents.Count; i++)
        {
            nets.Add(agents[i].net);
        }
        DataManager.instance.Save(nets);
    }

    public void Load()
    {
        Data data = DataManager.instance.Load();
        if (data != null)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].net = data.nets[i];
            }
        }

        End();
    }

    public void End()
    {
        StopAllCoroutines();
        StartCoroutine(Loop());
    }
}
