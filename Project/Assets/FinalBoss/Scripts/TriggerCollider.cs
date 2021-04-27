using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//check if the player was hit by melee attack collider - Dvir
public class TriggerCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerStatus>().takeDamage(20,transform.forward,other.GetComponent<PlayerStatus>().unblockable);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
