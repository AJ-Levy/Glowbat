using UnityEngine;
using System.Collections;

/// <summary>
/// A class that defines the shrink powerup.
/// </summary>
public class ShrinkPowerUp : PowerUp
{
    /// <summary>
    /// Shrinks the player for a period.
    /// </summary>
    /// <param name="player">A reference to the script that manages the player.</param>
    public override void ApplyPowerUp(PlayerManager player)
    {
        player.StartCoroutine(ShrinkCoroutine(player));
    }

    /// <summary>
    /// Shrinks the player then returns them to normal size after a set time.
    /// </summary>
    /// <param name="player">A reference to the script that manages the player.</param>
    /// <returns>An IEnumerator that can be used to yield execution in a coroutine.</returns>
    private IEnumerator ShrinkCoroutine(PlayerManager player)
    {
        player.Shrink();
        yield return new WaitForSeconds(4); // Duration of the power-up
        player.UnShrink();

    }
}
