using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator m_animator;
    private PlayerController m_playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_playerController == null) { m_playerController = GetComponent<PlayerController>(); }
        if (m_animator == null) { m_animator = GetComponent<Animator>(); }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool m_isBuilderModeActive = m_playerController.IsBuilding;
        if (m_isBuilderModeActive)
        {
            // Activate Animation Layer if player is currently in buider mode
            m_animator.SetLayerWeight(1, 0f);
            m_animator.SetLayerWeight(2, 1f);
        }
        else
        {
            // Default to Explorer Mode
            m_animator.SetLayerWeight(1, 1f);
            m_animator.SetLayerWeight(2, 0f);
        }
        SetMovementParams();
    }

    public void SetAnimationTriggers(string triggerName)
    {
        if (m_animator == null)
        {
            Debug.LogError("ANIMATOR IS NULL! Check references on Player object.");
            return;
        }

        Debug.Log($"ANIMATION FIRED: {triggerName}");
        m_animator.SetTrigger(triggerName);
    }

    private void SetMovementParams()
    {
        m_animator.SetBool("IsIdle", m_playerController.IsIdle);
        m_animator.SetBool("IsGrounded", m_playerController.IsGrounded);
        m_animator.SetBool("IsSprinting", m_playerController.IsSprinting);
        m_animator.SetBool("IsJetpackActive", m_playerController.IsJetpackActive);
    }
}
