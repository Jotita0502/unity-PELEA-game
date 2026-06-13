using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JugadorAnimacion : MonoBehaviour
{
    public Transform visual;       
    public Animator animator;       
    public float velocidadMaxima = 6f;     
    public float girosPorSegundo = 540f;   
    public float umbralGiro = 0.15f;      
    public float dampingVel = 0.15f;       

    Rigidbody rb;
    float velParam;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
        if (!visual && transform.childCount > 0) visual = transform.GetChild(0);
        if (!animator && visual) animator = visual.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!animator) return;


        Vector3 v = rb.linearVelocity; v.y = 0f;
        float speed = v.magnitude;


        float deadzone = 0.05f;          // m/s
        if (speed < deadzone) speed = 0f;


        float target = Mathf.Clamp01(speed / Mathf.Max(0.01f, velocidadMaxima));


        float velUp = 10f;    
        float velDown = 18f;            
        float rate = (target < velParam) ? velDown : velUp;

        velParam = Mathf.MoveTowards(velParam, target, rate * Time.deltaTime);
        animator.SetFloat("Velocidad", velParam);
    }


    void LateUpdate()
    {
        if (!visual) return;


        Vector3 dir = rb.linearVelocity; dir.y = 0f;
        if (dir.magnitude < umbralGiro) return;

        Quaternion objetivo = Quaternion.LookRotation(dir.normalized, Vector3.up);
        visual.rotation = Quaternion.RotateTowards(visual.rotation, objetivo, girosPorSegundo * Time.deltaTime);
    }
    public void SetAnimator(Animator a)
    {
        animator = a;
        if (!animator)
            Debug.LogError("[JugadorAnimacion] SetAnimator recibió null.");
    }


    public void SetVisual(Transform v)
    {
        visual = v;
    }
}
