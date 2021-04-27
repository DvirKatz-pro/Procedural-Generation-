using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TrainingTarget
// Manages the training targes in the tutorial level
//
// Written by: Cal
public class TrainingTarget : MonoBehaviour
{
    // Variables
    public bool isDead;
    [SerializeField] private GameObject destroyFXPrefab;

    // Destory the target
    public void DestroyTarget()
    {
        isDead = true;
        Instantiate(destroyFXPrefab, this.transform);
        GetComponent<MeshRenderer>().enabled = false;
    }
}
