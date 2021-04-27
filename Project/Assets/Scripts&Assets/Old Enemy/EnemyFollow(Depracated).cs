using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [NO LONGER IN USE]
// EnemyFollow
//
// Written by: Cal
public class EnemyFollow : MonoBehaviour
{
    // Transforms
    public Transform enemy;
    public Transform player;

    // Movement
    private float currentSpeed;
    public float acceleration;
    public float maxSpeed;
    public float maxTurnRate;

    // Distances
    public float distanceToAttack;
    public float distanceToChase;

    // Animator
    Animator animator;

    // States: 0 idle, 1 chase, 2 attack
    int state;

    // Attack sphere
    SphereCollider attackCollider;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        attackCollider = GetComponent<SphereCollider>();

        state = 0;
        currentSpeed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (state != 2) // If enemy isn't currently attacking
        {
            float distanceToPlayer = Vector3.Distance(enemy.position, player.position);

            if (distanceToPlayer <= distanceToAttack) // If enemy is close enough to attack
            {
                StartCoroutine("Attack");
            }
            else if (distanceToPlayer <= distanceToChase) // If enemy is close enough to chase, chase
            {
                state = 1;
                ChasePlayer();
            }
            else if (state != 0) // If the enemy is not already idling, idle
            {
                currentSpeed = 0.0f;
                animator.SetBool("Dash Forward", false);
                state = 0;
            }
        }
    }

    IEnumerator Attack()
    {
        state = 2;
        currentSpeed = 0.0f;
        animator.SetTrigger("Stab Attack");
        Debug.Log("I attacked");
        yield return new WaitForSeconds(0.5f);

        // Get angle between enemy and player
        Vector3 enemyForward = enemy.forward;
        Vector3 playerDirection = player.position - enemy.position;
        float angle = Vector3.Angle(playerDirection, enemyForward);

        // Get distance to player
        float distanceToPlayer = Vector3.Distance(enemy.position, player.position);

        // Was player attacked successfuly
        if (angle <= 20.0f && distanceToPlayer <= 3.0f)
        {
            Debug.Log("Attack successful!");
        }
        else
        {
            Debug.Log("Attack unsuccessful!");
        }

        yield return new WaitForSeconds(1.0f);
        state = 0;
        animator.ResetTrigger("Stab Attack");
        Debug.Log("I am normal");
    }

    void ChasePlayer()
    {

        // Add accelaration until max speed
        currentSpeed = (currentSpeed + acceleration * Time.deltaTime < maxSpeed) ? currentSpeed + acceleration * Time.deltaTime : maxSpeed;

        // Get angle between enemy and player
        Vector3 enemyForward = enemy.forward;
        Vector3 playerDirection = player.position - enemy.position;
        float angle = Vector3.SignedAngle(playerDirection, enemyForward, Vector3.up);

        // Limit the turn of the enemy
        angle = (angle > maxTurnRate) ? maxTurnRate : angle;
        angle = (angle < -maxTurnRate) ? -maxTurnRate : angle;

        // Rotate the enemy
        enemy.Rotate(0.0f, -angle * 100 * Time.deltaTime, 0.0f, Space.Self);

        // Move towards the player
        animator.SetBool("Dash Forward", true);
        enemy.position += transform.forward * currentSpeed * Time.deltaTime;
    }

}
