using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class PyramidAutoBury : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveTime = 3f;      // segundos en bajar o subir (más alto = más lento)

    [Header("Tiempos de espera")]
    public float waitDown = 5f;      // tiempo enterrada antes de subir
    public float waitUp = 3f;      // tiempo visible arriba antes de volver a bajar

    [Header("Detección de suelo")]
    public LayerMask groundMask = ~0;   // capas consideradas como “suelo”
    public float buryMargin = 0.03f;    // cuánto queda por debajo del suelo (margen)
    public float fallbackDepth = 1.2f;  // profundidad de respaldo si no hay hit

    Vector3 startPos;                   // posición visible (arriba)
    const float EPS = 0.0001f;

    void Awake()
    {
        startPos = transform.position;  // guarda la posición de escena como “arriba”
    }

    void OnEnable()
    {
        StopAllCoroutines();
        transform.position = startPos;  // empieza visible
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        // Calcula el Y de suelo y la Y objetivo “abajo” para que la pirámide quede oculta
        Vector3 down = ComputeBuriedPosition();

        while (true)
        {
            // Baja suave hasta quedar completamente oculta
            yield return MoveWorld(startPos, down, moveTime);

            // Espera enterrada
            yield return new WaitForSeconds(waitDown);

            // Sube suave
            yield return MoveWorld(down, startPos, moveTime);

            // Espera visible
            yield return new WaitForSeconds(waitUp);
        }
    }

    Vector3 ComputeBuriedPosition()
    {
        // Bounds actuales de la pirámide
        var col = GetComponent<Collider>();
        Bounds b = col.bounds;

        // Raycast hacia abajo para encontrar el Y del suelo justo debajo
        Vector3 rayStart = new Vector3(b.center.x, b.max.y + 2f, b.center.z);
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 100f, groundMask, QueryTriggerInteraction.Ignore))
        {
            float groundY = hit.point.y;

            // Queremos que la "tapa" (top) del objeto quede por debajo del suelo
            // Top nuevo = b.max.y + deltaY  <= groundY - buryMargin
            float deltaY = (groundY - buryMargin) - b.max.y;  // deltaY negativo
            float targetY = transform.position.y + deltaY;

            return new Vector3(startPos.x, targetY, startPos.z);
        }

        // Si no encontramos suelo, usamos una profundidad de respaldo
        return startPos + Vector3.down * Mathf.Abs(fallbackDepth);
    }

    IEnumerator MoveWorld(Vector3 from, Vector3 to, float duration)
    {
        duration = Mathf.Max(0.01f, duration);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float eased = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            transform.position = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }

        // Snap exacto al destino (evita deriva)
        transform.position = to;
        if ((transform.position - to).sqrMagnitude > EPS)
            transform.position = to;
    }
}
