using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Grenade
// Throws a grenade, compatible with both singleplayer and multiplayer
//
//  Written by: Cal
public class Grenade : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform grenadePosition;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private LayerMask mask;

    private CombatController controller;
    private Inventory inventory;
    private PlayerWeaponManager playerWeaponManager;
    private PlayerStatus status;

    private GameObject grenadeInstance;
    // Animation and audio
    private Animator animator;
    private AudioSource audioSource;
    [SerializeField] private AudioClip grenadePin;
    [SerializeField] private AudioClip grenadeExplosion;

    // Stats
    [SerializeField] private int damage;
    [SerializeField] private float radius;
    [SerializeField] private float maxRange;
    private float height = 3;
    private float gravity = -18;
    private bool grenadeActive;

    #endregion

    #region Main

    // Setup
    void Start()
    {
        controller = GetComponent<CombatController>();
        inventory = GetComponent<Inventory>();
        playerWeaponManager = GetComponent<PlayerWeaponManager>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        status = GetComponent<PlayerStatus>();
        grenadeActive = false;
    }

    public void handleAttack()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (!GetComponent<PhotonView>().IsMine)
            {
                return;
            }
        }

        // If the user presses G to throw a grenade, there is enough scrap, and there isnt a grenade already active
        if (Input.GetKey(KeyCode.G) && inventory.GetScrap() >= 100 && !grenadeActive)
        {
            // Reset the scrab bar and stats
            status.changeScrap(0);
            inventory.setScrap(0);
            grenadeActive = true;
            status.isImmortal = true;

            // Start grenade throw
            audioSource.PlayOneShot(grenadePin, 0.3f);
            animator.SetBool("Throw", true);
            playerWeaponManager.DisableWeapon();
            controller.setState(CombatController.State.GrenadeThrow);

            // Call grenade throw RPC if online
            if (!PhotonNetwork.OfflineMode)
                this.gameObject.GetComponent<PhotonView>().RPC("GrenadeThrowRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID);
        }

    }

    // Get the target position of the grenade by using the placement of the cursor on screen
    private Vector3 GetTargetPosition()
    {
        // Instantiate vector3 with point under page in case error
        Vector3 clickPosition = new Vector3(0, -10, 0);

        // Use raycast to get position in world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, mask))
        {
            clickPosition = hit.point;
            if (Vector3.Distance(clickPosition, this.transform.position) > maxRange)
            {
                clickPosition = clickPosition.normalized * maxRange;
            }
        }

        return clickPosition;
    }

    // Used to calculate the force required to land at position: https://www.youtube.com/watch?v=IvT8hjy6q4o
    private Vector3 GetLaunchVelocity()
    {
        Vector3 target = GetTargetPosition();

        float displacementY = target.y - grenadeInstance.transform.position.y;
        Vector3 displacementXZ = new Vector3(target.x - grenadeInstance.transform.position.x, 0, target.z - grenadeInstance.transform.position.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity));

        return velocityXZ + velocityY;
    }

    // Throw the grenade, is an animation event
    private void PlayerGrenadeThrowEvent()
    {
        // If I am online, and this is not my player, return
        if (!PhotonNetwork.OfflineMode)
        {
            if (!GetComponent<PhotonView>().IsMine)
                return;
        }

        // If online, room instantiate the grenade, otherwise instantiate normally
        if (!PhotonNetwork.OfflineMode)
            grenadeInstance = PhotonNetwork.Instantiate(grenadePrefab.name, grenadePosition.position, grenadePosition.rotation);
        else
            grenadeInstance = Instantiate(grenadePrefab, grenadePosition.position, grenadePosition.rotation);

        // Physics
        Rigidbody rb = grenadeInstance.GetComponent<Rigidbody>();
        Physics.gravity = Vector3.up * gravity;
        rb.velocity = GetLaunchVelocity();
        StartCoroutine(GrenadeExplosionRoutine());

        // Reset player
        animator.SetBool("Throw", false);
        playerWeaponManager.EnableWeapon();
        controller.setState(CombatController.State.Normal);
    }

    // Grenade explosion
    IEnumerator GrenadeExplosionRoutine()
    {
        // Wait
        yield return new WaitForSeconds(3f);

        // Instantiate explosion where the grenade is
        Vector3 explosionPosition = grenadeInstance.transform.position;
        GameObject explosion = Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
        audioSource.PlayOneShot(grenadeExplosion, 0.3f);
        CalculateDamage(explosion.transform.position);

        // If online, call grenade explosion RPC
        if (!PhotonNetwork.OfflineMode)
            this.gameObject.GetComponent<PhotonView>().RPC("GrenadeExplosionRPC", RpcTarget.All, this.gameObject.GetComponent<PhotonView>().ViewID, explosionPosition);

        yield return new WaitForSeconds(0.5f);

        // Destroy the grenade
        if (!PhotonNetwork.OfflineMode)
            PhotonNetwork.Destroy(grenadeInstance.GetComponent<PhotonView>());
        else
            Destroy(grenadeInstance);
        grenadeActive = false;

        // No longer immortal
        status.isImmortal = false;
    }

    // Calculate the damage that the grenade should do
    private void CalculateDamage(Vector3 position)
    {
        // Get colliders in range and for each one
        Collider[] objectsInRange = Physics.OverlapSphere(position, radius);
        foreach (Collider col in objectsInRange)
        {
            // If its an enemy, do damage
            if (col.gameObject.tag.Equals("Enemy"))
            {
                GameObject enemy = col.gameObject;
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.Damage(damage);
            }
            // If its a tutorial enemy / target, destroy it
            if (col.gameObject.tag.Equals("TutorialEnemy"))
            {
                GameObject enemyTarget = col.gameObject;
                TrainingTarget trainingTarget = enemyTarget.GetComponent<TrainingTarget>();
                trainingTarget.DestroyTarget();
            }
        }
    }

    #endregion

    #region RPCs

    // RPC for when the grenade is thrown
    [PunRPC]
    void GrenadeThrowRPC(int id)
    {
        // If this is not me, play grenade sound
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(grenadePin, 0.3f);
        }
    }

    // RPC for when the grenade explodes
    [PunRPC]
    void GrenadeExplosionRPC(int id, Vector3 explosionPos)
    {
        // If this is not me, play grenade explosion effect and sound
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (explosionPrefab == null)
                Debug.Log("Grenade null");
            Instantiate(explosionPrefab, explosionPos, Quaternion.identity);

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(grenadeExplosion, 0.3f);
        }
    }

    #endregion
}
