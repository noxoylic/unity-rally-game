using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform forcePoint;

    public Wheel driveWheelRight;
    public Wheel driveWheelLeft;

    [Header("Divechain Variables")]
    public AnimationCurve engineCurve;
    public float differential;
    public float transmissionEfficiency;
    public float[] gear;
    public float driveTorque;

    public float rpm;
    public int i;

    float x;
    float y;

    void Start()
    {
        
    }

    void Update()
    {
        enginePower();
        inputs();
    }

    void FixedUpdate()
    {
        //apply air resistance
    }

    void enginePower()
    {
        rpm = (rb.velocity.magnitude / driveWheelLeft.wheelRadius) * gear[i] * differential * 60 / (2 * Mathf.PI);
        if (rpm < 1000)
        {
            rpm = 1000;
        }
        if(x > 0)
        {
            driveTorque = engineCurve.Evaluate(rpm) * gear[i] * differential * transmissionEfficiency * x;
        }
    }



    void inputs()
    {
        x = Input.GetAxis("Vertical");
        y = Input.GetAxis("Horizontal");
    }

    void shiftUp()
    {
        i++;
    }

    void shiftDown()
    {
        i--;
    }
}
