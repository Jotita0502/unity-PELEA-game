using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectUI : MonoBehaviour
{
    public CharacterPrefabManager preview;   // Jugador_Preview
    public string menuSceneName = "MainMenu";

    int index;

    void Start()
    {
        index = PlayerPrefs.GetInt("SkinIndex", 0);
        if (preview && preview.Count > 0)
            preview.ApplyCharacterIndex(Mathf.Clamp(index, 0, preview.Count - 1));
    }

    public void Next()
    {
        if (!preview || preview.Count == 0) return;
        index = (index + 1) % preview.Count;
        preview.ApplyCharacterIndex(index);
    }

    public void Prev()
    {
        if (!preview || preview.Count == 0) return;
        index = (index - 1 + preview.Count) % preview.Count;
        preview.ApplyCharacterIndex(index);
    }

    // Guardar skin y regresar al menú
    public void Confirm()
    {
        if (preview) preview.ApplyAndSave(index);       // guarda SkinIndex
        SceneManager.LoadScene(menuSceneName);          // ← vuelve a MainMenu
    }

    // Botón "Volver" sin guardar (opcional)
    public void BackToMenu() => SceneManager.LoadScene(menuSceneName);
}
