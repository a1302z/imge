using UnityEngine;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
    public GameObject focusPoint;

    Quaternion optimalPositionR;
    Vector3 optimalPositionP;
    public float distanceToObjectZ;
    public float distanceToObjectY;

    void Start()
    {
        optimalPositionR = new Quaternion();
        optimalPositionP = new Vector3();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        optimalPositionP = focusPoint.transform.position - focusPoint.transform.forward * (distanceToObjectZ + focusPoint.transform.localScale.z) + focusPoint.transform.up * distanceToObjectY;
        optimalPositionR = focusPoint.transform.rotation;
        optimalPositionR *= Quaternion.Euler(new Vector3(10, 0, 0));

        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, optimalPositionR, 5 * Time.deltaTime);
        gameObject.transform.position = Vector3.Slerp(gameObject.transform.position, optimalPositionP, 5 * Time.deltaTime);
    }
}
