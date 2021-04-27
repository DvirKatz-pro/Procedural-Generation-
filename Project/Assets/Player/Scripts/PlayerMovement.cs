using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * script incharge of player movement - Dvir
 */
public class PlayerMovement : MonoBehaviour
{
    //script refernces
    private CharacterController characterC;
    private Animator animator;

    //gameplay related values
    [SerializeField] private float playerSpeed = 5.0f;
    private Vector3 lookDir;
    private int floorMask;
    private bool move = true;
    private bool rot = true;
    private bool shouldAnimate = true;



    // Start is called before the first frame update
    void Start()
    {
        characterC = GetComponent<CharacterController>();
        floorMask = LayerMask.GetMask("Terrain");
        animator = GetComponent<Animator>();
    }
   
    public void Update()
    {
        //check if player is grounded
        if (!characterC.isGrounded)
        {
            characterC.Move(Physics.gravity * Time.deltaTime);
        }
        if (move)
        {
            moving();
        }
     
    }
    public bool isMoving()
    {
        return move;
    }
    public void moving()
    {
        
        rotate();
        //find the mouse pointer and move to that location
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 movement = Vector3.zero;
        if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity, floorMask))
        {

            if (movement.y > 0)
            {
                movement.y = 0;
            }

            //if "W" hit move forword
            if (Input.GetKey(KeyCode.W) && Vector3.Distance(this.transform.position, hit.point) > 0.0f)
            {
                movement = transform.forward * playerSpeed;
                characterC.SimpleMove(movement);

                animate("Forword", true);
                animate("Backword", false);
                animate("Idle", false);
            }
            //if "S" hit move backword
            else if (Input.GetKey(KeyCode.S) && Vector3.Distance(this.transform.position, hit.point) > 0.0f)
            {
                movement = (transform.forward * -1) * playerSpeed;
                characterC.SimpleMove(movement);
                animate("Forword", false);
                animate("Backword", true);
                animate("Idle", false);
            }
            //if neither "S" or "W" are hit then idle
            if (Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.W) == false)
            {
                animate("Forword", false);
                animate("Backword", false);
                animate("Idle", true);
            }
        }

    }
    //Rotate the player based on mouse position
    public void rotate()
    {
        if (rot)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity, floorMask))
            {
                //calculate direction between player and mouse


                lookDir = hit.point - transform.position;

                // Ensure the vector is entirely along the floor plane.
                lookDir.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                if (Vector3.Distance(transform.position, hit.point) > 1f)
                {
                    //Quaternion newRotation = new Quaternion();
                    // newRotation.SetLookRotation(lookDir);
                    Quaternion newRotation = Quaternion.LookRotation(lookDir);

                    // Set the player's rotation to this new rotation.
                    //rigidB.MoveRotation(newRotation);
                    this.transform.rotation = newRotation;

                }

            }
        }
    }
    //set if player can move
    public void setMove(bool m_move)
    {
        move = m_move;
    }
    //set if the player can rotate
    public void setRotate(bool m_rot)
    {
        rot = m_rot;
    }

    //get where the player is looking
    public Vector3 getLookDirection()
    {
        return lookDir;
    }
    //set player animation
    void animate(string animationName, bool status)
    {
        if (shouldAnimate)
        {
            animator.SetBool(animationName, status);
        }

    }
    //should the player animate 
    public void setAnimate(bool m_shouldAnimate)
    {
        shouldAnimate = m_shouldAnimate;
        if (shouldAnimate == false)
        {
            animate("Forword", false);
            animate("Backword", false);
            animate("Idle", false);
        }
    }

}
