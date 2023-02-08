using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private CarController carController;

    private void Start()
    {
        NeuralNetwork network = new NeuralNetwork(new []{2,3,3,2});
        network.FeedForward(new []{0.5f,0.75f});
    }

    private void Update()
    {
        carController.horizontalInput = Input.GetAxis("Horizontal");
        carController.verticalInput = Input.GetAxis("Vertical");
    }
}
