using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LongShortTermMemoryCellComponentSettings
{
    public LongShortMemoryCellComponentType type;

    public NeuralNetworkMutation biasMutation;

    public NeuralNetworkMutation inputWeightMutation;

    public NeuralNetworkMutation shortTermMemoryWeightMutation;
}