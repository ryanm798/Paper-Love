using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnchor : MonoBehaviour
{
    [Tooltip("degrees / second")]
    public float rotationSpeed = 10f;
    public bool clockwise = false;

    void Start()
    {
        transform.GetChild(0).LookAt(transform);
    }

    void Update()
    {
        float dirMult = 1f;
        if (clockwise) dirMult = -1f;

        transform.RotateAround(transform.position, Vector3.up, dirMult * rotationSpeed * Time.deltaTime);
    }
}
