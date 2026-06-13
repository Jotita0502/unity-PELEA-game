using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MapSelectManager : MonoBehaviour
{
    [Header("Maps")]
    public string[] sceneNames = { "Map_Arena", "Map_Ice" };  // nombres EXACTOS en Build Settings
    public string[] titles = { "Arena", "Ice" };           // mismo largo que sceneNames
    public Sprite[] thumbnails;                               // opcional, mismo orden

    [Header("UI")]
    public Transform slotsParent;      // Canvas/MapSlots
    public GameObject mapCardPrefab;   // prefab con MapCardUI

    int current = 0;
    readonly List<MapCardUI> cards = new();

    void Start()
    {
        // construir tarjetas
        for (int i = 0; i < sceneNames.Length; i++)
        {
            var go = Instantiate(mapCardPrefab, slotsParent);
            var ui = go.GetComponent<MapCardUI>();

            var spr = (thumbnails != null && i < thumbnails.Length) ? thumbnails[i] : null;
            var ttl = (titles != null && i < titles.Length) ? titles[i] : sceneNames[i];

            ui.Set(spr, ttl);
            int idx = i; // captura para el botón
            if (ui.button)
            {
                ui.button.onClick.AddListener(() => SetIndex(idx));
                // si quieres que al click cargar de una vez:
                // ui.button.onClick.AddListener(Play);
            }
            cards.Add(ui);
        }
        Refresh();
    }

    void Update()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        if (kb != null)
        {
            if (kb.leftArrowKey.wasPressedThisFrame) Prev();
            if (kb.rightArrowKey.wasPressedThisFrame) Next();
            if (kb.enterKey.wasPressedThisFrame) Play();
            if (kb.escapeKey.wasPressedThisFrame) Back();
        }

        if (gp != null)
        {
            if (gp.dpad.left.wasPressedThisFrame) Prev();
            if (gp.dpad.right.wasPressedThisFrame) Next();
            if (gp.buttonSouth.wasPressedThisFrame) Play(); // A
            if (gp.buttonEast.wasPressedThisFrame) Back(); // B
        }
    }

    // --- Métodos para asignar a Botones UI ---
    public void Prev() { Move(-1); }
    public void Next() { Move(+1); }
    public void Play() { LoadCurrent(); }
    public void Back() { SceneManager.LoadScene("MainMenu"); }

    public void SetIndex(int i)
    {
        if (cards.Count == 0) return;
        current = Mathf.Clamp(i, 0, cards.Count - 1);
        Refresh();
    }

    public void Move(int dir)
    {
        if (cards.Count == 0) return;
        current = (current + dir + cards.Count) % cards.Count;
        Refresh();
    }

    void Refresh()
    {
        for (int i = 0; i < cards.Count; i++)
            cards[i].SetSelected(i == current);
    }

    void LoadCurrent()
    {
        if (sceneNames == null || current >= sceneNames.Length) return;
        var scene = sceneNames[current];
        if (Application.CanStreamedLevelBeLoaded(scene))
            SceneManager.LoadScene(scene);
        else
            Debug.LogError($"La escena '{scene}' no está en Build Settings.");
    }

    // Si quieres seguir soportando confirmación por PlayerInput:
    public void Confirm(PlayerInput pi)
    {
        if (pi != null && pi.playerIndex != 0) { Debug.Log("Solo Player #0 confirma."); return; }
        LoadCurrent();
    }
}
