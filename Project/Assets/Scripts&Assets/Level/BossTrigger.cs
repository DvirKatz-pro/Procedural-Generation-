using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BossTrigger
// Triggers the boss battle
//
// Written by: Cal
public class BossTrigger : MonoBehaviour
{
    // Variables
    private LevelManager manager;

    // Initialize
    private void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<LevelManager>();
        if (manager == null)
            Debug.LogError(this.name + " has invalid LevelManager.");
    }

    // When player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            manager.InitializeBoss();
            Destroy(this.gameObject);
        }
    }
}