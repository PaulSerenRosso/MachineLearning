using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BasicNeuron : Neuron
{
    public BasicNeuron()
    {
        Init();
    }
    public override void UpdateNeuron(float input)
    {
        output = input;
    }

    public override void Init()
    {
        output = 0;
    }

    public override void Mutate()
    {
       
    }
}
