using UnityEngine;
using System.Collections;

public class Wheels : MonoBehaviour
{

    public GameObject Vehicle;
    public Vector3 PositionInVehicleSpace;
    public float k;
    float normalLength;
    Vector3 force;
    Vector3 currentDistanceVector;
    Rigidbody vehicleRigidbody;
    Rigidbody wheelRigidbody;


    public Wheels[] otherWheels;
    float[] L_w;
    float[] l_w;
    Vector3 forceAccumulator;
    Vector3[] currentDistanceVectors;
    Rigidbody[] otherWheelRigidbodies;

    // Use this for initialization
    void Start()
    {
        vehicleRigidbody = Vehicle.GetComponent<Rigidbody>();
        wheelRigidbody = gameObject.GetComponent<Rigidbody>();

        gameObject.transform.position = Vehicle.transform.position + PositionInVehicleSpace;
        normalLength = Vector3.Magnitude(PositionInVehicleSpace);
        force = Vector3.zero;
        currentDistanceVector = PositionInVehicleSpace;



        currentDistanceVectors = new Vector3[otherWheels.Length];
        otherWheelRigidbodies = new Rigidbody[otherWheels.Length];
        L_w = new float[otherWheels.Length];
        l_w = new float[otherWheels.Length];
        for (int i = 0; i < otherWheels.Length; i++)
        {
            otherWheelRigidbodies[i] = otherWheels[i].GetComponent<Rigidbody>();
            currentDistanceVectors[i] = otherWheels[i].transform.position - gameObject.transform.position;
            L_w[i] = l_w[i] = Vector3.Magnitude(currentDistanceVectors[i]);

        }
        forceAccumulator = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentDistanceVector = gameObject.transform.position - Vehicle.transform.position;
        float l = Vector3.Magnitude(currentDistanceVector);
        force = -k * (l - normalLength) * (currentDistanceVector) / l;

        vehicleRigidbody.AddForce(-force);
        wheelRigidbody.AddForce(force);


        for (int i = 0; i < otherWheels.Length; i++)
        {
            forceAccumulator += -k * (l_w[i] - L_w[i]) * currentDistanceVectors[i] / l_w[i];
        }
        wheelRigidbody.AddForce(forceAccumulator);
        forceAccumulator = Vector3.zero;
    }
}
