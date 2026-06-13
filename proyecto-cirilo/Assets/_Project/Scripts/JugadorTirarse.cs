using UnityEngine;
using UnityEngine.InputSystem;

public class JugadorTirarse : MonoBehaviour
{
    private Animator animator;
    private ControlesJugador controles;

    void Awake()
    {
        controles = new ControlesJugador();
    }

    void OnEnable()
    {
        controles.Jugador.Enable();
        controles.Jugador.Tirarse.performed += OnTirarse;
    }

    void OnDisable()
    {
        controles.Jugador.Tirarse.performed -= OnTirarse;
        controles.Jugador.Disable();
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTirarse(InputAction.CallbackContext context)
    {
        if (context.performed && animator != null)
        {
            Debug.Log(" Animacion de tirarse activada");
            animator.SetTrigger("Tirarse");
        }
    }
}
