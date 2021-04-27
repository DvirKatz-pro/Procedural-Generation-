using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [NO LONGER IN USE]
// PinAttack
//
// Written by: Cal
public class PinAttack : EnemyAttack
{
    // Stats
    [SerializeField]
    int attackStrength;

    // Manager
    private EnemyManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = this.GetComponent<EnemyManager>();
    }

    // Called to iniate an attack
    public override void Attack()
    {
        manager.animator.SetTrigger("Stab Attack");
    }

    // Called to end an attack
    public override void EndAttack()
    {
        manager.animator.ResetTrigger("Stab Attack");
        manager.SetState("IDLE");
    }

    // Attack Event - Player when the attack animation hits its peak
    protected void PinAttackEvent()
    {
        // Get distance to player
        float distanceToPlayer = Vector3.Distance(this.transform.position, manager.player.transform.position);

        Vector3 enemyForward = this.transform.forward;
        Vector3 playerDirection = manager.player.transform.position - this.transform.position;
        float angle = Vector3.Angle(playerDirection, enemyForward);

        if (angle <= 20.0f && distanceToPlayer <= 3.0f)
        {
            // Do damage to the player
            PlayerStatus playerS = manager.player.GetComponent<PlayerStatus>();
            playerS.takeDamage(attackStrength, transform.forward);
        }
    }

    protected void PinAttackEndEvent()
    {
        EndAttack();
    }

}
