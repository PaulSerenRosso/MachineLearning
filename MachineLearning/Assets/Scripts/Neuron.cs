using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Apple;

[Serializable]
public abstract class Neuron 
{
    public float Ouput => output;
    protected float output;
    public abstract void UpdateNeuron(float input);

    public abstract void Init();

    public abstract void Mutate();
    
    
    public Neuron()
    {
        
    }

    public abstract void Copy(Neuron copy);
    
}
