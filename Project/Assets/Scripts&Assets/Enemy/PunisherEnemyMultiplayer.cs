using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

// PunisherEnemyMultiplayer
// Punisher enemy for use in multiplayer
// !!! NOT IMPLEMENTED !!!
//
// Written by: Cal
public class PunisherEnemyMultiplayer : EnemyMultiplayer
{
    protected override void Start()
    {

    }
    protected override void Update()
    {

    }
    protected override void SetDestination(Vector3 position, float stoppingDistance)
    {

    }
    protected override void Attack(GameObject targerPlayer)
    {

    }
    public override void SetState(EnemyState newState)
    {

    }
    public override EnemyMType GetEnemyType()
    {
        return EnemyMType.Ranged;
    }
    public override void Damage(int damageAmount)
    {

    }
    public override void Death()
    {

    }
    public override float DistanceToPlayer(int viewID)
    {
        return 0;
    }
    public override void SetFocusPlayer(int viewID)
    {

    }
}
