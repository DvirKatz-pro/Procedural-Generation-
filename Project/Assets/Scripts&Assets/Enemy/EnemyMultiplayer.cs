using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// EnemyMultiplayer
// Base enemy type for multiplayer
//
// Written by: Cal

// States
public enum EnemyMState { Idle, Formation, Attack, Stunned, Dead };
public enum EnemyMType { Close, Ranged };

// Abstract methods
public abstract class EnemyMultiplayer : MonoBehaviourPunCallbacks
{
    protected abstract void Start();
    protected abstract void Update();
    protected abstract void SetDestination(Vector3 position, float stoppingDistance);
    protected abstract void Attack(GameObject targerPlayer);
    public abstract void SetState(EnemyState newState);
    public abstract EnemyMType GetEnemyType();
    public abstract void Damage(int damageAmount);
    public abstract void Death();
    public abstract float DistanceToPlayer(int viewID);
    public abstract void SetFocusPlayer(int viewID);
}
