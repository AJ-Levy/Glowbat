using UnityEngine;
using System.Collections;

/// <summary>
/// A class that defines the shield powerup.
/// </summary>
public class SpeedBoostPowerUp : PowerUp
{
    /// <summary>
    /// Increases the player's movement speed for a period.
    /// </summary>
    /// <param name="player">A reference to the script that manages the player.</param>
    public override void ApplyPowerUp(PlayerManager player)
    {
        player.StartCoroutine(SpeedBoostCoroutine(player));
    }

    /// <summary>
    /// Increases the player's speed and then decreases it after a set time.
    /// </summary>
    /// <param name="player">A reference to the script that manages the player.</param>
    /// <returns>An IEnumerator that can be used to yield execution in a coroutine.</returns>
    private IEnumerator SpeedBoostCoroutine(PlayerManager player)
    {
        player.SpeedBoost();
        yield return new WaitForSeconds(4); // Duration of the power-up
        player.ReduceSpeed();

    }
}
