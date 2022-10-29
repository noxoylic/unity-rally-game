using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Rigidbody rb;
    public CarController controller;

    [Header("Drive Type")]
    public bool fwd;
    public bool rwd;
    public bool awd;

    [Header("Grip and Traction")]
    public AnimationCurve slipCurve;
    public float rollingResistance;
    public float brakeForce;

    public float frontResultantForce;
    public float rearResultantForce;

    public float brakeTorque;
    public float slipRatio;
    private float weightOnWheel;
    public float angularVelocity;
    private Vector3 wheelVelocity;

    [Header("Suspension")]
    public float springTravel;              
    public float springStriffness;          
    public float damperStiffness;

    private float springLength;             
    private float springForce;             

    private float damperForce;            
    private float springVelocity;          
    private float lastLength;               

    private Vector3 suspensionForce;       

    [Header("Wheel")]
    public GameObject wheelModel;           
    public float wheelRadius;
    public float wheelMass;
    public bool isFront;

    void Start()
    {
    }

    void Update()
    {
        //spin wheel
        calculateSlipRatio();

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, springTravel + wheelRadius))
        {
            calculateForces(hit);
        }
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, springTravel + wheelRadius))
        {
            suspension(hit);
            applyForces(hit);
        }
    }

    void suspension(RaycastHit hit)
    {
        lastLength = springLength;                                                  
        springLength = hit.distance - wheelRadius;                                  
        springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;         
        springForce = springStriffness * (springTravel - springLength);
        damperForce = damperStiffness * springVelocity;

        suspensionForce = (springForce + damperForce) * transform.up;
        rb.AddForceAtPosition(suspensionForce, transform.position);
    }

    void calculateForces(RaycastHit hit)
    {
        wheelVelocity = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));
        Vector3 rollingDrag = -rollingResistance * wheelVelocity;

        if(Input.GetAxis("Vertical") < 0 && rb.velocity.magnitude > 0.1)
        {
            brakeTorque = brakeForce * wheelRadius * Input.GetAxis("Vertical");
        }
        else
        {
            brakeTorque = 0;
        }

        if (fwd)
        {
            if (isFront) 
            {
                float springPercentage = hit.distance / (springTravel + wheelRadius);
                float finalTorque = controller.driveTorque - brakeTorque; 
                angularVelocity = finalTorque / (wheelMass * wheelRadius * wheelRadius / 2);
                frontResultantForce = slipCurve.Evaluate(slipRatio) / springPercentage;
            }
            else 
            {
                float springPercentage = hit.distance / (springTravel + wheelRadius);
                float finalTorque = -brakeTorque;
                angularVelocity = finalTorque / (wheelMass * wheelRadius * wheelRadius / 2);
                rearResultantForce = slipCurve.Evaluate(slipRatio) / springPercentage;
            }
        }
        else if(rwd)
        {
            if (!isFront)
            {
                float springPercentage = hit.distance / (springTravel + wheelRadius);
                float finalTorque = - brakeTorque;
                angularVelocity = finalTorque / (wheelMass * wheelRadius * wheelRadius / 2);
                rearResultantForce = slipCurve.Evaluate(slipRatio) / springPercentage;
            }
            else
            {
                float springPercentage = hit.distance / (springTravel + wheelRadius);
                float finalTorque = controller.driveTorque - brakeTorque;
                angularVelocity = finalTorque / (wheelMass * wheelRadius * wheelRadius / 2);
                frontResultantForce = slipCurve.Evaluate(slipRatio) / springPercentage;
            }
        }
        else if(awd)
        {
            float springPercentage = hit.distance / (springTravel + wheelRadius);
            float finalTorque = controller.driveTorque + brakeTorque;
            angularVelocity = finalTorque / (wheelMass * wheelRadius * wheelRadius / 2);
            frontResultantForce = (slipCurve.Evaluate(slipRatio) / 4) / springPercentage;
            rearResultantForce = (slipCurve.Evaluate(slipRatio) / 4) / springPercentage;
        }
    }

    void applyForces(RaycastHit hit)
    {
        if (isFront)
        {
            rb.AddForceAtPosition(frontResultantForce * transform.forward, hit.point);
        }

        if (!isFront)
        {
            rb.AddForceAtPosition(rearResultantForce * transform.forward, hit.point);
        }
    }

    void calculateSlipRatio()
    {
        if (Input.GetAxis("Vertical") == -1)
        {
            slipRatio = -6;
        }
        else
        {
            slipRatio = (angularVelocity * wheelRadius) / rb.velocity.magnitude;
        }
    }
}
