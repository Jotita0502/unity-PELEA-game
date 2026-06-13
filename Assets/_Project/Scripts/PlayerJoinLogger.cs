using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinLogger : MonoBehaviour
{
    // PlayerInputManager con "Send Messages" llamará esto
    public void OnPlayerJoined(PlayerInput pi)
    {
        Debug.Log($"[JOIN] Player #{pi.playerIndex} - Device: {(pi.devices.Count > 0 ? pi.devices[0].displayName : "Unknown")}");
    }
    public void OnPlayerLeft(PlayerInput pi)
    {
        Debug.Log($"[LEAVE] Player #{pi.playerIndex}");
    }
}
