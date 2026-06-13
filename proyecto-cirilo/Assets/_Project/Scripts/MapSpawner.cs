using UnityEngine;
using UnityEngine.InputSystem;

public class MapSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // arrastra Spawn_0..Spawn_3

    void Start()
    {
        var gm = GameManager.Instance ?? FindFirstObjectByType<GameManager>();
        if (gm == null) { Debug.LogError("No GameManager en escena"); return; }
        var players = gm.JoinedPlayers;
        if (players.Count == 0) return;

        for (int i = 0; i < players.Count; i++)
        {
            var p = players[i];
            var sp = (spawnPoints != null && spawnPoints.Length > 0)
                        ? spawnPoints[i % spawnPoints.Length] : null;

            if (sp != null)
            {
                p.transform.position = sp.position;
                p.transform.rotation = sp.rotation;
            }

            var rb = p.GetComponent<Rigidbody>();
            if (rb) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

            // Asegura el mapa de acciones de juego
            if (p.currentActionMap == null || p.currentActionMap.name != "Jugador")
                p.SwitchCurrentActionMap("Jugador");
        }
    }
}
