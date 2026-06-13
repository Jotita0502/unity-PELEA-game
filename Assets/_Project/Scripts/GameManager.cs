using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // (Opcional) biblioteca central de materiales, si quieres usarla.
    [Header("Skins (opcional, para quien quiera pedir el material al GM)")]
    public Material[] allSkins;

    // Jugadores unidos (si usas PlayerInputManager en el menú)
    public readonly List<PlayerInput> JoinedPlayers = new();

    // Índice de skin elegido por jugador (0..3)
    private readonly int[] chosenSkin = new int[4];

    // ------------------------------------------------------------
    // Bootstrap automático: crea el GM si entras directo a una escena
    // ------------------------------------------------------------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoCreate()
    {
        if (Instance == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cargar elección persistida (opcional)
        for (int i = 0; i < chosenSkin.Length; i++)
            chosenSkin[i] = PlayerPrefs.GetInt($"skin_{i}", 0);
    }

    // ------------------------------------------------------------
    // Registro de jugadores (opcional, si quieres mantenerlos vivos)
    // ------------------------------------------------------------
    public void Register(PlayerInput pi)
    {
        if (!JoinedPlayers.Contains(pi))
        {
            JoinedPlayers.Add(pi);
            DontDestroyOnLoad(pi.gameObject);
        }
    }

    public void Unregister(PlayerInput pi)
    {
        if (pi != null) JoinedPlayers.Remove(pi);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ------------------------------------------------------------
    // Skins: guardar / leer índice SIN depender de 'allSkins'
    // ------------------------------------------------------------
    public void SetSkinForPlayer(int p, int idx)
    {
        p = Mathf.Clamp(p, 0, chosenSkin.Length - 1);
        // si tienes biblioteca central, clamp por su tamaño; si no, deja el índice tal cual
        if (allSkins != null && allSkins.Length > 0)
            idx = Mathf.Clamp(idx, 0, allSkins.Length - 1);

        chosenSkin[p] = idx;
        PlayerPrefs.SetInt($"skin_{p}", idx);  // persistimos para próximas ejecuciones
        PlayerPrefs.Save();

        Debug.Log($"[GM] Save skin player {p} -> {chosenSkin[p]}");
    }

    public int GetSkinForPlayer(int p)
    {
        p = Mathf.Clamp(p, 0, chosenSkin.Length - 1);
        return chosenSkin[p];
    }

    // (Opcional) Si un script quiere pedir el material al GameManager:
    public Material GetSkinMaterial(int idx)
    {
        if (allSkins != null && idx >= 0 && idx < allSkins.Length)
            return allSkins[idx];
        return null;
    }
}
