using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class PickupSpawnManager : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnSlot
    {
        public Transform point;
        [HideInInspector] public GameObject current;
    }

    [Header("Puntos de spawn (hijos en el mapa)")]
    public SpawnSlot[] slots;

    [Header("Prefabs")]
    public GameObject prefabKill;
    public GameObject prefabHeal2;
    public GameObject prefabSpeed;

    [Header("Pesos (probabilidades relativas)")]
    [Range(0, 1)] public float weightKill = 0.2f;
    [Range(0, 1)] public float weightHeal2 = 0.5f;
    [Range(0, 1)] public float weightSpeed = 0.3f;

    [Header("Respawn por slot")]
    public Vector2 respawnDelay = new Vector2(6f, 12f);

    [Header("Control global")]
    [Tooltip("Máximo de pickups activos simultáneamente (≤0 = sin límite)")]
    public int maxActive = 0;
    [Tooltip("Tiempo mínimo entre spawns (en cualquier slot)")]
    public float globalCooldownBetweenSpawns = 1.5f;

    float nextGlobalAllowed = 0f;
    int activeCount = 0;

    // trackeo de respawns en curso por slot
    readonly Dictionary<int, Coroutine> pending = new();

    void Start()
    {
        StartCoroutine(InitialFill());
    }

    IEnumerator InitialFill()
    {
        // Llenado escalonado para evitar picos
        for (int i = 0; i < slots.Length; i++)
        {
            TrySpawn(i);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void OnPickupConsumed(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        if (slots[slotIndex].current != null)
        {
            slots[slotIndex].current = null;
            activeCount = Mathf.Max(0, activeCount - 1);
        }

        if (pending.ContainsKey(slotIndex)) return;
        pending[slotIndex] = StartCoroutine(RespawnRoutine(slotIndex));
    }

    IEnumerator RespawnRoutine(int index)
    {
        // Espera aleatoria por slot
        float delay = Random.Range(respawnDelay.x, respawnDelay.y);
        yield return new WaitForSeconds(delay);

        // Respeta cooldown global
        float wait = Mathf.Max(0f, nextGlobalAllowed - Time.time);
        if (wait > 0f) yield return new WaitForSeconds(wait);

        TrySpawn(index);
        pending.Remove(index);
    }

    void TrySpawn(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        if (!slots[index].point) return;
        if (slots[index].current) return;

        if (maxActive > 0 && activeCount >= maxActive) return;

        GameObject prefab = ChoosePrefab();
        if (!prefab) return;

        Vector3 pos = slots[index].point.position;
        if (NavMesh.SamplePosition(pos, out var hit, 2f, NavMesh.AllAreas))
            pos = hit.position;

        var go = Instantiate(prefab, pos, slots[index].point.rotation);
        slots[index].current = go;
        activeCount++;
        nextGlobalAllowed = Time.time + Mathf.Max(0f, globalCooldownBetweenSpawns);

        // Conectar el pickup con el spawner/slot
        var p = go.GetComponent<Pickup>();
        if (p)
        {
            p.owner = this;
            p.slotIndex = index;
        }
    }

    GameObject ChoosePrefab()
    {
        float k = Mathf.Max(0.0001f, weightKill);
        float h = Mathf.Max(0.0001f, weightHeal2);
        float s = Mathf.Max(0.0001f, weightSpeed);
        float sum = k + h + s;
        float r = Random.value * sum;

        if (r < k) return prefabKill; r -= k;
        if (r < h) return prefabHeal2; r -= h;
        return prefabSpeed;
    }
}
