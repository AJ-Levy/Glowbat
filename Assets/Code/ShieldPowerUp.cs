using UnityEngine;

/// <summary>
/// A class that defines the shield powerup.
/// </summary>
public class ShieldPowerUp : PowerUp
{
    /// <summary>
    /// Activates a shield that protects the player from one collision.
    /// </summary>
    /// <param name="player">A reference to the script that manages the player.</param>
    public override void ApplyPowerUp(PlayerManager player)
    {
        player.ActivateShield();
    }

}
