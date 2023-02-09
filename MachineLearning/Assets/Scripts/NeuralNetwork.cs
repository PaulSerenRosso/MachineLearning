using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class NeuralNetwork
{
    public int[] layers = new[] { 6, 6,6, 2 };
    public Neuron[][] neurons;
    public float[][][] axons;
    private int x;
    private int y;
    private int z;

    private int yPreviousLayer;
    private int previousLayer;
    private readonly bool isOneMethod = true;

    public NeuralNetwork()
    {
        
    }

    public NeuralNetwork(int[] layersModel, bool isOneMethod = true)
    {
        this.isOneMethod = isOneMethod;
        layers = new int[layersModel.Length];
        for (x = 0; x < layersModel.Length; x++)
        {
            layers[x] = layersModel[x];
        }
        InitNeurons();
        InitAxons();
    }

    private void InitAxons()
    {
        axons = new float[layers.Length - 1][][];

        for (x = 0; x < layers.Length - 1; x++)
        {
            axons[x] = new float[layers[x]][];

            for (y = 0; y < layers[x]; y++)
            {
                axons[x][y] = new float[layers[x + 1]];

                for (z = 0; z < layers[x + 1]; z++)
                {
                    axons[x][y][z] = UnityEngine.Random.Range(-1f, 1f);
                }
            }
        }
        /*
        if (isOneMethod)
        {
        axons = new float[layers.Length - 1][][];
        for (x = 0; x < layers.Length-1 ; x++)
        {
            axons[x] = new float[layers[x + 1]][];
            for ( y = 0; y < layers[x+1]; y++)
            {
                axons[x][y] = new float[layers[x]];
                for (z = 0; z < layers[x]; z++)
                {
                    axons[x][y][z] = Random.Range(-1f, 1f);
                }
            }
        }
        }
        else
        {
            axons = new float[layers.Length - 1][][];
            for (x = 0; x < layers.Length-1 ; x++)
            {
                axons[x] = new float[layers[x + 1]][];
                for ( y = 0; y < layers[x+1]; y++)
                {
                    axons[x][y] = new float[layers[x+1]];
                    for (z = 0; z < layers[x+1]; z++)
                    {
                        axons[x][y][z] = Random.Range(-1f, 1f);
                    }
                }
            }
        }
        */
    }

   
        float value;
    
        public void FeedForward(float[] inputs)
        {
            for (x = 0; x < neurons[0].Length; x++)
            {
                neurons[0][x].UpdateNeuron(inputs[x]);
            }
            for ( x = 1; x < layers.Length; x++)
            {
                for ( y = 0; y < layers[x]; y++)
                {
                    value = 0;

                    for ( yPreviousLayer = 0; yPreviousLayer < layers[x -1]; yPreviousLayer++)
                    {
                        value += neurons[x - 1][yPreviousLayer].Ouput * axons[x - 1][yPreviousLayer][y];
                    }
                    neurons[x][y].UpdateNeuron((float)Math.Tanh(value));
                }
            }
        }
        /*
        neurons[0] = inputs;

        if (isOneMethod)
        {
            for (x = 1; x < layers.Length; x++)
            {
                previousLayer = x - 1;
                for (y = 0; y < layers[x]; y++)
                {
                    neurons[x][y] = 0;
                    for (yPreviousLayer = 0; yPreviousLayer < axons[previousLayer][y].Length; yPreviousLayer++)
                    {
                        neurons[x][y] += axons[previousLayer][y][yPreviousLayer] *
                                         neurons[previousLayer][yPreviousLayer];
                    }

                    Debug.Log(math.tanh(5));
                    neurons[x][y] = math.tanh(neurons[x][y]);
                }
            }
        }
        else
        {
            for (x = 1; x < layers.Length; x++)
            {
                previousLayer = x - 1;
                for (y = 0; y < layers[x]; y++)
                {
                    neurons[x][y] = 0;
                    for (yPreviousLayer = 0; yPreviousLayer < neurons[previousLayer].Length; yPreviousLayer++)
                    {
                        neurons[x][y] += axons[previousLayer][yPreviousLayer][y] *
                                         neurons[previousLayer][yPreviousLayer];
                    }

                    neurons[x][y] = math.tanh(neurons[x][y]);
                }
            }
        }
    */
// biais
// en fonction du pourcentage du circuit 
// short memoriel recurrent je garde en mémmoire les précédentes valeurs des neuronnes et je mets un facteur de prise en compte du passé
//
    private void InitNeurons()
    {
        neurons = new Neuron[layers.Length][];
        neurons[0] = new BasicNeuron[layers[0]];
        for (y = 0; y < neurons[0].Length; y++)
        {
            neurons[0][y] = new BasicNeuron();
        }
        neurons[^1] = new BasicNeuron[layers[^1]];
        for (y = 0; y < neurons[^1].Length; y++)
        {
            neurons[^1][y] = new BasicNeuron();
        }
        for (x = 1; x < layers.Length-1; x++)
        {
            neurons[x] = new LongShortTermMemoryCell[layers[x]];
            for (y = 0; y < neurons[x].Length; y++)
            {
                neurons[x][y] = new LongShortTermMemoryCell();
            }
        }
    }

        public void CopyNet(NeuralNetwork netCopy)
        {
            for (x = 0; x < netCopy.axons.Length; x++)
            {
                for (y = 0; y < netCopy.axons[x].Length; y++)
                {
                    for (z = 0; z < netCopy.axons[x][y].Length; z++)
                    {
                        axons[x][y][z] = netCopy.axons[x][y][z]; 
                    }
                }
                
            }

            for (x = 0; x < netCopy.axons.Length; x++)
            {
                for (y = 0; y < netCopy.axons[x].Length; y++)
                {
                    neurons[x][y].Copy(netCopy.neurons[x][y]);
                }
            }

        }

        // rendre propoertionnel à l'efficacité de notre voiture 
        // avoir plusieurs thickness
        
        public void Mutate(NeuralNetworkMutation axonMutation)
        {
            for (x = 0; x < axons.Length; x++)
            {
                for (y = 0; y < axons[x].Length; y++)
                {
                    for (z = 0; z < axons[x][y].Length; z++)
                    {
                        axonMutation.MutateValue(ref axons[x][y][z]);
                    }
                }
            }

            for (x = 0; x < neurons.Length; x++)
            {
                for (y = 0; y < neurons[x].Length; y++)
                {
                    neurons[x][y].Mutate();
                }
            }
            
        }

        public void ResetNeuralNetworkForNewGeneration()
        {
            for (x = 0; x < neurons.Length; x++)
            {
                for (y = 0; y < neurons[x].Length; y++)
                {
                    neurons[x][y].Init();
                }
            }
        }
}