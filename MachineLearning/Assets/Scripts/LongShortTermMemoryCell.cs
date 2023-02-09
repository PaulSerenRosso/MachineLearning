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

    public float startLongTermMemory;
    public float startShortTermMemory;
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
        /*
        longTermMemory = startLongTermMemory;
        shortTermMemory = startShortTermMemory;
        */
        longTermMemory = 0;
        shortTermMemory = 0; 
        longTermToRememberFactor.SetActivationFunction((value) => 1.0f / (1.0f + Mathf.Exp(-value)));
        potentialNewLongMemoryToRemember.SetActivationFunction((value) => math.tanh(value));
        potentialNewLongTermMemoryToRememberFactor.SetActivationFunction((value) => 1.0f / (1.0f + Mathf.Exp(-value)));
        potentialShortMemoryToRememberFactor.SetActivationFunction((value) => 1.0f / (1.0f + Mathf.Exp(-value)));
/*
longTermToRememberFactor.SetActivationFunction((value) => math.tanh(value));
        potentialNewLongMemoryToRemember.SetActivationFunction((value) => math.tanh(value));
        potentialNewLongTermMemoryToRememberFactor.SetActivationFunction((value) => math.tanh(value));
        potentialShortMemoryToRememberFactor.SetActivationFunction((value) => math.tanh(value));
        */
        output = shortTermMemory;
        so = AgentManager.instance.so;
    }

    public override void Mutate()
    {
   //     so.startLongTermMemoryMutation.MutateValue(ref startLongTermMemory);
     //   so.startShortTermMemoryMutation.MutateValue(ref startShortTermMemory);
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
  
    public override void Copy(Neuron copy)
    {
        LongShortTermMemoryCell cell = (LongShortTermMemoryCell)copy;
        
        longTermMemory = cell.longTermMemory;
        shortTermMemory = cell.shortTermMemory;
        longTermToRememberFactor.Copy(cell.longTermToRememberFactor);
        potentialNewLongMemoryToRemember.Copy(cell.potentialNewLongMemoryToRemember);
        potentialNewLongTermMemoryToRememberFactor.Copy(cell.potentialNewLongTermMemoryToRememberFactor);
        potentialShortMemoryToRememberFactor.Copy(cell.potentialShortMemoryToRememberFactor);
        startLongTermMemory = cell.startLongTermMemory;
        startShortTermMemory = cell.startShortTermMemory;
        so = cell.so;
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
        settings.inputWeightMutation.MutateValue(ref inputWeight);
        settings.biasMutation.MutateValue(ref bias);
        settings.shortTermMemoryWeightMutation.MutateValue(ref shortTermMemoryWeight);
    }

    public LongShortTermMemoryCellComponent()
    {
        inputWeight = Random.Range(-1f, 1f);
        shortTermMemoryWeight = Random.Range(-1f, 1f);
        bias = Random.Range(-1f, 1f);
    }


    public void Copy(LongShortTermMemoryCellComponent copy)
    {
        bias = copy.bias;
        activationFunction = copy.activationFunction;
        inputWeight = copy.inputWeight;
        shortTermMemoryWeight = copy.shortTermMemoryWeight;
    }
}