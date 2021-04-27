using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AreaTrigger
// Manages an area trigger
//
// Written by: Cal
public class AreaTrigger : MonoBehaviour
{
    // Variables
    private LevelManager levelManager;
    private BoxCollider boxCollider;

    // Initialize
    private void Start()
    {
        levelManager = GameObject.Find("GameManager").GetComponent<LevelManager>();
        if (levelManager == null)
            Debug.LogError(this.name + " has invalid LevelManager.");

        boxCollider = this.gameObject.GetComponent<BoxCollider>();
        if (boxCollider == null)
            Debug.LogError(this.name + " has invalid BoxCollider.");
    }

    // When player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            levelManager.IncrementArea();
            Destroy(this.gameObject);
        }
    }

    // Set this area as a trigger
    public void setAsTrigger()
    {
        boxCollider.isTrigger = true;
    }
}
