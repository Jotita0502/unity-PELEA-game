using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuFlow : MonoBehaviour
{
    [Header("UI")]
    public Button firstButton; // Btn_Jugar u otro

    void OnEnable()
    {
        if (firstButton)
        {
            EventSystem.current?.SetSelectedGameObject(firstButton.gameObject);
            firstButton.Select();
        }
    }

    // --- ya existente ---
    public void OnPlayerJoined(PlayerInput pi) { /* tu código tal cual */ }
    public void OnPlayerPressedStart(PlayerInput pi) => PlayLocal();

    // --- botones ---
    public void PlayLocal() { SceneManager.LoadScene("MapSelect"); } // si luego quieres que vaya a otra, cámbialo
    public void OpenOptions() { Debug.Log("Opciones (WIP)"); }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // **NUEVO**: Botón Skins ? CharacterSelect
    public void OpenSkins()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
}
