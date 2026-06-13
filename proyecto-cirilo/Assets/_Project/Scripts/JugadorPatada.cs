using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object; 
using System.Collections;

public class JugadorPatada : NetworkBehaviour
{
    // Cambiado: Usa "Patada" ya que es el nombre del Trigger en tu Animator.
    private const string PATADA_TRIGGER_NAME = "Patada"; 
    
    // Cambiado: Usa "Patada" ya que es el nombre del estado en tu Animator.
    private const string PATADA_STATE_NAME = "Patada"; 

    // Referencia al Animator. 
    private Animator animator;
    
    private bool isAttacking = false;
    private const float MIN_COOLDOWN = 0.3f; 

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>(true);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!animator) animator = GetComponentInChildren<Animator>(true);
    }
    
    // Método de Input del New Input System
    public void OnPatada(InputValue v)
    {
        if (!IsOwner) 
            return;
        
        // Ejecuta solo si es presionado y no está en cooldown
        if (v.isPressed && !isAttacking)
        {
            ServerRpc_RequestPatada();
            StartCoroutine(WaitForCooldown());
        }
    }

    // Cliente -> Servidor
    [ServerRpc]
    private void ServerRpc_RequestPatada()
    {
        ObserversRpc_PlayPatada();
    }

    // Servidor -> Todos los Clientes
    [ObserversRpc]
    private void ObserversRpc_PlayPatada()
    {
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado al intentar sincronizar la patada.");
            return;
        }
        
        // Usa el Trigger corregido "Patada"
        animator.ResetTrigger(PATADA_TRIGGER_NAME);
        animator.SetTrigger(PATADA_TRIGGER_NAME);
    }

    private IEnumerator WaitForCooldown()
    {
        isAttacking = true;
        
        yield return null; 

        float waitTime = 0f;
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                // Busca el clip que coincida con el nombre del estado "Patada"
                if (clip.name.Contains(PATADA_STATE_NAME))
                {
                    waitTime = clip.length;
                    break;
                }
            }
        }
        
        if (waitTime < MIN_COOLDOWN)
        {
            waitTime = MIN_COOLDOWN;
        }

        yield return new WaitForSeconds(waitTime);
        isAttacking = false;
    }
}