using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Slider slider;

    Health target;

    void Awake()
    {
        if (!slider) slider = GetComponentInChildren<Slider>(true);
    }

    public void Bind(Health h)
    {
        if (target != null)
        {
            target.OnHealthChanged -= OnHealthChanged;
            target.OnMaxHealthChanged -= OnMaxChanged;
        }

        target = h;

        if (target != null)
        {
            slider.maxValue = target.MaxHealth;
            slider.value = target.CurrentHealth;
            target.OnHealthChanged += OnHealthChanged;
            target.OnMaxHealthChanged += OnMaxChanged;
        }
        else
        {
            slider.value = 0f;
        }
    }

    void OnDestroy()
    {
        if (target != null)
        {
            target.OnHealthChanged -= OnHealthChanged;
            target.OnMaxHealthChanged -= OnMaxChanged;
        }
    }

    void OnHealthChanged(int cur) => slider.value = cur;
    void OnMaxChanged(int max) => slider.maxValue = max;
}
