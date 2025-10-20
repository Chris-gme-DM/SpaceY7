using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// This script handles the Rover. I don't have time to go into details right now
/// </summary>
public class RoverScript : BaseInteractable
{
    #region Settings
    private Transform m_playerTransform;
    private NavMeshAgent m_agent;
    private float distanceToPlayer;
    [SerializeField] private float m_minFollowDistance = 4f;
    private bool m_isHovering;
    public bool IsHovering => m_isHovering;
    #endregion
    #region Important
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_playerTransform == null )
        {
            m_playerTransform = FindAnyObjectByType<PlayerController>().transform;
        }
        if ( m_agent == null )
        {
            m_agent = GetComponent<NavMeshAgent>();
        }
        m_agent.updatePosition = true;
        m_agent.updateRotation = true;
    }
    void Update()
    {
        if (m_playerTransform == null || m_agent == null) return;
    }
    void FixedUpdate()
    {
        // Check distance to player
        distanceToPlayer = Vector3.Distance(transform.position, m_playerTransform.position);
        HandleNavMeshMovement();
    }
    private void HandleNavMeshMovement()
    {
        // Check if the Agent is currently on a baked surface.
        if (!m_agent.isOnNavMesh) return;

        // 1. Get the player's current position (the target).
        Vector3 playerPosition = m_playerTransform.position;

        // 2. The ONLY necessary complexity: Find the ground under the player.
        // This prevents the Rover from freezing or snapping when the player jumps.
        // It projects the player's 3D position down onto the 2D NavMesh surface.
        if (NavMesh.SamplePosition(playerPosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            // 3. Set the destination to the ground spot under the player.
            // The Agent's built-in Stopping Distance (set to 8.0f) will now handle the offset and stopping.
            m_agent.SetDestination(hit.position);
        }
    }
    public override void Interact(GameObject interactor)
    {
        if( distanceToPlayer <= m_minFollowDistance )
        {
            // Open the Rover Menu;
            UIManager.Instance.TogglePlayerMenu();
        }
    }
    #endregion
}
