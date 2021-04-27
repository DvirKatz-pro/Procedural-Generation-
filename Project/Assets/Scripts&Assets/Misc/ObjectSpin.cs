using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ObjectSpin
// Used to spin an object
//
// Written by: Cal
public class ObjectSpin : MonoBehaviour
{
    // Variables
    public float xSpeed;
    public float ySpeed;
    public float zSpeed;

    // Update
    void Update()
    {
        this.transform.Rotate((0 + xSpeed) * Time.deltaTime, (0 + ySpeed) * Time.deltaTime, (0 + zSpeed) * Time.deltaTime);
    }
}
