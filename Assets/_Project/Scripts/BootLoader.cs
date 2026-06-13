using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[DefaultExecutionOrder(-500)]
public class BootLoader : MonoBehaviour
{
    [Header("Config")]
    public string nextScene = "MainMenu";
    public float minSplashTime = 1.0f;   // tiempo mínimo de splash
    public bool autoLoad = true;

    [Header("UI")]
    public CanvasGroup rootGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI devicesText;
    public Slider progressBar;

    void Awake()
    {
        // Garantiza GameManager (y otros singletons si los tienes)
        if (GameManager.Instance == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    void Start()
    {
        if (autoLoad) StartCoroutine(LoadRoutine());
        else UpdateDevices();
    }

    IEnumerator LoadRoutine()
    {
        UpdateDevices();
        yield return new WaitForSeconds(0.1f);

        if (!Application.CanStreamedLevelBeLoaded(nextScene))
        {
            SetStatus($"Escena '{nextScene}' no está en Build Settings.");
            yield break;
        }

        float startTime = Time.time;
        SetStatus("Cargando escena…");

        var op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            SetProgress(op.progress / 0.9f);
            yield return null;
        }

        // Espera al menos el splash mínimo
        while (Time.time - startTime < minSplashTime)
            yield return null;

        // Fade out rápido (opcional)
        yield return Fade(0.15f, 0f);

        op.allowSceneActivation = true;
    }

    void SetStatus(string msg)
    {
        if (statusText) statusText.text = msg;
    }

    void SetProgress(float t)
    {
        if (progressBar) progressBar.value = Mathf.Clamp01(t);
    }

    void UpdateDevices()
    {
        bool hasKb = Keyboard.current != null;
        bool hasMs = Mouse.current != null;
        bool hasGp = Gamepad.current != null;
        if (devicesText)
            devicesText.text = $"Teclado {(hasKb ? "✓" : "✗")} | Mouse {(hasMs ? "✓" : "✗")} | Gamepad {(hasGp ? "✓" : "✗")}";
        Debug.Log($"[Boot] Devices: KB={hasKb} MS={hasMs} GP={hasGp}");
    }

    IEnumerator Fade(float duration, float toAlpha)
    {
        if (!rootGroup) yield break;
        float from = rootGroup.alpha;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            rootGroup.alpha = Mathf.Lerp(from, toAlpha, t);
            yield return null;
        }
        rootGroup.alpha = toAlpha;
    }
}
