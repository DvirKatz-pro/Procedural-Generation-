using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Camera controls - Dvir/Cal
public class CameraFollow : MonoBehaviour
{
    //Variables for following a target
    [SerializeField] private Transform target;
    public float smoothing = 5f;
    Vector3 offset;

    //variables for panning the camera
    [SerializeField] private float panDistance;
    bool pan = false;

    //keep track of object obstructing the camera
    GameObject obstructedObject;

    // Use this for initialization
    void Start()
    {
        //set the initial camera position
        offset = new Vector3(-28, 25, -28);
        this.transform.position = target.position + offset;
        //this.enabled = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
        {
            //follow target
            Vector3 targetCamPos = target.position + offset;
            targetCamPos.z -= 0.5f;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
            //if we need the camera to pan out
            if (pan)
            {
                pan = false;
                StartCoroutine(PanOut());
            }
        }
        //check for obstruction
        seeThrough();
    }
    /* should camera pan out
     * isPan - should camera pan
     */
    public void setPan(bool isPan)
    {
        pan = isPan;
    }
    //move the camera back smoothly 
    IEnumerator PanOut()
    {
        float currentSize = this.GetComponent<Camera>().orthographicSize;
        while (currentSize < panDistance)
        {
            currentSize += 1.5f * Time.deltaTime;
            this.GetComponent<Camera>().orthographicSize = currentSize;
            yield return new WaitForEndOfFrame();
        }
    }
    /*
     * set target for camera
     * t - target transform
     */
    public void SetTarget(Transform t)
    {
        target = t;
        offset = new Vector3(-28, 25, -28);
        this.transform.position = target.position + offset;
        this.enabled = true;
    }
    /*
     * check if building is obstructing the camera
     */
    private void seeThrough()
    {

        Vector3 cameraTarget = target.transform.position - transform.position;
        cameraTarget.Normalize();
        //create a ray between the camera and target
        RaycastHit hit;
        Ray ray = new Ray(transform.position, cameraTarget);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider != null && hit.transform.gameObject.tag == "Building")
            {
               //if the ray hit a building change the bullding render mode to fade and make it transparent
                MaterialExtensions.ToFadeMode(hit.transform.gameObject.GetComponent<Renderer>().material);
                Color fadeColor = hit.transform.gameObject.GetComponent<Renderer>().material.color;
                fadeColor.a = 0.5f;
                hit.transform.gameObject.GetComponent<Renderer>().material.color = fadeColor;
                obstructedObject = hit.transform.gameObject;
            }
            else
            {
                if (obstructedObject != null)
                {
                    //if ray is no longer hitting a building, change the render mode to opaque
                    Color fadeColor = obstructedObject.transform.gameObject.GetComponent<Renderer>().material.color;
                    fadeColor.a = 1;
                    obstructedObject.transform.gameObject.GetComponent<Renderer>().material.color = fadeColor;
                    MaterialExtensions.ToOpaqueMode(obstructedObject.transform.gameObject.GetComponent<Renderer>().material);
                    obstructedObject = null;
                
                }
            }

        }
        
        

    }
}
