using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEditor.Animations;

/// WORK IN PROGRESS
/// <summary>
/// A light pulse powerup.
/// </summary>
public class PulsePowerUp : PowerUp
{
    private Light2D globalLight; 
    private float pulseIntensity = 0.2f; 
    private float pulseDuration = 3f; 
    private float transitionDuration = 1f;
    private float transitionMultiplier = 0.01f;

    /// <summary>
    /// Getting reference to and intialising global light.
    /// </summary>
    private void Start()
    {
        globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light2D>();
        globalLight.intensity = 0f;
        globalLight.gameObject.SetActive(false);
    }

    /// <summary>
    /// Starting the light pulse.
    /// </summary>
    /// <param name="player">A reference to the script that manages the player.</param>
    public override void ApplyPowerUp(PlayerManager player)
    {   
        
        if (globalLight == null) 
        {
            Debug.Log("No Light found");
            return;
        }
        StartCoroutine(PulseEffect());
    }

    /// <summary>
    /// The global light increases in intensity then decreases before being disabled.
    /// </summary>
    /// <returns>An IEnumerator that can be used to yield execution in a coroutine.</returns>
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
            if (globalLight.intensity > 0f){
                globalLight.intensity -= transitionMultiplier * Time.deltaTime;
            } 
        }else{
            duration -= Time.deltaTime;
        }

        globalLight.gameObject.SetActive(false);
    }

}
