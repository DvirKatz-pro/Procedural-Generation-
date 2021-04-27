using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Player Dash ability - Dvir/Cal
public class PlayerDash : MonoBehaviour
{
    //dash attributes
    [SerializeField] private float dodgeRate = 1.0f;
    [SerializeField] private float dodgingForce = 1.0f;
    private float currentDodgeForce;
    private float nextDodge;
    private bool dashing = false;

    private CharacterController characterC;
    private CombatController controller;
    private ParticleSystem dashParticle;
    private Animator animator;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        characterC = GetComponent<CharacterController>();
        controller = GetComponent<CombatController>();
        dashParticle = this.transform.Find("Dodge Particle System").GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    /**
     * listen for the space bar
     */
    public void handleDash()
    {
        if (!dashing)
        {
            if (Input.GetKey(KeyCode.Space) && !Input.GetMouseButton(0) && Time.time > nextDodge)
            {
                dashParticle.Play();
                nextDodge = Time.time + dodgeRate;
                currentDodgeForce = dodgingForce;
                dashing = true;
                controller.setState(CombatController.State.Dashing);
                playerMovement.rotate();

                if (!PhotonNetwork.OfflineMode)
                {
                    this.gameObject.GetComponent<PhotonView>().RPC("StartDashRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);
                }

            }

        }
        else
        {
            dash();
        }
    }
    //Trigger dash
    void dash()
    {

        //move the player based in a deescalating way so that it doesnt look instantanious
        Vector3 movement = transform.forward * currentDodgeForce * Time.deltaTime;


        //Debug.Log(movement);
        characterC.Move(movement);
        currentDodgeForce -= currentDodgeForce * 10 * Time.deltaTime;
        if (currentDodgeForce < 5.0f)
        {
            dashParticle.Stop();
            if (Input.GetMouseButton(1))
            {
                animator.SetTrigger("Blocking");
                controller.setState(CombatController.State.Blocking);
            }
            else
            {
                animator.SetTrigger("Idle");
                controller.setState(CombatController.State.Normal);
            }

            dashing = false;

            if (!PhotonNetwork.OfflineMode)
                this.gameObject.GetComponent<PhotonView>().RPC("StopDashRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    // PUN RPCs (Written by Cal)

    [PunRPC]
    void StartDashRPC(int id)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (dashParticle == null)
                dashParticle = this.transform.Find("Dodge Particle System").GetComponent<ParticleSystem>();
            dashParticle.Play();
        }
    }

    [PunRPC]
    void StopDashRPC(int id)
    {
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            dashParticle.Stop();
        }
    }
}

