using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelectInput : MonoBehaviour
{
    public float deadzone = 0.25f;
    public float repeatDelay = 0.18f;

    private CharacterSelectManager _mgr;
    private PlayerInput _pi;
    private float _next;

    void Awake()
    {
        _pi = GetComponent<PlayerInput>();
        SceneManager.sceneLoaded += (s, m) => { _mgr = Object.FindFirstObjectByType<CharacterSelectManager>(); };
    }

    void EnsureMgr()
    {
        if (_mgr == null) _mgr = Object.FindFirstObjectByType<CharacterSelectManager>();
    }

    public void OnMover(InputValue v)
    {
        EnsureMgr();
        if (_mgr == null) return; // aún no estamos en CharacterSelect

        float x = v.Get<Vector2>().x;
        if (Mathf.Abs(x) < deadzone) return;
        if (Time.time < _next) return;

        int dir = x > 0 ? +1 : -1;
        Debug.Log($"[CS] Player#{_pi.playerIndex} Mover x={x} dir={dir}");
        _mgr.ChangeSkin(_pi, dir);
        _next = Time.time + repeatDelay;
    }

    public void OnIniciar()
    {
        EnsureMgr();
        if (_mgr == null) return; // en MainMenu no hace nada
        Debug.Log($"[CS] Player#{_pi.playerIndex} Iniciar (toggle listo)");
        _mgr.ToggleReady(_pi);
    }
}
