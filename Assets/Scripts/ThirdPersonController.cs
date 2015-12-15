using UnityEngine;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
    public GameObject focusPoint;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = focusPoint.transform.position - focusPoint.transform.forward * 10;
        gameObject.transform.rotation = focusPoint.transform.rotation;
    }
}
