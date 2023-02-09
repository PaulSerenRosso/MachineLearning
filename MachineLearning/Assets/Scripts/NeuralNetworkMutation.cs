using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct NeuralNetworkMutation
{
  [SerializeField]
  private float probability;
  [SerializeField]
  private float power;

  public void MutateValue(ref float currentValue)
  {
    if ( Random.value< power)
    {
      currentValue += Random.Range(-power, power);
    }
  }
}
