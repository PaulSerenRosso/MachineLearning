using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
// TODO: closeset point 
// previsualisation importante
public class Agent : MonoBehaviour
{
    public NeuralNetwork net;
    public float fitness; 
    [SerializeField] private CarController carController;
    [SerializeField] private float rayRange = 5;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Renderer renderer;
    [SerializeField] private Material firstMaterial; 
    [SerializeField] private Material defaultMaterial; 
    [SerializeField] private Material mutatedMaterial; 
    [SerializeField] private Vector3 carPosition;
    private float[] inputs = new float[5];

    [SerializeField] private Rigidbody rb;
    public Transform nextCheckPoint;

    [SerializeField]
    float totalCheckPointDist;

    private float distanceTraveled; 

    [SerializeField] private float nextCheckPointDist;

   
    public void ResetAgent()
    {
      transform.position = Vector3.zero;
      transform.rotation = quaternion.identity;
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      inputs = new float[net.layers[0]];
      carController.Reset();
      fitness = 0;
      net.ResetNeuralNetworkForNewGeneration();
      totalCheckPointDist = 0;
      nextCheckPoint = CheckPointManager.instance.firstCheckPoint.transform;
      nextCheckPointDist = (nextCheckPoint.position - transform.position).magnitude;
    }

    void FitnessUpdate()
    {
        distanceTraveled = totalCheckPointDist + (nextCheckPointDist-(nextCheckPoint.transform.position - transform.position).magnitude);
        if (fitness < distanceTraveled)
        {
            fitness = distanceTraveled;
        }
    }


    public void SetFirstMaterial() => renderer.material = firstMaterial; 
    public void SetDefaultMaterial() => renderer.material = defaultMaterial; 
    public void SetMutatedMaterial() => renderer.material = mutatedMaterial; 
    

    private void FixedUpdate()
    {
        InputUpdate();
        OutputUpdate();
        FitnessUpdate();
    }

    private void OutputUpdate()
    {
        net.FeedForward(inputs);
        carController.horizontalInput = net.neurons[^1][0].Ouput;
        carController.verticalInput = net.neurons[^1][1].Ouput;
    }

    private void InputUpdate()
    {
        carPosition = transform.position;
        inputs[0] = RaySensor(carPosition + Vector3.up * 0.2f, transform.forward, 4f);
        inputs[1] = RaySensor(carPosition + Vector3.up * 0.2f, transform.right, 1.5f);
        inputs[2] = RaySensor(carPosition + Vector3.up * 0.2f, -transform.right, 4f);
        inputs[3] = RaySensor(carPosition + Vector3.up * 0.2f, transform.forward+transform.right, 2f);
        inputs[4] = RaySensor(carPosition + Vector3.up * 0.2f, transform.forward-transform.right, 2f);
        inputs[5] = 1;

    }

    private RaycastHit hit;

    // travailler avec les mêmes inputs donner les meme inputs sinon perte de statbilité
    float RaySensor(Vector3 origin, Vector3 direction, float length)
    {
        Debug.DrawRay(origin, direction * hit.distance,
            Color.Lerp(Color.red, Color.green, Mathf.InverseLerp(length * rayRange, 0, hit.distance)));
        if (Physics.Raycast(origin, direction, out hit, length * rayRange, layerMask))
        {
            return Mathf.InverseLerp(length * rayRange, 0, hit.distance);
        }
        else
        {
            Debug.DrawRay(origin, direction*length*rayRange, Color.red);
            return 0;
        }
    }

    public void CheckPointReached(Transform checkPoint)
    {
        totalCheckPointDist += nextCheckPointDist;
        nextCheckPoint = checkPoint;
        nextCheckPointDist = (nextCheckPoint.position - transform.position).magnitude;
    }
}