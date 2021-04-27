using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

//script to control day/night cycle - Dvir
public class LightingControl : MonoBehaviour
{
    //post processing
    private PostProcessVolume postProscessVolume;
    private Vignette vignette;
    private ColorGrading colorGrading;

    //post processing values
    private float vignetteBaseValue;
    private float postExposureBaseValue;
    [SerializeField] private float greenColor = 100;

    //post processing control values
    private float postProcessingOnTimer = 1;
    private float postProcessingOffTimer = 1;
    private float postProcessingTimerAmount = 1;
    private bool isPostProcessing = false;
    
    //light components
    [SerializeField] private Light dirLight;
    [SerializeField] private Lighting lightProfile;
    [SerializeField] private float dayStartTime = 100;

    //time controls
    private float time;
    private float timeInDay;
    // Start is called before the first frame update
    void Start()
    {
        postProscessVolume = GetComponent<PostProcessVolume>();
        postProscessVolume.profile.TryGetSettings(out vignette);
        postProscessVolume.profile.TryGetSettings(out colorGrading);

        vignetteBaseValue = vignette.intensity.value;
        postExposureBaseValue = colorGrading.postExposure.value;

        //set the time to start, set the amount of time in a day to double the "start" time, so that we start in the morning
        time = dayStartTime;
        timeInDay = time * 2;
        
       
    }

    // Update is called once per frame
    void Update()
    {
        //update time
        time += Time.deltaTime;
        time %= timeInDay;
        updateLight(time / timeInDay);
        updatePostProcessing(time / timeInDay);
    }
    /*
     * update the time based on the precentage of how much time has passed in the day
     */
    private void updateLight(float timePercent)
    {
        RenderSettings.ambientLight = lightProfile.ambiant.Evaluate(timePercent);
        RenderSettings.fogColor = lightProfile.fog.Evaluate(timePercent);

        dirLight.color = lightProfile.directional.Evaluate(timePercent);
        dirLight.transform.localRotation = Quaternion.Euler(new Vector3(timePercent * 360f - 90f, 170, 0));
    }
    /*
     * control the post processing effects based on the precentage of how much time has passed in the day
     */
    private void updatePostProcessing(float timePercent)
    {
        //post processing in night time
        if ((timePercent < 0.3f || timePercent > 0.75f) && !isPostProcessing)
        {
            postProcessingOffTimer = postProcessingTimerAmount;
            isPostProcessing = true;
        }
        //disable night time post processing during day time
        else if((timePercent > 0.3f && timePercent < 0.75f) && isPostProcessing)
        {
            postProcessingOnTimer = postProcessingTimerAmount;
            isPostProcessing = false;
        }
        if (isPostProcessing)
        {
            //set the vignette and the post exposure in night time
            if (postProcessingOnTimer > 0)
            {
                postProcessingOnTimer -= Time.deltaTime;
                vignette.intensity.value = vignetteBaseValue * (1 - postProcessingOnTimer / postProcessingTimerAmount);
                colorGrading.postExposure.value = postExposureBaseValue * (1 - postProcessingOnTimer / postProcessingTimerAmount);
            }
           

        }
        else
        {
            if (postProcessingOffTimer > 0)
            {
                //disable the vignette and the post exposure in day time
                postProcessingOffTimer -= Time.deltaTime;
                vignette.intensity.value = vignetteBaseValue * postProcessingOffTimer / postProcessingTimerAmount;
                colorGrading.postExposure.value = postExposureBaseValue * (postProcessingOffTimer / postProcessingTimerAmount);
            }
        }
        //post processing day time
        if (timePercent > 0.3f && timePercent < 0.75f)
        {
            //decrease the amount of green color throughout the day, for more orange color overall
            colorGrading.mixerRedOutGreenIn.value = greenColor * (timePercent - 0.3f);
        }
        else
        {
            //reset when night time
            colorGrading.mixerRedOutGreenIn.value = 0;
        }
       

    }
    /*
     * reset the day to morning time
     */
    public void resetDay()
    {
        time = dayStartTime;
    }
   
}
