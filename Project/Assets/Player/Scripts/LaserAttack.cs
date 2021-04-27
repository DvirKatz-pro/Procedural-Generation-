using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Player Lazer Attack

public class LaserAttack : MonoBehaviour
{
    private bool aiming = false;

    //refernces to  where the lazer should start and spawn
    [SerializeField] private GameObject lazerSpawn;
    [SerializeField] private GameObject lazerStart;

    //refernces to particles
    private ParticleSystem lazerParticle;
    private ParticleSystem lazerStartParticle;

    //script references
    private CombatController controller;
    private PlayerStatus abilityPoints;
    private PlayerMovement move;

    //animator references
    private Animator animator;
    private void Start()
    {
        move = GetComponent<PlayerMovement>();
        controller = GetComponent<CombatController>();
        abilityPoints = GetComponent<PlayerStatus>();
        lazerParticle = lazerSpawn.GetComponent<ParticleSystem>();
        lazerStartParticle = lazerStart.GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();

    }
    private void Update()
    {

    }
    //activate lazer aiming
    public void laserAttack()
    {
        //if left shift is pressed then we disable or enable aiming
        if (Input.GetKeyDown(KeyCode.LeftShift) && abilityPoints.getAp() >= 200 && abilityPoints.getAp() < 300)
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

                controller.setState(CombatController.State.LazerAttack);
                attack();
            }

        }
        else if (aiming)
        {
            attack();
            move.rotate();
        }
    }
    //trigger lazer attack if aiming
    void attack()
    {
        //if left click is pressed and aiming is enabled, change animation, play particle effects, and start the attack
        if (Input.GetMouseButtonDown(0) && aiming)
        {
            // Call the start laser attack RPC
            if (!PhotonNetwork.OfflineMode)
                this.gameObject.GetComponent<PhotonView>().RPC("StartLaserAttackRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);

            animator.SetBool("Forword", false);
            animator.SetBool("Backword", false);
            animator.SetBool("Idle", true);
            move.setMove(false);
            abilityPoints.isImmortal = true;

            lazerParticle.Play();
            lazerStartParticle.Play();
            abilityPoints.changeAp(-200, 5);
            move.rotate();

            StartCoroutine(LazerAttack());

        }

    }
    //acivate lazer
    IEnumerator LazerAttack()
    {
        yield return new WaitForSeconds(5f);

        // Call the stop laser attack RPC
        if (!PhotonNetwork.OfflineMode)
            this.gameObject.GetComponent<PhotonView>().RPC("StopLaserAttackRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);

        //stop the attack and return to a normal state
        lazerParticle.Stop();
        lazerStartParticle.Stop();
        animator.SetBool("Idle", false);
        aiming = false;
        controller.setState(CombatController.State.Normal);
        abilityPoints.isImmortal = false;
        yield return new WaitForSeconds(0.5f);
    }


    // PUN RPCs (Written by Cal)

    [PunRPC]
    void StartLaserAttackRPC(int id)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (lazerParticle == null)
                lazerParticle = lazerSpawn.GetComponent<ParticleSystem>();
            if (lazerStartParticle == null)
                lazerStartParticle = lazerStart.GetComponent<ParticleSystem>();

            lazerParticle.Play();
            lazerStartParticle.Play();
        }
    }

    [PunRPC]
    void StopLaserAttackRPC(int id)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            lazerParticle.Stop();
            lazerStartParticle.Stop();
        }
    }
}
