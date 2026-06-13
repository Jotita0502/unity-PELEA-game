using System;                 // <— para Action<>
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{

    public event Action<int> OnHealthChanged;   
    public event Action<int> OnMaxHealthChanged; 

    [Header("Vida")]
    [SerializeField] int maxHealth = 20;
    [SerializeField] int currentHealth = 20;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    [Header("Referencias")]
    public Animator animator;
    public string dieTrigger = "Die";
    public string upperBodyLayerName = "UpperBodyMask";

    [Header("Al morir, desactivar:")]
    public MonoBehaviour[] componentsToDisable;

    Rigidbody rb;
    int upperBodyLayer = -1;
    bool dead;

    void Reset()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;
    }

    void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (Application.isPlaying)
        {
            OnMaxHealthChanged?.Invoke(maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        if (currentHealth <= 0) currentHealth = maxHealth;

        if (animator)
            upperBodyLayer = animator.GetLayerIndex(upperBodyLayerName);

        OnMaxHealthChanged?.Invoke(maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }


    public void ApplyDamage(int amount)
    {
        if (dead) return;
        amount = Mathf.Abs(amount);

        int old = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth != old)
            OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (dead) return;
        int old = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Abs(amount));
        if (currentHealth != old)
            OnHealthChanged?.Invoke(currentHealth);
    }

    public void SetMaxHealth(int newMax, bool keepRatio = true)
    {
        newMax = Mathf.Max(1, newMax);
        if (newMax == maxHealth) return;

        if (keepRatio && maxHealth > 0)
        {
            float ratio = (float)currentHealth / maxHealth;
            maxHealth = newMax;
            currentHealth = Mathf.RoundToInt(ratio * maxHealth);
        }
        else
        {
            maxHealth = newMax;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        OnMaxHealthChanged?.Invoke(maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }


    void Die()
    {
        if (dead) return;
        dead = true;

        foreach (var c in componentsToDisable)
            if (c) c.enabled = false;

        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = true;
        }

        if (animator && upperBodyLayer >= 0)
            animator.SetLayerWeight(upperBodyLayer, 0f);

        if (animator && !string.IsNullOrEmpty(dieTrigger))
        {
            animator.ResetTrigger(dieTrigger);
            animator.SetTrigger(dieTrigger);
            StartCoroutine(FreezeOnDeathEnd());
        }
    }

    IEnumerator FreezeOnDeathEnd()
    {
        int baseLayer = 0;
        while (!animator.GetCurrentAnimatorStateInfo(baseLayer).IsName("Death"))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(baseLayer).normalizedTime < 1f)
            yield return null;

        animator.enabled = false;
    }
}
