using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//boss teleport ability - Dvir
public class Teleport : MonoBehaviour
{
    //set the references
    [SerializeField] private GameObject player;
    [SerializeField] private Transform topRight;
    [SerializeField] private Transform bottomLeft;
    [SerializeField] private GameObject portal;

    [SerializeField] private AudioClip teleportSound;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    /*
     * calculate possible positions for boss to teleport to
     * minRadius - the minimum distance the boss can teleport to
     * maxRadius - the minimum distance the boss can teleport to
     */
    public void teleport(float minRadius,float maxRadius)
    {
        this.GetComponent<FirstBossEnemy>().isTeleporting = true;
        Vector3 playerPosition = player.transform.position;

        //calculate possible positions within the radiuses 
        RaycastHit hit;
        Vector3 circleVector;
        List<Vector3> positions = new List<Vector3>();

        #region Directions
        //fire a raycast in 8 directions, generate a point between a minimum radius point on a circle around the player and a maximum point on a circle around the player in relation to the rest of the arena 
        //if a returned position has a y that is less than 0, then a position was not found for that direction, so its not added to the list of possible positions to teleport to
        //north
        circleVector = Quaternion.Euler(0, 90, 0) * Vector3.forward;
        if (Physics.Raycast(playerPosition, circleVector, out hit))
        {
            Vector3 posiblePosition = getPosition(90, minRadius, maxRadius, hit);
            if (posiblePosition.y >= 0)
            {
                positions.Add(posiblePosition);
            }
          
        }
        
        circleVector = Quaternion.Euler(0, 45, 0) * Vector3.forward;

        //north East
        //circleVector = transform.position * Mathf.Cos((Mathf.Deg2Rad * 45));
        if (Physics.Raycast(playerPosition, circleVector, out hit))
        {
            Vector3 posiblePosition = getPosition(45, minRadius, maxRadius, hit);
            if (posiblePosition.y >= 0)
            {
                positions.Add(posiblePosition);
            }
           
        }
        //East
         circleVector = Quaternion.Euler(0, 0, 0) * Vector3.forward;
        if (Physics.Raycast(playerPosition, circleVector, out hit))
        {
            Vector3 posiblePosition = getPosition(0, minRadius, maxRadius, hit);
            if (posiblePosition.y >= 0)
            {
                positions.Add(posiblePosition);
            }

        }

        //southEast
        circleVector = Quaternion.Euler(0, 315, 0) * Vector3.forward;
        if (Physics.Raycast(playerPosition, circleVector, out hit))
        {
            Vector3 posiblePosition = getPosition(315, minRadius, maxRadius, hit);
            if (posiblePosition.y >= 0)
            {
                positions.Add(posiblePosition);
            }
          

        }
        //south
        circleVector = Quaternion.Euler(0, 270, 0) * Vector3.forward;
        if (Physics.Raycast(playerPosition, circleVector, out hit))
        {
            Vector3 posiblePosition = getPosition(270, minRadius, maxRadius, hit);
            if (posiblePosition.y >= 0)
            {
                positions.Add(posiblePosition);
            }
        

        }
        //southWest
        circleVector = Quaternion.Euler(0, 225, 0) * Vector3.forward;
        if (Physics.Raycast(playerPosition, circleVector, out hit))
        {
            Vector3 posiblePosition = getPosition(225, minRadius, maxRadius, hit);
            if (posiblePosition.y >= 0)
            {
                positions.Add(posiblePosition);
            }
           

        }
        //West
        circleVector = Quaternion.Euler(0, 180, 0) * Vector3.forward;
        if (Physics.Raycast(playerPosition, circleVector, out hit))
        {
            Vector3 posiblePosition = getPosition(180, minRadius, maxRadius, hit);
            if (posiblePosition.y >= 0)
            {
                positions.Add(posiblePosition);
            }
           

        }
        //northWest
        circleVector = Quaternion.Euler(0, 135, 0) * Vector3.forward;
        if (Physics.Raycast(playerPosition, circleVector, out hit))
        {
            Vector3 posiblePosition = getPosition(135, minRadius, maxRadius, hit);
            if (posiblePosition.y >= 0)
            {
                positions.Add(posiblePosition);
            }
           
        }
        #endregion
        
        StartCoroutine(TeleportMove(positions));
        Vector3 position = player.transform.position;

        Vector3 targetDirection = this.transform.position - position;
        targetDirection *= -1;
        float singleStep = 5 * Time.deltaTime;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        
        transform.rotation = Quaternion.LookRotation(newDirection);

    }
    /*
     * check if a position is possible
     * angle - angle of position relative to player
     * minRadius - the minimum distance the boss can teleport to
     * maxRadius - the minimum distance the boss can teleport to
     * hit - the ray of the possible position
     */
    private Vector3 getPosition(float angle, float minRadius,float maxRadius, RaycastHit hit)
    {
        Vector3 circlePoint = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 maxCirclePoint = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 min, max;
        //see if raycast hit the arena edge
        if (hit.collider.gameObject.tag == "Bound")
        {
            //calculate the circle point for the minimum distance around the player
            circlePoint.x = player.transform.position.x + minRadius * Mathf.Cos((Mathf.Deg2Rad * angle));
            circlePoint.y = 0;
            circlePoint.z = player.transform.position.z + minRadius * Mathf.Sin((Mathf.Deg2Rad * angle));
            //calculate the circle point for the maximum distance around the player
            maxCirclePoint.x = player.transform.position.x + maxRadius * Mathf.Cos((Mathf.Deg2Rad * angle));
            maxCirclePoint.y = 0;
            maxCirclePoint.z = player.transform.position.z + maxRadius * Mathf.Sin((Mathf.Deg2Rad * angle));
            min = circlePoint;
            max = maxCirclePoint;
            //check if both points are inside the arena
            if (insideArena(min))
            {
                if (!insideArena(max))
                {
                    //if the maximum point is not inside the arena, then the area edge is now the max
                    max = hit.point;
                }
                //get a random point between the maximum and the minimum
                Vector3 posiblePosition = new Vector3();
                posiblePosition.x = Random.Range(min.x, max.x);
                posiblePosition.y = 0;
                posiblePosition.z = Random.Range(min.z, max.z);
                return posiblePosition;
            }

        }
        //if for some reason a point was not found, return a vector with a y that is less than 0
        return new Vector3(0, -100, 0);
    }

