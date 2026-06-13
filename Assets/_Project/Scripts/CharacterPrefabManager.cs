using UnityEngine;

[DefaultExecutionOrder(-100)]
public class CharacterPrefabManager : MonoBehaviour
{
    [Header("Dónde van los modelos visuales")]
    public Transform visualParent;

    [Header("Prefabs de personajes disponibles")]
    public GameObject[] characterPrefabs;

    [Header("Animator Controller compartido (opcional)")]
    public RuntimeAnimatorController sharedController;

    [Header("Persistencia")]
    public bool autoApplyFromPrefs = true;
    public string prefsKey = "SkinIndex";

    GameObject current;
    Animator currentAnimator;

    public int Count => characterPrefabs != null ? characterPrefabs.Length : 0;

    void Awake()
    {
        if (!visualParent) visualParent = transform;


        int index = 0;
        if (autoApplyFromPrefs)
            index = PlayerPrefs.GetInt(prefsKey, 0);

        ApplyCharacterIndex(Mathf.Clamp(index, 0, Mathf.Max(0, Count - 1)));
    }

    public void ApplyAndSave(int index)
    {
        ApplyCharacterIndex(index);
        PlayerPrefs.SetInt(prefsKey, index);
        PlayerPrefs.Save();
    }

    public void ApplyCharacterIndex(int index)
    {
        // Limpia anterior
        if (current) Destroy(current);

        if (Count == 0 || index < 0 || index >= Count)
        {
            Debug.LogWarning("[CharacterPrefabManager] Índice inválido o lista vacía.");
            current = null;
            currentAnimator = null;
            PushAnimatorToCore(null);
            return;
        }

        // Instancia
        var prefab = characterPrefabs[index];
        current = Instantiate(prefab, visualParent);
        current.transform.localPosition = Vector3.zero;
        current.transform.localRotation = Quaternion.identity;
        current.transform.localScale = Vector3.one;


        currentAnimator = current.GetComponentInChildren<Animator>(true);
        if (!currentAnimator)
            currentAnimator = current.AddComponent<Animator>();

        if (sharedController)
            currentAnimator.runtimeAnimatorController = sharedController;


        PushAnimatorToCore(currentAnimator);
    }

    void PushAnimatorToCore(Animator anim)
    {

        var jm = GetComponent<JugadorMovimiento>();
        if (jm) jm.SetAnimator(anim);

        var ja = GetComponent<JugadorAnimacion>();
        if (ja) ja.animator = anim;

        var atk = GetComponent<PlayerAttackTrigger>();
        if (atk) atk.SetAnimator(anim);
    }
}
