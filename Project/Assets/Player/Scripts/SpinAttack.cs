using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Script to control spin attack - Dvir/Cal
public class SpinAttack : MonoBehaviourPunCallbacks
{
    //attack attributes
    [SerializeField] private float radius;
    [SerializeField] private int damage;
    private bool aiming = false;
    private bool isAttacking = false;
    private bool canDamage = true;

    //particle system reference
    [SerializeField] private ParticleSystem tornado;

    //script references 
    private CombatController controller;
    private PlayerStatus abilityPoints;
    private Animator animator;
    private Inventory inventory;
    // Sounds
    //[HideInInspector] public AudioSource audioSource;
    //[SerializeField] private AudioClip splashAudio;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CombatController>();
        abilityPoints = GetComponent<PlayerStatus>();
        animator = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
        // audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        //check if still attacking
        if (isAttacking && canDamage)
        {
            canDamage = false;
            StartCoroutine(DamagePause());
            damageCheck();
            //transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 1);

        }
    }
    //check if we can activate spin attack
    public void spinAttack()
    {
        //
        //check to see if player can initiate a special attack
        if (Input.GetKeyDown(KeyCode.LeftShift) && abilityPoints.getAp() == 300 && !isAttacking)
        {
            //set the player to be aiming or turn it off
            if (aiming)
            {
                aiming = false;
                controller.setState(CombatController.State.Normal);
            }
            else
            {
                aiming = true;
                controller.setState(CombatController.State.SpinAttack);
                attack();
            }

        }

        else if (aiming)
        {
            attack();
        }
        //if the player is aiming, he may then press the mouse button to trigger a special attack


    }
    //check if spin attack is confirmed by mouse click
    void attack()
    {
        if (Input.GetMouseButtonDown(0) && aiming)
        {
            // Call the stop spin attack RPC
            if (!PhotonNetwork.OfflineMode)
                this.gameObject.GetComponent<PhotonView>().RPC("StartSpinAttackRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);

            tornado.Play();
            animator.SetBool("Spin", true);
            abilityPoints.isImmortal = true;
            //audioSource.PlayOneShot(splashAudio, 0.3f);
            aiming = false;
            isAttacking = true;
            abilityPoints.changeAp(-300, 8);

            damageCheck();

            StartCoroutine(SpinAttackRoutine());
        }

    }
    /**
     * check if damage was done using distance and angle
     */
    void damageCheck()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag.Equals("Enemy"))
            {

                GameObject enemy = hitCollider.gameObject;
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript == null)
                {
                    PinEnemyTraining enemyTrainingScript = enemy.GetComponent<PinEnemyTraining>();
                    if (enemyTrainingScript != null && !enemyTrainingScript.combatAttackable)
                    {
                        enemyTrainingScript.Damage(inventory.GetWeaponDamage());
                    }
                }
                else
                {
                    enemyScript.Damage(inventory.GetWeaponDamage());
                }
            }
        }
    }
    //give some pause after the special attack
    IEnumerator SpinAttackRoutine()
    {
        yield return new WaitForSeconds(8f);

        // Call the stop spin attack RPC
        if (!PhotonNetwork.OfflineMode)
            this.gameObject.GetComponent<PhotonView>().RPC("StopSpinAttackRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);

        tornado.Stop();
        isAttacking = false;
        animator.SetBool("Spin", false);
        controller.setState(CombatController.State.Normal);
        abilityPoints.isImmortal = false;
        yield return new WaitForSeconds(0.5f);

        // splashParticle.Stop();
    }
    IEnumerator DamagePause()
    {
        yield return new WaitForSeconds(0.5f);
        canDamage = true;
    }

    // PUN RPCs (Written by Cal)

    [PunRPC]
    void StartSpinAttackRPC(int id)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            tornado.Play();
        }
    }

    [PunRPC]
    void StopSpinAttackRPC(int id)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            tornado.Stop();
        }
    }
}
