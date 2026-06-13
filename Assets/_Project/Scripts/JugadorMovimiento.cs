using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using FishNet.Object;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NetworkObject))]
public class JugadorMovimiento : NetworkBehaviour
{
    [Header("Movimiento")]
    public float fuerzaMovimiento = 20f;
    public float velocidadMaxima = 4f;

    [Header("Freno / Drag")]
    public float dragSuelo = 4f;
    public float dragAire = 0.2f;
    public float fuerzaFreno = 10f;
    public float umbralParado = 0.1f;

    [Header("Salto")]
    public float fuerzaSalto = 4f;

    [Header("Chequeo de suelo")]
    public Transform chequeoSuelo;
    public LayerMask capaSuelo = ~0;
    public float radioSuelo = 0.3f;

    [Header("Controles")]
    [SerializeField] bool movimientoRelativoACamara = true;
    [SerializeField] bool invertirX = false;
    [SerializeField] bool invertirY = true;

    [Header("Animación (salto)")]
    [SerializeField] Animator animator;
    [SerializeField] string paramIsGrounded = "IsGrounded";
    [SerializeField] string triggerJump = "Jump";

    Rigidbody rb;

#if ENABLE_INPUT_SYSTEM
    Vector2 moverInput;
    bool saltoSolicitado;
#endif

    public override void OnStartClient()
    {
        base.OnStartClient();

        rb = GetComponent<Rigidbody>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        // Muy importante: sólo el dueño simula físicas locales.
        rb.isKinematic = !IsOwner;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Habilita el PlayerInput sólo en el dueño (si lo usas).
#if ENABLE_INPUT_SYSTEM
        var pi = GetComponent<PlayerInput>();
        if (pi) pi.enabled = IsOwner;
#endif
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        if (!animator)
            Debug.LogError("[Movimiento] Animator NO asignado / no encontrado en hijos.");
        if (!chequeoSuelo)
            Debug.LogWarning("[Movimiento] 'chequeoSuelo' NO asignado. El salto dependerá de esto.");
    }

    void FixedUpdate()
    {
        if (!IsOwner)
        {
            if (animator) animator.SetBool(paramIsGrounded, EstaEnElSuelo());
            return;
        }
        Vector2 m;
#if ENABLE_INPUT_SYSTEM
        m = moverInput;
#else
        m = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif
        if (Mathf.Abs(m.x) < 0.001f) m.x = 0f;
        if (Mathf.Abs(m.y) < 0.001f) m.y = 0f;

        if (invertirX) m.x = -m.x;
        if (invertirY) m.y = -m.y;

        Vector3 inputDir;
        if (movimientoRelativoACamara && Camera.main != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 camFwd = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;
            inputDir = camFwd * m.y + camRight * m.x;
        }
        else
        {
            inputDir = new Vector3(m.x, 0f, m.y);
        }

        bool enSuelo = EstaEnElSuelo();


        if (animator) animator.SetBool(paramIsGrounded, enSuelo);


        if (inputDir.sqrMagnitude > 0.0001f)
            rb.AddForce(inputDir.normalized * fuerzaMovimiento, ForceMode.Acceleration);


        rb.linearDamping = enSuelo ? dragSuelo : dragAire;


        if (enSuelo && inputDir.sqrMagnitude < 0.0001f)
        {
            Vector3 vel = rb.linearVelocity;
            Vector3 horiz = new Vector3(vel.x, 0f, vel.z);
            rb.AddForce(-horiz * fuerzaFreno, ForceMode.Acceleration);
            if (horiz.magnitude < umbralParado)
                rb.linearVelocity = new Vector3(0f, vel.y, 0f);
        }


        {
            Vector3 vel = rb.linearVelocity;
            Vector3 horiz = new Vector3(vel.x, 0f, vel.z);
            if (horiz.magnitude > velocidadMaxima)
                rb.linearVelocity = horiz.normalized * velocidadMaxima + Vector3.up * vel.y;
        }


#if ENABLE_INPUT_SYSTEM
        if (saltoSolicitado)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.VelocityChange);
            saltoSolicitado = false;
        }
#else
        if (enSuelo && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.VelocityChange);
            if (animator)
            {
                animator.ResetTrigger(triggerJump);
                animator.SetTrigger(triggerJump);
                animator.SetBool(paramIsGrounded, false);
            }
        }
#endif
    }

    bool EstaEnElSuelo()
    {
        if (!chequeoSuelo) return false;
        Vector3 p1 = chequeoSuelo.position + Vector3.up * 0.10f; // arriba
        Vector3 p2 = chequeoSuelo.position - Vector3.up * 0.05f; // abajo
        return Physics.CheckCapsule(p1, p2, radioSuelo, capaSuelo, QueryTriggerInteraction.Ignore);
    }

#if ENABLE_INPUT_SYSTEM
    // PlayerInput (Send Messages) — sólo dueño
    void OnMover(InputValue value)
    {
        if (!IsOwner) return;
        moverInput = value.Get<Vector2>();
    }

    void OnSaltar(InputValue v)
    {
        if (!IsOwner) return;
        if (!v.isPressed) return;
        if (!EstaEnElSuelo()) return;

        saltoSolicitado = true;

        if (animator)
        {
            animator.ResetTrigger(triggerJump);
            animator.SetTrigger(triggerJump);
            animator.SetBool(paramIsGrounded, false);
        }
    }
#endif

    public void SetAnimator(Animator a)
    {
        animator = a;
        if (!animator)
            Debug.LogError("[JugadorMovimiento] SetAnimator recibió null.");
        else
            animator.applyRootMotion = false; // por si acaso
    }

    void OnDrawGizmosSelected()
    {
        if (!chequeoSuelo) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(chequeoSuelo.position, radioSuelo);
    }
}
