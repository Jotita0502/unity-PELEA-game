using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections;
using FishNet.Object;

public class PlayerAttackTrigger : NetworkBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string triggerName = "Attack";
    [SerializeField] string upperBodyLayerName = "UpperBodyMask";
    [SerializeField] float minCooldown = 0.05f;

    int upperBodyLayer;
    bool atacando;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!animator) animator = GetComponentInChildren<Animator>(true);
        RecalcUpperBodyLayer();
    }

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);
        RecalcUpperBodyLayer();
    }

#if ENABLE_INPUT_SYSTEM
    void OnGolpe(InputValue v)
    {
        if (!IsOwner) return;
        if (!v.isPressed || atacando || animator == null)
            return;

        atacando = true;


        animator.ResetTrigger(triggerName);
        animator.SetTrigger(triggerName);


        ServerRpc_RequestAttack();

        StartCoroutine(EsperarFinDeGolpe());
    }
#endif

    IEnumerator EsperarFinDeGolpe()
    {
        yield return null;


        while (true)
        {
            var info = animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
            if (info.IsName("Punch")) break;

            if (animator.IsInTransition(upperBodyLayer))
            {
                var next = animator.GetNextAnimatorStateInfo(upperBodyLayer);
                if (next.IsName("Punch")) break;
            }
            yield return null;
        }


        const float releaseAt = 0.9f;
        while (true)
        {
            var info = animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
            if (!info.IsName("Punch")) break;

            if (animator.IsInTransition(upperBodyLayer))
            {
                var next = animator.GetNextAnimatorStateInfo(upperBodyLayer);
                if (!next.IsName("Punch")) break;
            }
            if (info.normalizedTime >= releaseAt) break;
            yield return null;
        }

        if (minCooldown > 0f) yield return new WaitForSeconds(minCooldown);
        atacando = false;
    }


    [ServerRpc]
    private void ServerRpc_RequestAttack()
    {

        ObserversRpc_PlayAttack();
    }


    [ObserversRpc(ExcludeOwner = true, RunLocally = false, BufferLast = false)]
    private void ObserversRpc_PlayAttack()
    {
        if (!animator) return;
        animator.ResetTrigger(triggerName);
        animator.SetTrigger(triggerName);
    }

    /* --------------------------------------------- */

    public void SetAnimator(Animator a)
    {
        animator = a;
        RecalcUpperBodyLayer();
        if (!animator)
            Debug.LogError("[PlayerAttackTrigger] SetAnimator recibió null.");
    }

    void RecalcUpperBodyLayer()
    {
        upperBodyLayer = (animator ? animator.GetLayerIndex(upperBodyLayerName) : -1);
        if (upperBodyLayer < 0 && animator)
            Debug.LogWarning($"[PlayerAttackTrigger] No se encontró la capa '{upperBodyLayerName}' en {animator.runtimeAnimatorController?.name}");
    }
}
