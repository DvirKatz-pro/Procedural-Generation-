using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BulletCollider
// Collider script that calls back to the enemy
//
// Written by: Cal
public class BulletCollider : MonoBehaviour
{
    // Variables
    private PunisherEnemy enemy;

    // Initialize
    public void setEnemyAttackScript(PunisherEnemy punisherEnemy)
    {
        enemy = punisherEnemy;
    }

    // When we collide with another object
    private void OnCollisionEnter(Collision other)
    {
        // Call to the enemy that shot us that we hit
        enemy.hitPlayer(other.gameObject);
    }
}
