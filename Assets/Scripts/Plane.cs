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
    public float c_w_x;
    public float c_w_y;
    public float c_w_z;

    public float A_wing;
    public float A_crossSectionArea;
    public float A_crossSectionSide;

    public float breakforce;

    public float rotSpeed;

    // Use this for initialization
    void Start()
    {

        rigidbody = gameObject.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            //Debug.Log("up is pressed");
            rigidbody.AddForce(gameObject.transform.forward * power);//(power / rigidbody.velocity.magnitude));
        }
        if (gameObject.transform.position.y > 2)
        {
            float rotFactor = Time.deltaTime * rotSpeed;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                gameObject.transform.Rotate(gameObject.transform.forward, -rotFactor);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
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
            if (Input.GetKey(KeyCode.A))
            {
                gameObject.transform.Rotate(gameObject.transform.up, rotFactor);
            }
            if (Input.GetKey(KeyCode.D))
            {
                gameObject.transform.Rotate(gameObject.transform.up, -rotFactor);
            }
        }
        //lift = v^2* (rho/2)*A*c_a
        /* 
         float cosForwardToVelocity = Vector3.Dot(gameObject.transform.forward, rigidbody.velocity);
         Vector3 vVector = gameObject.transform.forward * cosForwardToVelocity;


         //Debug.Log("vVector: " + vVector);
         float lift = Vector3.Dot(vVector, vVector) * rho_air * c_a * A_wing / 2f;
         rigidbody.AddForce(gameObject.transform.up * lift);

         float drag = Vector3.Dot(rigidbody.velocity, rigidbody.velocity) * rho_air * c_w * (A_crossSectionArea * Mathf.Abs(component.z) + A_wing * Mathf.Abs(component.y)) / 2f;
         rigidbody.AddForce(rigidbody.velocity.normalized * (-drag));*/

        Vector3 component = Vector3.zero;
        if (!rigidbody.velocity.Equals(Vector3.zero))
        {
            component = transform.InverseTransformDirection(rigidbody.velocity);
        }
        float dragConst = rho_air / 2f;
        float drag_X = Mathf.Sign(component.x) * component.x * component.x * dragConst * A_crossSectionSide * c_w_x;
        float drag_Y = Mathf.Sign(component.y) * component.y * component.y * dragConst * A_wing * c_w_y;
        float tempZ = Mathf.Sign(component.z) * component.z * component.z;
        float drag_Z = tempZ * dragConst * A_crossSectionArea * c_w_z;
        float lift = tempZ * rho_air * c_a * A_wing / 2f;

        rigidbody.AddForce((lift - drag_Y) * transform.up - drag_X * transform.right - drag_Z * transform.forward);

        Debug.Log("Componentwise: " + component + ",\tDrag (" + drag_X + "," + drag_Y + "," + drag_Z + "),\tLift: " + lift);

        if (component.z > 0 && Input.GetKey(KeyCode.DownArrow))
        {
            if (transform.position.y > 2)
                rigidbody.AddForce(-transform.forward * breakforce);
            else rigidbody.AddForce(-transform.forward * breakforce * 4);
        }
    }
}
