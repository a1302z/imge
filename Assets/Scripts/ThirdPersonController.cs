using UnityEngine;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
    public GameObject focusPoint;

    Quaternion optimalPositionR;
    Vector3 optimalPositionP;
    public float distanceToObjectZ;
    public float distanceToObjectY;
    float slerpfactor;
    public float maxVel = 250f;
    public float minSlerping = 0.1f;
    Rigidbody rb;

    void Start()
    {
        rb = focusPoint.GetComponent<Rigidbody>();
        optimalPositionR = new Quaternion();
        optimalPositionP = new Vector3();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        optimalPositionP = focusPoint.transform.position - focusPoint.transform.forward * (distanceToObjectZ + focusPoint.transform.localScale.z) + focusPoint.transform.up * distanceToObjectY;
        optimalPositionR = focusPoint.transform.rotation;
        optimalPositionR *= Quaternion.Euler(new Vector3(10, 0, 0));

        //slerpfactor should be in [0.75, 1]
        if (rb != null)
        {
            slerpfactor = minSlerping + (1f - minSlerping) * Mathf.Clamp(Mathf.Abs((Vector3.Magnitude(rb.velocity))) / maxVel, 0, 1f);
            //Debug.Log("Slerpfactor: " + slerpfactor + "\tTime: " + Time.deltaTime);
        }
        else slerpfactor = 1f;

        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, optimalPositionR, slerpfactor);
        gameObject.transform.position = Vector3.Slerp(gameObject.transform.position, optimalPositionP, slerpfactor);
    }
}
