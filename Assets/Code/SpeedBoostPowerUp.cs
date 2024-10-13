using UnityEngine;
using System.Collections;

public class SpeedBoostPowerUp : PowerUp
{
    public override void ApplyPowerUp(PlayerManager player)
    {
        player.StartCoroutine(SpeedBoostCoroutine(player));
    }

    private IEnumerator SpeedBoostCoroutine(PlayerManager player)
    {
        player.SpeedBoost();
        yield return new WaitForSeconds(4); // Duration of the power-up
        player.ReduceSpeed();

    }
}
