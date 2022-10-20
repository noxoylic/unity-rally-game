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
        //spin wheel model
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, springTravel + wheelRadius))
        {
            suspension(hit);
            calculateWeight(hit);
            applyForces(hit);
            calcualteSlipRatio();
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

    void applyForces(RaycastHit hit)
    {
        wheelVelocity = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));
        Vector3 rollingDrag = -rollingResistance * wheelVelocity;

        if(Input.GetAxis("Vertical") < 0)
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
                float finalTorque = controller.driveTorque - brakeTorque; 
                angularVelocity = finalTorque / (wheelMass * wheelRadius * wheelRadius / 2);
                rb.AddForceAtPosition(slipCurve.Evaluate(slipRatio) / 2 * transform.forward, hit.point);
            }
            else 
            {
                float finalTorque = -brakeTorque;
                angularVelocity = finalTorque / (wheelMass * wheelRadius * wheelRadius / 2);
                // add brake torque
            }
            //front wheel drive
        }
        else if(rwd)
        {
            if (!isFront)
            {
                float finalTorque = - brakeTorque;
                angularVelocity = finalTorque / (wheelMass * wheelRadius * wheelRadius / 2);
                rb.AddForceAtPosition(slipCurve.Evaluate(slipRatio) * transform.forward, hit.point);
            }
            else
            {
                float finalTorque = controller.driveTorque - brakeTorque;
                angularVelocity = finalTorque / (wheelMass * wheelRadius * wheelRadius / 2);
                //add brake torque
            }
            //rear wheel dirve
        }
        else if(awd)
        {
            float finalTorque = controller.driveTorque - brakeTorque;
            angularVelocity = finalTorque / (wheelMass * wheelRadius * wheelRadius / 4);
            rb.AddForceAtPosition(slipCurve.Evaluate(slipRatio) * transform.forward / 4, hit.point);
        }
    }

    void calculateWeight(RaycastHit hit)
    {
        float springPercentage = hit.distance / (springTravel + wheelRadius);
        weightOnWheel = (rb.mass / springPercentage) / 4;
    }

    void calcualteSlipRatio()
    {
        slipRatio = ((angularVelocity * wheelRadius) / wheelVelocity.magnitude);
    }
}
