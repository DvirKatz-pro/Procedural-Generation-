using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for boss bullet control - Dvir
public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
      
        //if we hit the player the player should take damage
        if (collision.gameObject.tag == "Player")
        {
            PlayerStatus status = collision.gameObject.GetComponent<PlayerStatus>();
            status.takeDamage(10, transform.forward, status.unblockable);
            Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
            Destroy(this.gameObject);
        }
        //if we hit the outside border destroy the bullet
        else if (collision.gameObject.tag == "Bound")
        {
            Destroy(this.gameObject);
            
        }
        //otherwise ignore the collision
        else
        {
            Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
        }

    }

    [SerializeField] private float speed = 5.0f;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(TravelTime());
    }

    // Update is called once per frame
    void Update()
    {
        //move forword
        rigidbody.velocity = transform.forward.normalized * (speed * Time.deltaTime);
    }

    //if the bullet is traveling too long and hasnt been destroyed, then destroy it
    IEnumerator TravelTime()
    {
        yield return new WaitForSeconds(10);
        Destroy(this);
    }
    /*
     * set the speed of a bullet
     */
    public void setSpeed(float m_speed)
    {
        speed = m_speed;
    }
}
