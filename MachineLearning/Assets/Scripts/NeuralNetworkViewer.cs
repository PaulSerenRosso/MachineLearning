using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct NeuronDisplay
{
    public GameObject go;
    public RectTransform rectTransform;
    public Image image;
    private TMP_Text outputText;
    private TMP_Text longTermMemoryText;

    public void Init(float xPos, float yPos)
    {
        rectTransform = go.GetComponent<RectTransform>();
        image = go.GetComponent<Image>();
        outputText = go.transform.GetChild(0).GetComponent<TMP_Text>();
        longTermMemoryText = go.transform.GetChild(1).GetComponent<TMP_Text>();
        rectTransform.anchoredPosition = new Vector2(xPos, yPos);
    }

    public void Refresh(float outputValue, float longTermMemoryValue, Color color)
    {
        image.color = color;
        if(longTermMemoryValue != 0)
        longTermMemoryText.text = longTermMemoryValue.ToString("f2");
        outputText.text = outputValue.ToString("f2");
    }
}

public struct AxonDisplay
{
    public GameObject go;
    public Image image;
    private RectTransform rectTransform;

    public void Init(RectTransform start, RectTransform end, float thickNess, float neuronDiameter)
    {
        rectTransform = go.GetComponent<RectTransform>();
        image = go.GetComponent<Image>();
        Vector2 direction = (end.anchoredPosition - start.anchoredPosition);
        rectTransform.anchoredPosition =
            start.anchoredPosition + direction * 0.5f;
        rectTransform.sizeDelta =
            new Vector2(direction.magnitude - neuronDiameter, thickNess);
        rectTransform.rotation =
            Quaternion.FromToRotation(rectTransform.right, direction.normalized);
        rectTransform.SetAsFirstSibling();
    }
    
}

public class NeuralNetworkViewer : MonoBehaviour
{
    [SerializeField] private float layerSpacing = 100;
    [SerializeField] private float neuronVerticalSpacing = 32;
    [SerializeField] private float neuronDiameter = 32;
    [SerializeField] private float axonThickness = 2;
    [SerializeField] private Gradient colorGradient;
    
    [SerializeField] private GameObject neuronPrefab;
    [SerializeField] private GameObject axonPrefab;
    [SerializeField] private GameObject fitnessPrefab;
    [SerializeField] private RectTransform viewGroup;

    public Agent agent;
    private NeuralNetwork net;
    private NeuronDisplay[][] neurons;
    private AxonDisplay[][][] axons;
    private TMP_Text fitnessDisplay;

    private bool initialised;
    private int maxNeurons;
    private float padding;

    private int x;
    private int y;
    private int z;

    public static NeuralNetworkViewer instance;

    private void Awake()
    {
        instance = this; 
    }

    public void Refresh(Agent agent)
    {
        this.agent = agent;
        net = this.agent.net;
        if (!initialised)
        {
            initialised = true;
            Init();
        }
        RefreshAxons();
    }

    private void RefreshAxons()
    {
        for (x = 0; x < axons.Length; x++)
        {
            for (y = 0;y  < axons[x].Length; y++)
            {
                for (z = 0; z < axons[x][y].Length; z++)
                {
                    axons[x][y][z].image.color = colorGradient.Evaluate((net.axons[x][y][z] + 1) * 0.5f);
                }
            }
        }
    }

    private void Update()
    {
       
            for (y = 0; y < neurons[0].Length; y++)
            {
                neurons[0][y].Refresh(net.neurons[0][y].Ouput, 0, colorGradient.Evaluate(net.neurons[0][y].Ouput+1)*.5f);
            }
            for (y = 0; y < neurons[^1].Length; y++)
            {
                neurons[^1][y].Refresh(net.neurons[^1][y].Ouput, 0, colorGradient.Evaluate(net.neurons[^1][y].Ouput+1)*.5f);
            }
        
        for ( x= 1; x < neurons.Length-1; x++)
        {
            
            for (y = 0; y < neurons[x].Length; y++)
            {
                var longShortTermMemoryCell = (LongShortTermMemoryCell)agent.net.neurons[x][y];
                neurons[x][y].Refresh(net.neurons[x][y].Ouput, longShortTermMemoryCell.longTermMemory, colorGradient.Evaluate(net.neurons[x][y].Ouput+1)*.5f);
                
            }
        }
        fitnessDisplay.text = agent.fitness.ToString("F1");
    }

    private void Init()
    {
        InitMaxNeurons();
        InitNeurons();
        InitAxons();
        InitFitness();
    }

    private void InitFitness()
    {
        GameObject fitness = Instantiate(fitnessPrefab, viewGroup);
        fitness.GetComponent<RectTransform>().anchoredPosition = new Vector2(net.layers.Length * layerSpacing,
            -maxNeurons * 0.5f * neuronVerticalSpacing);
        fitnessDisplay = fitness.GetComponent<TMP_Text>();
    }

    private void InitAxons()
    {
        axons = new AxonDisplay[net.layers.Length - 1][][];
        for (x = 0; x < net.layers.Length-1; x++)
        {
            axons[x] = new AxonDisplay[net.layers[x]][];
            for (y = 0; y < net.layers[x] ; y++)
            {
                axons[x][y] = new AxonDisplay[net.layers[x+1]];
                for (z = 0; z < net.layers[x+1]; z++)
                {
                    axons[x][y][z] = new AxonDisplay();
                    axons[x][y][z].go = Instantiate(axonPrefab, viewGroup);
                    axons[x][y][z].Init(neurons[x][y].rectTransform, neurons[x+1][z].rectTransform, axonThickness, neuronDiameter);
                }
            }
        }
    }

    private void InitNeurons()
    {
        neurons = new NeuronDisplay[net.layers.Length][];
        int maxNeuronModulo = maxNeurons % 2;
        for (x = 0; x < net.layers.Length; x++)
        {
            if (net.layers[x] < maxNeurons)
            {
                padding = (maxNeurons - net.layers[x]) * 0.5f * neuronVerticalSpacing;
                if (net.layers[x] % 2 != maxNeuronModulo)
                {
                    padding += neuronVerticalSpacing * .5f;
                }
            }
            else padding = 0;

            neurons[x] = new NeuronDisplay[net.layers[x]];
            for ( y = 0; y < net.layers[x]; y++)
            {
                neurons[x][y] = new NeuronDisplay();
                neurons[x][y].go = Instantiate(neuronPrefab, viewGroup);
                neurons[x][y].Init(x*layerSpacing, (-padding - neuronVerticalSpacing )*y);
            }
        }
        
    }
    
    

    private void InitMaxNeurons()
    {
        for (x = 0;  x< net.layers.Length; x++)
        {
            if (net.layers[x] > maxNeurons)
            {
                maxNeurons = net.layers[x];
            }
        }
    }
}
