using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [NO LONGER IN USE]
// EnemyHealth
//
// Written by: Cal
public class EnemyHealth : MonoBehaviour
{
    // Health
    [SerializeField]
    int maxHealth;
    private int currentHealth;

    // Manager
    EnemyManager manager;


    // Start is called before the first frame update
    void Start()
    {
        manager = this.GetComponent<EnemyManager>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    public void Hurt(int health)
    {
        currentHealth -= health;
        if (currentHealth <= 0)
        {
            manager.kill();
        }
        else
        {
            if (manager.isEnemyAttacking())
            {
                manager.eAttack.EndAttack();
            }
            manager.animator.SetTrigger("Take Damage");
            manager.SetState("STUNNED");
        }

    }

    public void PinStunnedEndEvent()
    {
        manager.SetState("IDLE");
    }

    public void Heal(int health)
    {
        currentHealth += health;
    }
}