    /*
     * check if a point is inside the arena
     * point - the given point
     */
    private bool insideArena(Vector3 point)
    {
        //check to see if a given point is inside the arena using a reference to the top right and the bottomleft points of the arena 
        if (point.x > bottomLeft.position.x && point.z > bottomLeft.position.z && point.x < topRight.position.x && point.z < topRight.position.z)
        {
            return true;
        }
        return false;
    }
    //move to one of the possible positions
    public IEnumerator TeleportMove(List<Vector3> m_positions)
    {
        if (this.GetComponent<FirstBossEnemy>().enableTeleport)
        {
            //play the teleport particle
           
            Vector3 position = this.transform.position;
            position.y += 1.7f;
            position.x -= 0.5f;
            position.z -= 0.5f;
            GameObject portalInstance = Instantiate(portal, position, Quaternion.identity);
            ParticleSystem particle = portalInstance.GetComponent<ParticleSystem>();
            particle.Play();
            yield return new WaitForSeconds(0.2f);
         
            
            //get one of the avialable positions and play the teleport particle at that position then move the boss to that position
            int number = Random.Range(0, m_positions.Count);
            position = m_positions[number];
            position.y += 1.7f;
            position.x -= 0.5f;
            position.z -= 0.5f;
            GameObject otherPortalInstance = Instantiate(portal, position, Quaternion.identity);
            ParticleSystem otherParticle = otherPortalInstance.GetComponent<ParticleSystem>();
            otherParticle.Play();
            audioSource.PlayOneShot(teleportSound, 0.5f);
            this.transform.position = m_positions[number];
            this.transform.LookAt(player.transform.position);
            yield return new WaitForSeconds(0.5f);
            otherParticle.Stop();
            particle.Stop();
            
            this.GetComponent<FirstBossEnemy>().isTeleporting = false;
            yield return new WaitForSeconds(0.5f);
            Destroy(portalInstance);
            Destroy(otherPortalInstance);
        }
        

    }

   
}
