using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "", menuName = "ScriptableObjects/AgentGlobalSettingsSO", order = 1)]
public class AgentGlobalSettingsSO : ScriptableObject
{
    public int populationSize = 100;
    public float trainingDuration;
    public float axonMutationRate = 5;
    public float neuronMutationRate = 5;
    public float axonMutationPower = 5;
    public LongShortTermMemoryCellComponentSettings[] longShortTermMemoryCellGlobalSettings;
}