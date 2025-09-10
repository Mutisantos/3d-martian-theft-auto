using UnityEngine;

public enum ConsumableType
{
    HEALTH, AMMO, SHIELD, KEY
}

/** Define todos los objetos que pueden ser consumidos por el jugador, clasificados a través del enum ConsumableType
 * Esteban.Hernandez
 */
public class Consumable : MonoBehaviour
{
    public int amount = 10;
    public ConsumableType type = ConsumableType.HEALTH;
    public AudioClip ConsumptionClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool consumed = false;
            PlayerShooter playerRef = other.gameObject.GetComponentInParent<PlayerShooter>();
            switch (type)
            {
                case ConsumableType.HEALTH:
                    if (playerRef.CurrentHealth < playerRef.MaxHealth)
                    {
                        playerRef.LastItem = $"Salud ({amount})";
                        consumed = true;
                    }
                    break;
                case ConsumableType.AMMO:
                    if (playerRef.CurrentAmmo < playerRef.GetCurrentMaxAmmo())
                    {
                        playerRef.LastItem = $"Munición ({amount})";
                        consumed = true;
                    }
                    break;
                case ConsumableType.SHIELD:
                    if (playerRef.CurrentShield < playerRef.MaxShield)
                    {
                        playerRef.LastItem = $"Chaleco Antibalas ({amount})";
                        consumed = true;
                    }
                    break;
                case ConsumableType.KEY:
                    GameManager.instance.AddItemToInventory(gameObject.name, amount);
                    playerRef.LastItem = $"({gameObject.name})";
                    consumed = true;
                    break;
            }
            if (consumed)
            {
                playerRef.RecoverFromConsumable(type, amount);
                SoundSingleton.instance.PlayWithoutWait(ConsumptionClip);
                Destroy(gameObject);
            }
        }
    }
}
