using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Rigidbody rb;
    [Header("Grip and Traction")]
    public float friction;

    private float rollingResistance;


    [Header("Suspension")]
    public float springTravel;              //max length the spring can travel
    public float springStriffness;          
    public float damperStiffness;

    private float springLength;             
    private float springForce;             

    private float damperForce;              //current damper force
    private float springVelocity;           //current spring velocity
    private float lastLength;               //length of the spring the previous frame

    private Vector3 suspensionForce;        //spring force applied in upwards direction

    [Header("Wheel")]
    public GameObject wheelModel;           
    public float wheelRadius;

    void Start()
    {
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, springTravel + wheelRadius))
        {
            suspension(hit);

        }
    }

    void suspension(RaycastHit hit)
    {
        lastLength = springLength;                                                  //set spring length
        springLength = hit.distance - wheelRadius;                                  //calculate current spring length
        springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;         //calculate the rate of change of spring length
        springForce = springStriffness * (springTravel - springLength);
        damperForce = damperStiffness * springVelocity;

        suspensionForce = (springForce + damperForce) * transform.up;
        rb.AddForceAtPosition(suspensionForce, transform.position);
    }

    void forwardForces(RaycastHit hit)  
    {
        Vector3 wheelVelocity = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point)); 


    }
}
