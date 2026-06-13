using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class SkinPickerUI : MonoBehaviour
{
    [Header("Preview")]
    public Renderer previewRenderer;      // SkinnedMeshRenderer del preview (p.ej. Alpha_Surface)

    [Header("Skins")]
    public Material[] skins;              // Tus 6 materiales
    public string[] skinNames;            // (Opcional) nombres para mostrar
    public TMP_Text nameText;             // (Opcional) label

    [Header("Scenes")]
    public string menuScene = "MainMenu"; // nombre exacto de la escena menú

    int index;

    void Start()
    {
        index = (GameManager.Instance != null)
            ? GameManager.Instance.GetSkinForPlayer(0)
            : 0;

        Apply(false);
        RefreshLabel();
    }

    void Update()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        if (kb != null)
        {
            if (kb.leftArrowKey.wasPressedThisFrame) Prev();
            if (kb.rightArrowKey.wasPressedThisFrame) Next();
            if (kb.enterKey.wasPressedThisFrame) Apply(true);
            if (kb.rKey.wasPressedThisFrame) RandomizeSkin();
            if (kb.escapeKey.wasPressedThisFrame) BackToMenu();
        }

        if (gp != null)
        {
            if (gp.dpad.left.wasPressedThisFrame) Prev();
            if (gp.dpad.right.wasPressedThisFrame) Next();
            if (gp.buttonSouth.wasPressedThisFrame) Apply(true); // A
            if (gp.startButton.wasPressedThisFrame) RandomizeSkin();
            if (gp.buttonEast.wasPressedThisFrame) BackToMenu(); // B
        }
    }

    // ---- Lógica de selección ----
    public void Prev()
    {
        index = (index - 1 + skins.Length) % skins.Length;
        Apply(false);
        RefreshLabel();
    }

    public void Next()
    {
        index = (index + 1) % skins.Length;
        Apply(false);
        RefreshLabel();
    }

    public void RandomizeSkin()
    {
        index = Random.Range(0, skins.Length);
        Apply(false);
        RefreshLabel();
    }

    public void Apply(bool save)
    {
        if (previewRenderer != null && skins != null && skins.Length > 0)
            previewRenderer.material = skins[index];

        if (save && GameManager.Instance != null)
            GameManager.Instance.SetSkinForPlayer(0, index);
    }

    void RefreshLabel()
    {
        if (nameText != null && skinNames != null && index < skinNames.Length)
            nameText.text = skinNames[index];
    }

    public void BackToMenu()
    {
        // (Opcional) guarda siempre al salir
        if (GameManager.Instance != null) GameManager.Instance.SetSkinForPlayer(0, index);
        SceneManager.LoadScene(menuScene);
    }

    // ---- WRAPPERS para los botones (sin parámetros) ----
    public void UI_Next() => Next();
    public void UI_Prev() => Prev();
    public void UI_Randomize() => RandomizeSkin();
    public void UI_Apply() => Apply(true);
    public void UI_BackToMenu() => BackToMenu();
}
