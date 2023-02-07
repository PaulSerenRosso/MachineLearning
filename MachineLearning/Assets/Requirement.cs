using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Requirement : MonoBehaviour
{
    public int[] layers = new[] { 2, 3, 3, 2 };
    public float[][] neurons;
    public float[][][] axoms;

    private void Start()
    {
        neurons = new float[layers.Length][];
        axoms = new float[layers.Length-1][][];
        for (int x = 0; x < layers.Length; x++)
        {
            neurons[x] = new float[layers[x]];
            if (x != 0)
            {
            axoms[x-1] = new float[layers[x]][];
            for (int y = 0; y < layers[x]; y++)
            {
                axoms[x-1][y] = new float[layers[x-1]];
                for (int z = 0; z < axoms[x-1][y].Length; z++)
                {
                    axoms[x-1][y][z] = Random.Range(-1.0f, 1.0f);
                    Debug.Log("layer count " + x +"neurons "+ y + "axom " + z);
                }
            }
            }
        }
    }
}
