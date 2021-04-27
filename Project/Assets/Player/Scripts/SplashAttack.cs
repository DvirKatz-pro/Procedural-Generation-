using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Script to control splash attack - Dvir/Cal
public class SplashAttack : MonoBehaviour
{


    //splash attack attributes
    [SerializeField] private float radius;
    [SerializeField] private float hitAngle;
    [SerializeField] private int damage;
    private bool aiming = false;

    [SerializeField] private GameObject splashSpawn;

    CombatController controller;
    PlayerStatus abilityPoints;
    Inventory inventory;
    GameObject particleSpawn;

    // Sounds
    [HideInInspector] public AudioSource audioSource;
    [SerializeField] private AudioClip splashAudio;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CombatController>();
        abilityPoints = GetComponent<PlayerStatus>();
        audioSource = GetComponent<AudioSource>();
        inventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    //check if we can splash attack
    public void splashAttack()
    {
        //
        //check to see if player can initiate a special attack
        if (Input.GetKeyDown(KeyCode.LeftShift) && abilityPoints.getAp() >= 100 && abilityPoints.getAp() < 200)
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
                controller.setState(CombatController.State.SpecialAttack);
                attack();
            }

        }

        else if (aiming)
        {
            attack();
        }
        //if the player is aiming, he may then press the mouse button to trigger a special attack


    }
    //check if splash attack is confirmed by mouse click
    void attack()
    {
        if (Input.GetMouseButtonDown(0) && aiming)
        {
            // Call the start splash attack RPC
            if (!PhotonNetwork.OfflineMode)
                this.gameObject.GetComponent<PhotonView>().RPC("StartSplashAttackRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);

            particleSpawn = Instantiate(splashSpawn, this.transform.position, this.transform.rotation);
            audioSource.PlayOneShot(splashAudio, 0.3f);
            aiming = false;
            abilityPoints.changeAp(-100);
            damageCheck();
            StartCoroutine(EndSplashAttack());
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

                Vector3 playerToEnemy = hitCollider.gameObject.transform.position - this.transform.position;
                playerToEnemy.Normalize();

                float angle = Vector3.Dot(playerToEnemy, this.transform.forward);

                if (angle > hitAngle)
                {
                    //Debug.Log("hit " + angle + " " + hitCollider.gameObject.name);

                    // Do damage to enemy (CAL)
                    GameObject enemy = hitCollider.gameObject;
                    Enemy enemyScript = enemy.GetComponent<Enemy>();
                    enemyScript.Damage(inventory.GetWeaponDamage() + 25);


                }
                else
                {
                    //Debug.Log("miss" + angle + " " + hitCollider.gameObject.name);
                }
            }
        }
    }
    //give some pause after the special attack
    IEnumerator EndSplashAttack()
    {
        // Call the stop splash attack RPC
        if (!PhotonNetwork.OfflineMode)
            this.gameObject.GetComponent<PhotonView>().RPC("StopSplashAttackRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);

        yield return new WaitForSeconds(0.5f);
        controller.setState(CombatController.State.Normal);
        yield return new WaitForSeconds(0.5f);
        Destroy(particleSpawn);

        // splashParticle.Stop();
    }

    // PUN RPCs (Written by Cal)

    [PunRPC]
    void StartSplashAttackRPC(int id)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            particleSpawn = Instantiate(splashSpawn, this.transform.position, this.transform.rotation);
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(splashAudio, 0.3f);
        }
    }

    [PunRPC]
    void StopSplashAttackRPC(int id)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            Destroy(particleSpawn.gameObject);
        }
    }
}
