using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    
    
    [SerializeField] private Agent agentPrefab;
    [SerializeField] private Transform agentGroup;

    private Agent agent; 
    List<Agent> agents= new ();
    [SerializeField] private CameraController cameraController;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text generationCountText;
    private int generationCount; 
    private float startingTime;

    public AgentGlobalSettingsSO so;
    public static AgentManager instance;
    private void Awake()
    {
        instance = this; 
    }


    private void Start()
    {
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        StartNewGeneration();
        Focus();
        yield return new WaitForSeconds(so.trainingDuration);
        StartCoroutine(Loop());
    }

    private void Focus()
    {
        NeuralNetworkViewer.instance.Refresh(agents[0]);
        cameraController.target = agents[0].transform;
    } 

    private void StartNewGeneration()
    {
        ResetTimer();
        agents = agents.OrderByDescending(a=> a.fitness).ToList();
        AddOrRemoveAgent();
        Mutate();
        ResetAgents();
        SetDefaultMaterials();
        RefreshGenerationCount();
    }

    private void RefreshGenerationCount()
    {
        generationCount++;
        generationCountText.text = generationCount.ToString();
    }


    private void Mutate()
    {
        for (int i = agents.Count/2; i < agents.Count; i++)
        {
            agents[i].net.CopyNet(agents[i-(agents.Count/2)].net);
            agents[i].net.Mutate(so.axonMutationRate, so.neuronMutationRate ,so.axonMutationPower );
            agents[i].SetMutatedMaterial();
        }
    }

    void ResetTimer()
    {
        startingTime = Time.time; 
    }

    private void Update()
    {
        timerText.text = (so.trainingDuration - (Time.time - startingTime)).ToString("f0");
        
    }

    private void AddOrRemoveAgent()
    {
        if (agents.Count != so.populationSize)
        {
            int dif = so.populationSize - agents.Count;
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
        DataManager.instance.Save(nets, generationCount);
    }

    public void Load()
    {
        Data data = DataManager.instance.Load();
        generationCount = data.generationCount;
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
