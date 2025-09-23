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
    private Rigidbody m_rb;
    private NavMeshAgent m_agent;
    [SerializeField] private float m_minFollowDistance = 5f;
    [SerializeField] private float m_hoverForceUp = 200f;
    [SerializeField] private float m_hoverForceForward = 40f;
    [SerializeField] private float m_hoverDuration = 5f;
    [SerializeField] private float m_rotationSpeed = 5f;
    private bool m_isHovering;
    private float m_hoverTimer;
    #endregion
    #region Important
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_agent = GetComponent<NavMeshAgent>();
        if (m_playerTransform == null )
        {
            m_playerTransform = FindAnyObjectByType<PlayerController>().transform;
        }
        m_agent.updatePosition = false;
        m_agent.updateRotation = false;

        StartNavMeshMode();
        m_isHovering = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Determine distance between Rover and player
        float distanceToPlayer = Vector3.Distance(transform.position, m_playerTransform.position);
        // Determine direction of player

        if (m_isHovering)
        {
            StartCoroutine(Hover());
        }
        else
        {
            HandleNavMeshMovement(distanceToPlayer);
        }
    }
    private void FixedUpdate()
    {
        if (!m_isHovering)
        {
            // Determine new position for the agent to follow
            Vector3 newPosition = Vector3.Lerp(m_rb.position, m_agent.nextPosition, Time.fixedDeltaTime * 10f);
            m_rb.MovePosition(newPosition);

            if (m_agent.velocity.magnitude >= 0.1f)
            {
                Vector3 direction = m_agent.velocity.normalized;
                Rotate(direction);
            }
            
        }
    }

    public override void Interact(GameObject interactor)
    {
        // Open the Rover Menu;
    }
    #endregion
    #region Movement
    private void HandleNavMeshMovement(float distanceToPlayer)
    {
        if (m_agent.isOnNavMesh)
        {
            m_agent.isStopped = false;
            m_agent.SetDestination(m_playerTransform.position);

            if(distanceToPlayer <= m_minFollowDistance)
            {
                m_agent.isStopped = true;
            }
            if (m_agent.pathStatus == NavMeshPathStatus.PathPartial && distanceToPlayer >= m_minFollowDistance)
            {
                StartCoroutine(Hover());
            }

        }

    }
    private void Rotate(Vector3 direction)
    {
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * m_rotationSpeed);
    }
    private IEnumerator Hover()
    {
        m_isHovering = true;
        m_agent.enabled = false;
        m_rb.isKinematic = false;
        m_rb.useGravity = false;

        m_hoverTimer = m_hoverDuration;
        while (m_hoverTimer > 0)
        {
            Vector3 directionOfPlayer = (m_playerTransform.position - transform.position).normalized;
            directionOfPlayer.y = 0;
            directionOfPlayer.Normalize();

            m_rb.AddForce(m_hoverForceUp * Time.deltaTime * Vector3.up, ForceMode.Force);
            m_rb.AddForce(m_hoverForceForward * Time.deltaTime * directionOfPlayer, ForceMode.Force);

            m_hoverTimer -= Time.deltaTime;
            yield return null;
        }
        StartNavMeshMode();
    }

    #endregion
    private void StartNavMeshMode()
    {
        m_isHovering = false;
        m_agent.enabled = true;
        m_agent.isStopped = false;
        m_rb.isKinematic = true;
        m_rb.useGravity = true;

    }
}
