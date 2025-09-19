using UnityEngine;
using System.Collections;
using UnityEditor.Animations;
using System.Runtime.InteropServices;

public class EnyBall : BaseInteractable, IInteractable
{
    [Header("Movement Settings")]
    [SerializeField] private float m_forceMagnitude = 1f;
    [SerializeField] private float m_torqueMagnitude = 1f;
    [SerializeField] private float m_forceInterval = 2f;

    private float m_forceTimer;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_forceTimer = Random.Range(0f, m_forceInterval);
    }
    private void FixedUpdate()
    {
        if (rb != null)
        {
            // After timer runs out
            m_forceTimer -= Time.deltaTime;
            if (m_forceTimer <= 0f)
            {
                ApplyRandomForce();
                m_forceTimer = m_forceInterval;
            }
            // Move around

        }
    }

    private void ApplyRandomForce()
    {
        // Generate random direction and torque and apply them to the object
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        rb.AddForce(randomDirection * m_forceMagnitude, ForceMode.Impulse);

        Vector3 randomTorque = Random.insideUnitSphere.normalized;
        rb.AddTorque(randomTorque * m_torqueMagnitude, ForceMode.Impulse);
    }

    public override void Interact(GameObject interactor)
    {
        // Add Energy to Player
        PlayerStats.Instance.ChangeVitals(interactableData.Resources); // This calls upon a method in Player Stats that changes the vitals of the player
        // Make sound?

        // Destroy itself
        Destroy(gameObject);
    }
}
