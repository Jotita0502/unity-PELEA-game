using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MapSelectInput : MonoBehaviour
{
    private MapSelectManager _mgr;
    private PlayerInput _pi;

    [Tooltip("Tiempo mínimo entre pasos left/right")]
    public float repeatDelay = 0.22f;
    private float _nextTime;

    void Awake()
    {
        _pi = GetComponent<PlayerInput>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        // Re-resolver manager cuando entramos a MapSelect
        _mgr = Object.FindFirstObjectByType<MapSelectManager>();
        _nextTime = 0f;
    }

    void EnsureMgr()
    {
        if (_mgr == null)
            _mgr = Object.FindFirstObjectByType<MapSelectManager>();
    }

    // === Input System (Action: Mover / Value Vector2) ===
    public void OnMover(InputValue v)
    {
        EnsureMgr();
        if (_mgr == null) return;

        float x = v.Get<Vector2>().x;
        if (Mathf.Abs(x) < 0.5f) return;        // zona muerta
        if (Time.time < _nextTime) return;      // anti-repeat muy rápido

        int dir = x > 0 ? +1 : -1;
        Debug.Log($"[MS] Player#{_pi?.playerIndex ?? -1} mover dir={dir}");
        _mgr.Move(dir);                         // <-- API nueva
        _nextTime = Time.time + repeatDelay;
    }

    // === Input System (Action: Iniciar / Button) ===
    public void OnIniciar()
    {
        EnsureMgr();
        if (_mgr == null) return;

        Debug.Log($"[MS] Player#{_pi?.playerIndex ?? -1} confirmar");
        _mgr.Play();                             // <-- cargar escena seleccionada
    }

    // === (Opcional) Input System (Action: Cancelar / Button) ===
    // Así puedes volver al menú con Esc / B
    public void OnCancelar()
    {
        EnsureMgr();
        if (_mgr == null) return;
        _mgr.Back();                             // <-- regresar a MainMenu
    }
}
