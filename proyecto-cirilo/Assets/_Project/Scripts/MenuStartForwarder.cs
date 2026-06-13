using UnityEngine;
using UnityEngine.InputSystem;

public class MenuStartForwarder : MonoBehaviour
{
    // Reenvía "Iniciar" al GameObject que tiene el PlayerInputManager,
    // llamando por nombre al método OnPlayerPressedStart (sin importar la clase).
    public void OnIniciar()
    {
        var pi = GetComponent<PlayerInput>();

        // Busca el PlayerInputManager en la escena MainMenu
        var pim = Object.FindFirstObjectByType<PlayerInputManager>();
        if (pim == null)
        {
            // Si no hay PlayerInputManager (p.ej., ya no estamos en MainMenu), no hacemos nada.
            Debug.LogWarning("MenuStartForwarder: PlayerInputManager no encontrado (probablemente no estamos en MainMenu).");
            return;
        }

        // Llama al método "OnPlayerPressedStart(PlayerInput)" si existe en ese GO
        pim.gameObject.SendMessage("OnPlayerPressedStart", pi, SendMessageOptions.DontRequireReceiver);
    }
}
