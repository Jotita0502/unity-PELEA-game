using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("Asignar en el prefab: los renderers del mesh")]
    public Renderer[] renderers;

    [HideInInspector] public int skinIndex = 0;
    [HideInInspector] public bool ready = false;

    // Aplica el material a todos los renderers configurados
    public void ApplyMaterial(Material m)
    {
        if (renderers == null) return;
        foreach (var r in renderers)
        {
            if (!r) continue;
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++) mats[i] = m;
            r.sharedMaterials = mats;
        }
    }
}
