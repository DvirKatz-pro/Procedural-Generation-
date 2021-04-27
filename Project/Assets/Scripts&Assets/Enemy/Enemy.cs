using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy
// Base enemy type for singleplayer
//
// Written by: Cal

// States
public enum EnemyState { Idle, Formation, Attack, Stunned, Dead };
public enum EnemyType { Close, Ranged };

// Abstract methods
public abstract class Enemy : MonoBehaviour
{
    protected abstract void Start();
    protected abstract void Update();
    protected abstract void SetDestination(Vector3 position, float stoppingDistance);
    protected abstract void Attack();
    public abstract void SetState(EnemyState newState);
    public abstract void SetFormationPosition(Vector3 pos);
    public abstract EnemyType GetEnemyType();
    public abstract void Damage(int damageAmount);
    public abstract void Death();
}
