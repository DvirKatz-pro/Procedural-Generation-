using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// [NO LONGER IN USE]
// PunisherChase
//
// Written by: Cal
public class PunisherChase : EnemyChase
{
    // Manager
    private EnemyManager manager;

    // Navmesh agent
    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        manager = this.GetComponent<EnemyManager>();
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        // Disable script by default
        this.enabled = false;
    }

    void Update()
    {
        navMeshAgent.SetDestination(manager.player.transform.position);
    }

    // Called to iniate an attack
    public override void StartChase()
    {
        manager.animator.SetBool("Fly Forward", true);
        this.enabled = true;
    }

    // Called to end an attack
    public override void StopChase()
    {
        manager.animator.SetBool("Fly Forward", false);
        this.enabled = false;
    }
}
