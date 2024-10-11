using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public abstract void ApplyPowerUp(PlayerManager player);
  
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            ApplyPowerUp(player);
            Destroy(gameObject); 
        }
    }
}
