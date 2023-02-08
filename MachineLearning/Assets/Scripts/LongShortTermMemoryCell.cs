using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class LongShortTermMemoryCell : Neuron
{
    public float longTermMemory;
    public float shortTermMemory;

    public LongShortTermMemoryCellComponent longTermToRememberFactor;
    public LongShortTermMemoryCellComponent potentialNewLongMemoryToRemember;
    public LongShortTermMemoryCellComponent potentialNewLongTermMemoryToRememberFactor;
    public LongShortTermMemoryCellComponent potentialShortMemoryToRememberFactor;

    private AgentGlobalSettingsSO so;
    private int i;

    public LongShortTermMemoryCell() : base()
    {
        longTermToRememberFactor = new LongShortTermMemoryCellComponent();
        potentialNewLongMemoryToRemember = new LongShortTermMemoryCellComponent();
        potentialNewLongTermMemoryToRememberFactor = new LongShortTermMemoryCellComponent();
        potentialShortMemoryToRememberFactor = new LongShortTermMemoryCellComponent();
        so = AgentManager.instance.so;
    }

    public override void Init()
    {
     
        longTermMemory = 0;
        shortTermMemory = 0;
        longTermToRememberFactor.SetActivationFunction((value) => 1.0f / (1.0f + Mathf.Exp(-value)));
        potentialNewLongMemoryToRemember.SetActivationFunction((value) => math.tanh(value));
        potentialNewLongTermMemoryToRememberFactor.SetActivationFunction((value) => 1.0f / (1.0f + Mathf.Exp(-value)));
        potentialShortMemoryToRememberFactor.SetActivationFunction((value) => 1.0f / (1.0f + Mathf.Exp(-value)));
        output = 0;
        so = AgentManager.instance.so;
    }

    public override void Mutate()
    {
        for (i = 0; i < so.longShortTermMemoryCellGlobalSettings.Length; i++)
        {
            switch (so.longShortTermMemoryCellGlobalSettings[i].type)
            {
                case LongShortMemoryCellComponentType.LongTermToRememberFactor:
                {
                    longTermToRememberFactor.Mutate(so.longShortTermMemoryCellGlobalSettings[i]);
                    break;
                }
                case LongShortMemoryCellComponentType.PotentialNewLongMemoryToRemember:
                {
                    potentialNewLongMemoryToRemember.Mutate(so.longShortTermMemoryCellGlobalSettings[i]);
                    break;
                }
                case LongShortMemoryCellComponentType.PotentialShortMemoryToRememberFactor:
                {
                    potentialShortMemoryToRememberFactor.Mutate(so.longShortTermMemoryCellGlobalSettings[i]);
                    break;
                }
                case LongShortMemoryCellComponentType.PotentialNewLongTermMemoryToRememberFactor:
                {
                    potentialNewLongTermMemoryToRememberFactor.Mutate(so.longShortTermMemoryCellGlobalSettings[i]);
                    break;
                }
            }
        }
    }

    public override void UpdateNeuron(float input)
    {
        UpdateForgetGate(input);
        UpdateInputGate(input);
        UpdateOutputGate(input);
        output = shortTermMemory;
    }

    void UpdateForgetGate(float input)
    {
        longTermMemory *= longTermToRememberFactor.GetResult(input, shortTermMemory);
    }

    void UpdateInputGate(float input)
    {
        longTermMemory += potentialNewLongMemoryToRemember.GetResult(input, shortTermMemory) *
                          potentialNewLongTermMemoryToRememberFactor.GetResult(input, shortTermMemory);
    }

    void UpdateOutputGate(float input)
    {
        shortTermMemory = math.tanh(longTermMemory) *
                          potentialShortMemoryToRememberFactor.GetResult(input, shortTermMemory);
    }
}

[Serializable]
public class LongShortTermMemoryCellComponent
{
    public float inputWeight;
    public float shortTermMemoryWeight;
    public float bias;
    public Func<float, float> activationFunction;

    public void SetActivationFunction(Func<float, float> activationFunction)
    {
        this.activationFunction = activationFunction;
    }

    public float GetResult(float input, float shortTermMemory)
    {
        return activationFunction.Invoke((input * inputWeight + shortTermMemoryWeight * shortTermMemory) + bias);
    }

    public void Mutate(LongShortTermMemoryCellComponentSettings settings)
    {
        inputWeight += Random.Range(-settings.inputWeightMutation, settings.inputWeightMutation);
        shortTermMemoryWeight +=
            Random.Range(-settings.shortTermMemoryWeightMutation, settings.shortTermMemoryWeightMutation);
        bias += Random.Range(-settings.biasMutation, settings.biasMutation);
    }

    public LongShortTermMemoryCellComponent()
    {
        inputWeight = Random.Range(-1f, 1f);
        shortTermMemoryWeight = Random.Range(-1f, 1f);
        bias = Random.Range(-1f, 1f);
    }
}