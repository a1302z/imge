using UnityEngine;
using System.Collections;

/*
 * Class, that simulates the behaviour of a plane
 * Therefore there will be forces added for:
 * -Propulsion: Force in x Direction of plane
 * -Lift depending on velocity: Force in y Direction of Plane
 * -Air Resistance depending on velocity: Force in -x Direction of Plane
 * 
 * 
 * 
 */

public class Plane : MonoBehaviour
{

    Rigidbody rigidbody;
    public float power;

    public float c_a;
    private float rho_air = 1.2041f; //in kg/m³
    public float c_w;

    public float A;


    public float rotSpeed;

    // Use this for initialization
    void Start()
    {

        rigidbody = gameObject.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("up is pressed");
            rigidbody.AddForce(gameObject.transform.forward * power);//(power / rigidbody.velocity.magnitude));
        }
        if (gameObject.transform.position.y > 2)
        {
            float rotFactor = Time.deltaTime * rotSpeed;
            if (Input.GetKey(KeyCode.D))
            {
                gameObject.transform.Rotate(gameObject.transform.forward, -rotFactor);
            }
            if (Input.GetKey(KeyCode.A))
            {
                gameObject.transform.Rotate(gameObject.transform.forward, rotFactor);
            }
            if (Input.GetKey(KeyCode.S))
            {
                gameObject.transform.Rotate(gameObject.transform.right, -rotFactor);
            }
            if (Input.GetKey(KeyCode.W))
            {
                gameObject.transform.Rotate(gameObject.transform.right, rotFactor);
            }
        }
        //lift = v^2* (rho/2)*A*c_a
        Vector3 vVector = gameObject.transform.forward * Vector3.Dot(gameObject.transform.forward, rigidbody.velocity);
        Debug.Log("vVector: " + vVector);
        float lift = Vector3.Dot(vVector, vVector) * rho_air * c_a * A / 2f;
        rigidbody.AddForce(gameObject.transform.up * lift);

        float drag = Vector3.Dot(rigidbody.velocity, rigidbody.velocity) * rho_air * c_w * A / 2f;
        rigidbody.AddForce(rigidbody.velocity * (-drag));
    }
}
