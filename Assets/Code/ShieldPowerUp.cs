using UnityEngine;

public class ShieldPowerUp : PowerUp
{
    public override void ApplyPowerUp(PlayerManager player)
    {
        player.ActivateShield();
    }

}
