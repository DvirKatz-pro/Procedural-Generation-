using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Photon.Pun;

//Player basic attack - Dvir/Cal
public class PlayerBasicAttack : MonoBehaviour
{

    //attack attributes

    [SerializeField] private float firstAttackRadius;
    [SerializeField] private float firstAttackForce;
    [SerializeField] private float attackForce;
    [SerializeField] private float attackRadius;
    [SerializeField] private float hitAngle = 0.3f;
    [SerializeField] private float AttackRate = 1.0f;
    [SerializeField] private float comboReset = 1.0f;
    [SerializeField] private float multiplier = 0.5f;
    [SerializeField] private GameObject currentWeaponSlot;

    private float multiplierResetTimer = 0.0f;
    private float multiplierResetTimerAmount = 4.0f;
    private int multiplierAmount = 1;
    private bool startTimer = false;
    private float nextAttack = 0;
    private float currentAttackForce;
    private float currentAttackRadius;
    private float currentCombo = 0;
    private bool isAttacking = false;
    private bool didIHit;


    private Vector3 closestEnemyPosition;
    //sound files
    public AudioClip[] swordSwingSounds = new AudioClip[4];
    public AudioClip[] swordStrikeSounds = new AudioClip[4];

    AudioSource audioSource;
    CharacterController characterC;
    Animator animator;
    ParticleSystem attackParticle;
    CombatController controller;
    PlayerMovement playerMovement;
    Inventory inventory;
    PlayerStatus status;
    UIElements uiElements;


    // Start is called before the first frame update

