using UnityEngine;

public enum PickupType { InstantKill, Heal2, SpeedBoost }

public class Pickup : MonoBehaviour
{
    public PickupType type = PickupType.Heal2;

    [Header("Speed Boost (solo si aplica)")]
    public float speedMultiplier = 1.5f;
    public float boostDuration = 5f;

    [Header("Vida útil (desaparece si nadie lo toma)")]
    public Vector2 lifetimeRange = new Vector2(8f, 16f);

    [HideInInspector] public PickupSpawnManager owner;
    [HideInInspector] public int slotIndex = -1;

    float despawnAt;

    void OnEnable()
    {
        despawnAt = Time.time + Random.Range(lifetimeRange.x, lifetimeRange.y);
    }

    void Update()
    {
        if (Time.time >= despawnAt)
        {
            if (owner) owner.OnPickupConsumed(slotIndex);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Buscamos Health en el player (para daño/curación)
        var health = other.GetComponentInParent<Health>();

        // Para speed, buscamos el receptor en el player
        var speedRx = other.GetComponentInParent<SpeedBoostReceiver>();

        switch (type)
        {
            case PickupType.InstantKill:
                if (health) health.ApplyDamage(int.MaxValue); // mata de una
                break;

            case PickupType.Heal2:
                if (health) health.Heal(2);
                break;

            case PickupType.SpeedBoost:
                if (speedRx) speedRx.ApplySpeedBoost(speedMultiplier, boostDuration);
                break;
        }

        if (owner) owner.OnPickupConsumed(slotIndex);
        Destroy(gameObject);
    }
}
