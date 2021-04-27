using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EarthSpin
// Spins the earth in the main menu
// 
// Written by: Cal
public class EarthSpin : MonoBehaviour
{
    // Some of my best work
    void Update()
    {
        this.transform.Rotate(0.0f, -5.0f * Time.deltaTime, 0.0f, Space.Self);
    }
}