    void Start()
    {
        characterC = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CombatController>();
        attackParticle = this.transform.Find("Attack Particle System").GetComponent<ParticleSystem>();
        playerMovement = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
        inventory = GetComponent<Inventory>();
        status = GetComponent<PlayerStatus>();

        // UI components
        uiElements = GameObject.Find("GameCanvas").GetComponent<UIElements>();
        if (uiElements == null)
            Debug.LogError("UIElements on " + this.name + " is null.");

        currentWeaponSlot = uiElements.currentWeaponSlot;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {

            if (Time.time > multiplierResetTimer)
            {
                multiplierAmount = 1;
                startTimer = false;
            }

        }
    }
    /*
     * listen for left clicks and execute an attacking combo
     */
    public void handleAttack()
    {
        if (!isAttacking)
        {
            //listen for mouse click if can attack 
            if (Input.GetMouseButton(0) && Time.time > nextAttack)
            {
                //set the state to attacking and incrment current combo
                controller.setState(CombatController.State.Attacking);
                nextAttack = Time.time + AttackRate;
                isAttacking = true;
                currentCombo++;
                if (currentCombo > 4)
                {
                    currentCombo = 1;
                }
                //start the combo
                if (currentCombo == 1)
                {
                    nextAttack = Time.time + AttackRate;
                    animator.SetTrigger("Attack1");
                    currentAttackForce = firstAttackForce;
                    currentAttackRadius = firstAttackRadius;

                }
                else
                {
                    currentAttackForce = attackForce;
                    currentAttackRadius = attackRadius;
                    if (currentCombo == 2)
                    {
                        animator.SetTrigger("Attack2");
                    }
                    else if (currentCombo == 3)
                    {
                        animator.SetTrigger("Attack3");
                    }
                    else
                    {
                        //last attack of the combo, insert some additional pause to indicate this
                        nextAttack = Time.time + AttackRate + 0.2f;
                        animator.SetTrigger("Attack4");
                    }





                }

                playerMovement.rotate();
                attackParticle.Play();
                closestEnemyPosition = damageCheck();


            }
            else
            {
                //reset the combo if enough time has passed
                if ((Time.time - nextAttack) > comboReset && currentCombo != 0)
                {

                    resetCombo();

                    //Debug.Log("combo reset time");
                }

            }
        }
        else
        {
            attack();
        }
    }
    /*
     * handle player attacking movement
     */
    void attack()
    {

        //move the player forword in a deescalating way so as to not look instantatious
        Vector3 movement = transform.forward * currentAttackForce * Time.deltaTime;


        characterC.Move(movement);
        currentAttackForce -= currentAttackForce * 10 * Time.deltaTime;
        //if the player is close enough to the closest enemy, then stop moving
        if (currentAttackForce < 5.0f || Vector3.Distance(closestEnemyPosition, transform.position) <= 3.5f)
        {
            attackParticle.Stop();
            isAttacking = false;
            if (!PhotonNetwork.OfflineMode)
                this.gameObject.GetComponent<PhotonView>().RPC("StopAttackRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);
        }


    }
    /*
     * reset the combo
     */
    public void resetCombo()
    {
        currentCombo = 0;
        if (!Input.GetMouseButton(0))
        {
            controller.setState(CombatController.State.Normal);
            //currentState = State.Normal;
            // GetComponent<PlayerMovement>().enabled = true;
            if (Input.GetMouseButton(1))
            {
                animator.SetTrigger("Blocking");
            }
            else
            {
                animator.SetTrigger("Reset");
            }

        }

    }

    /*
     * check the angle and distance between the player and enemies
     */
    Vector3 damageCheck()
    {


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentAttackRadius);

        Vector3 closestDistance = new Vector3(0.0f, -200.0f, 0.0f);

        didIHit = false;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag.Equals("Enemy"))
            {

                Transform enemyTransform = hitCollider.gameObject.transform;
                Vector3 targetDir = enemyTransform.position - transform.position;
                targetDir = targetDir.normalized;
                //angle between player and enemy
                float angle = Vector3.Dot(targetDir, transform.forward);

                if (angle > hitAngle)
                {
                    //Debug.Log("hit " + angle + " " + hitCollider.gameObject.name);
                    didIHit = true;

                    // Do damage to enemy (CAL)
                    GameObject enemy = hitCollider.gameObject;
                    Enemy enemyScript = enemy.GetComponent<Enemy>();
                    if (enemyScript == null)
                    {
                        PinEnemyTraining enemyTrainingScript = enemy.GetComponent<PinEnemyTraining>();
                        if (enemyTrainingScript != null && enemyTrainingScript.combatAttackable)
                        {
                            enemyTrainingScript.Damage(inventory.GetWeaponDamage());
                        }
                    }
                    else
                    {

                        enemyScript.Damage(inventory.GetWeaponDamage());

                        //find out if this enemy is closer to player position than all the enemies so far
                        closestDistance = enemy.transform.position;
                        if (Vector3.Distance(enemy.transform.position, transform.position) < Vector3.Distance(closestDistance, transform.position))
                        {
                            closestDistance = enemy.transform.position;
                        }
                    }
                }
                else
                {
                    //Debug.Log("miss" + angle + " " + hitCollider.gameObject.name);f
                    didIHit = false;
                }
            }
        }
        //if an enemy was hit, play a strike sound otherwise play a swing sound
        if (didIHit)
        {

            PlaySwordStrike();
            Weapon currentWeapon = inventory.GetCurrentWeapon();
            currentWeapon.AffectDurability(2);
            //      Old inventor?
            //GameObject child1 = currentWeaponSlot.transform.GetChild(0).gameObject;
            //Image sprite = child1.GetComponent<Image>();
            //sprite.GetComponent<TextPopUp>().setText(stats.name, stats.damage, stats.getDurability());
            multiplierAmount++;
            multiplierResetTimer = Time.time + multiplierResetTimerAmount;
            startTimer = true;
            if (multiplierAmount >= 15)
            {
                multiplierAmount = 15;
            }
            status.changeAp(multiplier * multiplierAmount);

            if (!PhotonNetwork.OfflineMode)
                this.gameObject.GetComponent<PhotonView>().RPC("StartAttackRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID, true);
        }
        else
        {
            PlaySwordSwing();
            if (!PhotonNetwork.OfflineMode)
                this.gameObject.GetComponent<PhotonView>().RPC("StartAttackRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID, false);

        }
        return closestDistance;
        //GetComponent<PlayerMovement>().enabled = true;

    }

    private void PlaySwordSwing()
    {
        //Debug.Log("SWING");
        audioSource.PlayOneShot(swordSwingSounds[Random.Range(0, 4)], 0.3f);
    }

    private void PlaySwordStrike()
    {
        //Debug.Log("STRIKE");
        audioSource.PlayOneShot(swordStrikeSounds[Random.Range(0, 4)], 0.4f);
    }


    // PUN RPCs (Written by Cal)

    [PunRPC]
    void StartAttackRPC(int id, bool hit)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (attackParticle == null)
                attackParticle = this.transform.Find("Attack Particle System").GetComponent<ParticleSystem>();
            attackParticle.Play();

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            if (hit)
                PlaySwordStrike();
            else
                PlaySwordSwing();
        }
    }

    [PunRPC]
    void StopAttackRPC(int id)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            attackParticle.Stop();
        }
    }

}
