using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MoveToGround
// This script moves the position of the scrap to be perfectly on the ground via a raycast
// Source: https://answers.unity.com/questions/128719/spawning-above-the-terrain.html
//
// Written by: Cal
public class MoveToGround : MonoBehaviour
{
    // Variables
    public LayerMask mask = 9;

    // Start
    void Start()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 100, Vector3.down);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            if (hit.collider != null)
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
        }
    }
}