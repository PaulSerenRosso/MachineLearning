using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "", menuName = "ScriptableObjects/AgentGlobalSettingsSO", order = 1)]
public class AgentGlobalSettingsSO : ScriptableObject
{
    public int populationSize = 100;
    public float trainingDuration;
    public NeuralNetworkMutation axonMutation ;
    public NeuralNetworkMutation startLongTermMemoryMutation;
    public NeuralNetworkMutation startShortTermMemoryMutation;
    public LongShortTermMemoryCellComponentSettings[] longShortTermMemoryCellGlobalSettings;
}
// j'ai voulu faire 
// processus de comment j'ai fait
// mon implementation
// l'observation de mon implémentation
// car c'était comme ça raison de pk ça a marché ou non  
// du coup j'aurais du faire comme ça 

// 3-4 slides 
// graph analyse
// gif 
// schéma
// commentaire

// 20 -30 minutes