using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AutoFXDestory
// Destroys an FX as soon as it finishes
//
// Written by: Cal
public class AutoFXDestroy : MonoBehaviour
{
    private ParticleSystem ps;

    public void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}