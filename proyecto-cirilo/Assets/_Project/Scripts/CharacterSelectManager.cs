using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Skins disponibles (6)")]
    public Material[] skins; // Asigna Skin_0 ... Skin_5 en el inspector

    [Header("UI")]
    public Transform slotsParent;   // asigna el objeto "Slots" del Canvas
    public GameObject playerSlotPrefab;
    public bool autoContinue = false;    // ?? por ahora apagado
    public int minPlayersToContinue = 2; // ?? si luego lo prendes, pide 2+

    // Estado local por jugador que se unió
    class SelState
    {
        public PlayerInput pi;
        public PlayerState ps;
        public PlayerSlotUI slot;
    }

    private readonly List<SelState> _states = new();

    void Start()
    {
        // Crea una tarjeta por jugador unido en MainMenu
        foreach (var pi in GameManager.Instance.JoinedPlayers)
        {
            var ps = pi.GetComponent<PlayerState>();
            if (ps == null) ps = pi.gameObject.AddComponent<PlayerState>();

            // Skin inicial (por comodidad, distinta para cada uno)
            ps.skinIndex = _states.Count % skins.Length;
            ps.ready = false;
            ps.ApplyMaterial(skins[ps.skinIndex]);

            var go = Instantiate(playerSlotPrefab, slotsParent);
            var slot = go.GetComponent<PlayerSlotUI>();
            slot.SetTitle($"Jugador #{pi.playerIndex}");
            slot.SetSkin(ps.skinIndex);
            slot.SetReady(ps.ready);

            _states.Add(new SelState { pi = pi, ps = ps, slot = slot });
        }
    }

    public void ChangeSkin(PlayerInput pi, int dir)
    {
        var st = _states.Find(s => s.pi == pi);
        if (st == null) return;
        if (st.ps.ready) return; // si está listo, no cambia

        st.ps.skinIndex = (st.ps.skinIndex + dir + skins.Length) % skins.Length;
        var prefabMgr = st.pi.GetComponent<CharacterPrefabManager>();
        if (prefabMgr != null)
            prefabMgr.ApplyCharacterIndex(st.ps.skinIndex);

        st.slot.SetSkin(st.ps.skinIndex);
    }

    public void ToggleReady(PlayerInput pi)
    {
        var st = _states.Find(s => s.pi == pi);
        if (st == null) return;

        st.ps.ready = !st.ps.ready;
        st.slot.SetReady(st.ps.ready);

        if (!autoContinue) return; // ?? por ahora NO cambiamos de escena

        if (_states.Count >= minPlayersToContinue)
        {
            bool allReady = true;
            foreach (var s in _states) if (!s.ps.ready) { allReady = false; break; }
            if (allReady)
                UnityEngine.SceneManagement.SceneManager.LoadScene("MapSelect");
        }
    }

}
