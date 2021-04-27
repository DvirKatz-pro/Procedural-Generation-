using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [NO LONGER IN USE]
// PunisherDeath
//
// Written by: Cal
public class PunisherDeath : EnemyDeath
{
    // Audio Clips
    [SerializeField]
    private AudioClip deathAudio;

    // Manager
    private EnemyManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = this.GetComponent<EnemyManager>();
    }

    // Called to kill the enemy
    public override void Death()
    {
        manager.animator.SetTrigger("Die");
        manager.audioSource.PlayOneShot(deathAudio, 0.3f);
    }

    public void PunisherDestroyEvent()
    {
        Destroy(this.gameObject);
    }
}
