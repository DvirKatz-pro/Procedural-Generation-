using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [NO LONGER IN USE]
// PunisherAttack
//
// Written by: Cal
public class PunisherAttack : EnemyAttack
{
    // Stats
    [SerializeField] int attackStrength;
    [SerializeField] float bulletSpeed;

    // Bullets
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private AudioClip laserShotSound;
    private GameObject bullet;
    private Vector3 shootPosition;
    private Vector3 positionToShoot;
    private int shotCount;
    private BulletCollider bulletCollider;

    private Transform bulletLocation1;
    private Transform bulletLocation2;
    private Transform bulletLocation3;
    private Transform bulletLocation4;

    // Manager
    private EnemyManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = this.GetComponent<EnemyManager>();

        shotCount = 0;
        bulletLocation1 = this.transform.Find("BulletLocation1");
        bulletLocation2 = this.transform.Find("BulletLocation2");
        bulletLocation3 = this.transform.Find("BulletLocation3");
        bulletLocation4 = this.transform.Find("BulletLocation4");

        this.enabled = false;
    }

    // Start is called before the first frame update
    void Update()
    {
        if (bullet != null) // The bullet exists
        {
            bullet.transform.position = bullet.transform.position + (positionToShoot * bulletSpeed * Time.deltaTime);
        }
        else // The bullet does not exist
        {
            this.enabled = false;
        }
    }

    // Called to iniate an attack
    public override void Attack()
    {
        if (bullet != null)
        {
            Destroy(bullet);
        }

        shotCount = shotCount % 4;
        switch (shotCount)
        {
            case 0:
                shootPosition = bulletLocation1.position;
                break;

            case 1:
                shootPosition = bulletLocation2.position;
                break;

            case 2:
                shootPosition = bulletLocation3.position;
                break;

            case 3:
                shootPosition = bulletLocation4.position;
                break;

            default:
                Debug.LogError("Shot count is not in range for " + this.gameObject.name);
                break;
        }
        shotCount++;

        bullet = Instantiate(bulletPrefab, shootPosition, Quaternion.identity);

        bulletCollider = bullet.GetComponent<BulletCollider>();
        //bulletCollider.setEnemyAttackScript(this);

        positionToShoot = manager.player.transform.position - bullet.transform.position + new Vector3(0.0f, 1.0f, 0.0f);

        manager.audioSource.PlayOneShot(laserShotSound, 0.3f);

        this.enabled = true;
        EndAttack();
    }

    public void hitPlayer(string collisionTag)
    {
        if (collisionTag.Equals("Player"))
        {
            // Do damage to the player
            PlayerStatus playerS = manager.player.GetComponent<PlayerStatus>();
            playerS.takeDamage(attackStrength, transform.forward);

        }
        Destroy(bullet);
        this.enabled = false;
    }

    // Called to end an attack
    public override void EndAttack()
    {
        manager.SetState("IDLE");
    }

}
