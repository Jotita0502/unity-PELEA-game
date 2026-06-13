using UnityEngine;

[RequireComponent(typeof(JugadorMovimiento))]
public class SpeedBoostReceiver : MonoBehaviour
{
    [Header("Multiplicadores por defecto (no toques)")]
    public float multFuerzaMovimiento = 1f;
    public float multVelocidadMaxima = 1f;

    JugadorMovimiento move;
    float boostEndTime;
    float baseFuerza;
    float baseVelMax;

    void Awake()
    {
        move = GetComponent<JugadorMovimiento>();
        baseFuerza = move.fuerzaMovimiento;
        baseVelMax = move.velocidadMaxima;
        boostEndTime = 0f;
    }

    void Update()
    {
        if (boostEndTime > 0f && Time.time >= boostEndTime)
            ClearBoost();
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        multiplier = Mathf.Max(1f, multiplier);
        duration = Mathf.Max(0.1f, duration);

        move.fuerzaMovimiento = baseFuerza * multiplier;
        move.velocidadMaxima = baseVelMax * multiplier;

        multFuerzaMovimiento = multiplier;
        multVelocidadMaxima = multiplier;
        boostEndTime = Time.time + duration;
    }

    public void ClearBoost()
    {
        move.fuerzaMovimiento = baseFuerza;
        move.velocidadMaxima = baseVelMax;
        multFuerzaMovimiento = 1f;
        multVelocidadMaxima = 1f;
        boostEndTime = 0f;
    }
}
