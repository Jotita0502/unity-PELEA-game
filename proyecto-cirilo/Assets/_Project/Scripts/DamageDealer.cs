using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 2;
    public LayerMask targetLayers;
    public bool useTrigger = true;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col && useTrigger) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        TryDamage(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (useTrigger) return;
        TryDamage(collision.gameObject);
    }

    void TryDamage(GameObject other)
    {
        if (((1 << other.layer) & targetLayers) == 0) return;

        var h = other.GetComponentInParent<Health>();
        if (h != null) h.ApplyDamage(damage);
    }
}
