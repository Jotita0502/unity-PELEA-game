using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class RandomPatrolAgent : MonoBehaviour
{
    [Header("Waypoints (cualquier orden)")]
    public Transform[] points;

    [Header("Comportamiento")]
    [Tooltip("Tiempo que espera en cada punto antes de ir al siguiente")]
    public float waitAtPoint = 1.0f;

    [Header("Animación")]
    [Tooltip("Nombre del parámetro float en el Animator")]
    public string speedParam = "Speed";

    [Header("NavMesh")]
    [Tooltip("Distancia para considerar que llegó al punto")]
    public float arriveTolerance = 0.15f;

    NavMeshAgent agent;
    Animator animator;
    int currentIndex = -1;
    float waitTimer;
    bool hasDestination;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        waitTimer = 0f;
        hasDestination = false;
        PickAndGoToNextPoint(startNearest: true);
    }

    void Update()
    {
        // sincroniza animación
        animator.SetFloat(speedParam, agent.velocity.magnitude);

        if (points == null || points.Length == 0) return;
        if (!hasDestination) return;

        if (!agent.pathPending && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, arriveTolerance))
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitAtPoint)
            {
                waitTimer = 0f;
                PickAndGoToNextPoint(startNearest: false);
            }
        }
    }

    void PickAndGoToNextPoint(bool startNearest)
    {
        if (points == null || points.Length == 0) return;

        int nextIndex;
        if (startNearest && currentIndex == -1)
        {
            // primera vez: usa el más cercano
            float best = float.MaxValue;
            nextIndex = 0;
            for (int i = 0; i < points.Length; i++)
            {
                if (!points[i]) continue;
                float d = (points[i].position - transform.position).sqrMagnitude;
                if (d < best) { best = d; nextIndex = i; }
            }
        }
        else
        {
            if (points.Length == 1) nextIndex = 0;
            else
            {
                do { nextIndex = Random.Range(0, points.Length); }
                while (nextIndex == currentIndex || points[nextIndex] == null);
            }
        }

        currentIndex = nextIndex;
        var target = points[currentIndex].position;

        if (NavMesh.SamplePosition(target, out var hit, 1.0f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(target);

        hasDestination = true;
    }

    void OnDrawGizmosSelected()
    {
        if (points == null) return;
        Gizmos.matrix = Matrix4x4.identity;
        foreach (var t in points)
        {
            if (!t) continue;
            Gizmos.DrawWireSphere(t.position, 0.25f);
        }
    }
}
