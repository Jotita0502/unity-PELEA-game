using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class PyramidAutoBury_InitialWait : MonoBehaviour
{
    [Header("Tiempos")]
    public float initialWaitUp = 2f;  // espera inicial arriba (visible)
    public float moveTime = 3f;  // segundos en bajar o subir
    public float waitDown = 5f;  // tiempo enterrada
    public float waitUp = 3f;  // tiempo visible arriba entre ciclos

    [Header("Detección de suelo")]
    public LayerMask groundMask = ~0;   // capas consideradas “suelo”
    public float buryMargin = 0.03f;// margen por debajo del suelo
    public float fallbackDepth = 1.2f; // profundidad de respaldo si no hay suelo

    Vector3 startPos;
    const float EPS = 0.0001f;

    void Awake()
    {
        startPos = transform.position; // posición visible (arriba)
    }

    void OnEnable()
    {
        StopAllCoroutines();
        transform.position = startPos; // arranca visible
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        // 1) Espera inicial visible (2s por defecto)
        if (initialWaitUp > 0f) yield return new WaitForSeconds(initialWaitUp);

        // Calcula el destino “abajo” para quedar oculta
        Vector3 down = ComputeBuriedPosition();

        while (true)
        {
            // 2) Baja suave hasta quedar oculta
            yield return MoveWorld(startPos, down, moveTime);

            // 3) Espera enterrada
            yield return new WaitForSeconds(waitDown);

            // 4) Sube suave
            yield return MoveWorld(down, startPos, moveTime);

            // 5) Espera visible arriba
            yield return new WaitForSeconds(waitUp);
        }
    }

    Vector3 ComputeBuriedPosition()
    {
        var col = GetComponent<Collider>();
        Bounds b = col.bounds;

        // Raycast vertical hacia abajo para hallar el suelo bajo la pirámide
        Vector3 rayStart = new Vector3(b.center.x, b.max.y + 2f, b.center.z);
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 100f, groundMask, QueryTriggerInteraction.Ignore))
        {
            float groundY = hit.point.y;
            // Queremos que el “techo” (b.max.y) quede bajo el suelo en buryMargin
            float deltaY = (groundY - buryMargin) - b.max.y; // negativo para bajar
            float targetY = transform.position.y + deltaY;
            return new Vector3(startPos.x, targetY, startPos.z);
        }

        // Si no hay suelo detectado, usa un descenso fijo de respaldo
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
