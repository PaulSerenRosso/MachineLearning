using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float maxSteerAngle = 42;
    [SerializeField] private float motorForce = 800;

    [SerializeField] private WheelCollider wheelColliderFrontLeftCollider,
        wheelColliderFrontRightCollider,
        wheelColliderRearLeftCollider,
        wheelColliderRearRightCollider;
    [SerializeField] private Transform wheelColliderFrontLeftModel,
        wheelColliderFrontRightModel,
        wheelColliderRearLeftModel,
        wheelColliderRearRightModel;

    public float horizontalInput;
    public float verticalInput;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform centerOfMass;
    private Vector3 pos;
    private Quaternion rot;

    private void Start()
    {
        rb.centerOfMass = centerOfMass.localPosition;
    }

    private void FixedUpdate()
    {
        Steer();
        Accelerate();
        UpdateWheelsModel();
    }

    void Steer()
    {
        wheelColliderFrontLeftCollider.steerAngle = horizontalInput * maxSteerAngle;
        wheelColliderFrontRightCollider.steerAngle = horizontalInput * maxSteerAngle;

    }

    void Accelerate()
    {
        wheelColliderRearLeftCollider.motorTorque = verticalInput * motorForce;
        wheelColliderRearRightCollider.motorTorque = verticalInput * motorForce;
    }

    void UpdateWheelsModel()
    {
        UpdateWheelModel(wheelColliderFrontRightCollider, wheelColliderFrontRightModel);
        UpdateWheelModel(wheelColliderFrontLeftCollider, wheelColliderFrontLeftModel);
        UpdateWheelModel(wheelColliderRearLeftCollider, wheelColliderRearLeftModel);
        UpdateWheelModel(wheelColliderRearRightCollider, wheelColliderRearRightModel);
    }

    void UpdateWheelModel(WheelCollider collider, Transform tr)
    {
        pos = tr.position;
        rot = tr.rotation;
        collider.GetWorldPose(out pos, out rot);

        tr.position = pos;
        tr.rotation = rot;
    }

    public void Reset()
    {
        horizontalInput = 0;
        verticalInput = 0; 
    }
}
