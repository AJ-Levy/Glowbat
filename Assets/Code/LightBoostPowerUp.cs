using UnityEngine;

public class LightBoostPowerUp : PowerUp
{
    private float glowIncrease = 0.5f;
    public override void ApplyPowerUp(PlayerManager player)
    {   
        player.IncreaseGlow(glowIncrease);
    }
}
