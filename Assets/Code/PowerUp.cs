using UnityEngine;

/// <summary>
/// An abstract class that all powerups are based on.
/// </summary>
public abstract class PowerUp : MonoBehaviour
{
    AudioManager audioManager;

    // <summary>
    /// Keeps the AudioManger accessible.
    /// </summary>
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    // <summary>
    /// Abstract method that needs to be implemented in child classes
    /// that specifies the action to be carried out when a powerup is consumed.
    /// </summary>
    /// <param name="player">The script that manages the player.</param>
    public abstract void ApplyPowerUp(PlayerManager player);
  
    // <summary>
    /// Manages consumption when the player collides with the powerup.
    /// </summary>
    /// <param name="other">The player's collider object.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            ApplyPowerUp(player);
            Destroy(gameObject);
            audioManager.PlaySFX(audioManager.powerUp);
        }
    }
}
