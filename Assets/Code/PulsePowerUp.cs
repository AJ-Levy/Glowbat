using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEditor.Animations;

public class PulsePowerUp : PowerUp
{
    private Light2D globalLight; 
    private float pulseIntensity = 0.2f; 
    private float pulseDuration = 3f; 
    private float transitionDuration = 1f;
    private float transitionMultiplier = 0.01f;

    private void Start()
    {
        globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light2D>();
        globalLight.intensity = 0f;
        globalLight.gameObject.SetActive(false);
    }

    public override void ApplyPowerUp(PlayerManager player)
    {   
        
        if (globalLight == null) 
        {
            Debug.Log("No Light found");
            return;
        }
        StartCoroutine(PulseEffect());
    }

    private IEnumerator PulseEffect()
    {
        globalLight.gameObject.SetActive(true);

        float duration = transitionDuration;
        if (duration <= 0){
            if (globalLight.intensity < pulseIntensity){
                globalLight.intensity += transitionMultiplier * Time.deltaTime;
            } 
        }else{
            duration -= Time.deltaTime;
        }

        globalLight.intensity = pulseIntensity;

        // Wait for the remaining pulse duration (after the intensity increase)
        yield return new WaitForSeconds(pulseDuration);

        duration = transitionDuration;
        if (duration <= 0){
            if (globalLight.intensity > pulseIntensity){
                globalLight.intensity -= transitionMultiplier * Time.deltaTime;
            } 
        }else{
            duration -= Time.deltaTime;
        }

        globalLight.gameObject.SetActive(false);
    }

}
