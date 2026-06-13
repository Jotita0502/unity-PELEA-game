using FishNet.Object;
using UnityEngine;

public class PlayerUIBinder : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner) return;

        var ui = FindObjectOfType<HealthBarUI>(true);
        var hp = GetComponent<Health>();

        if (ui && hp) ui.Bind(hp);
        else Debug.LogWarning("[PlayerUIBinder] Falta HealthBarUI o Health.");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (!IsOwner) return;

        var ui = FindObjectOfType<HealthBarUI>(true);
        if (ui) ui.Bind(null);
    }
}
