using UnityEngine;

public class SkeletonAutoMove : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 2f;              // Velocidad de movimiento
    public float moveDistance = 5f;       // Distancia hacia adelante antes de girar

    [Header("Animación")]
    private Animator animator;            // Referencia al Animator

    private Vector3 startPos;             // Posición inicial
    private Vector3 endPos;               // Posición final
    private bool movingForward = true;    // Dirección actual
    private bool isMoving = false;        // Estado del movimiento

    void Start()
    {
        // Guardamos la posición inicial y calculamos el punto final
        startPos = transform.position;
        endPos = startPos + transform.forward * moveDistance;

        // Obtenemos el Animator en el mismo objeto
        animator = GetComponent<Animator>();

        // Empezar caminando
        SetWalking(true);
    }

    void Update()
    {
        // Determinar destino actual (ida o vuelta)
        Vector3 target = movingForward ? endPos : startPos;

        // Mover al esqueleto hacia el destino
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Detectar si está en movimiento o llegó al destino
        bool reachedTarget = Vector3.Distance(transform.position, target) < 0.05f;

        if (reachedTarget)
        {
            // Si llegó, detenerse y girar
            SetWalking(false);
            movingForward = !movingForward;
            transform.Rotate(0f, 180f, 0f);

            // Esperar un poco antes de seguir
            Invoke(nameof(StartWalkingAgain), 1f);
        }
    }

    void StartWalkingAgain()
    {
        SetWalking(true);
    }

    void SetWalking(bool state)
    {
        isMoving = state;
        if (animator != null)
        {
            animator.SetBool("isWalking", state);
        }
    }
}
