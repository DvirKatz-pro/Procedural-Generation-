using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TutorialAreaTrigger
// Manages the triggers between the rooms in the tutorial level
//
// Written by: Cal
public class TutorialAreaTrigger : MonoBehaviour
{
    // Tutorial Area
    private TutorialManager manager;
    private BoxCollider boxCollider;
    public LightManager lightManager;
    private bool trigerred;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        if (manager == null)
            Debug.LogError(this.name + " has invalid TutorialManager.");

        boxCollider = this.gameObject.GetComponent<BoxCollider>();
        if (boxCollider == null)
            Debug.LogError(this.name + " has invalid BoxCollider.");

        lightManager = this.gameObject.GetComponentInChildren<LightManager>();
        if (lightManager == null)
            Debug.LogError(this.name + " has invalid light manager.");
    }

    // When player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !trigerred)
        {
            manager.NextTutorialArea();
            trigerred = true;
        }
    }

    // Set this area as a trigger
    public void Advance()
    {
        boxCollider.isTrigger = true;
        lightManager.SetColour(0.0f, 1.0f, 0.0f);
    }
}
