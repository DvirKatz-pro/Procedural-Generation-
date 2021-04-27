using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LightManager
// Manages a light
//
// Written by: Cal
public class LightManager : MonoBehaviour
{
    Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = this.GetComponent<Light>();
        if (light == null)
            Debug.LogError("Light on " + this.name + " is null.");
    }

    // Update is called once per frame
    public void SetColour(float r, float g, float b)
    {
        light.color = new Color(r, g, b);
    }
}
