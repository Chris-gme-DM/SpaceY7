using UnityEngine;

public class OxyBall : BaseInteractable, IInteractable
{
    [Header("Movement Settings")]
    [SerializeField] private float m_upwardForce = 2f;
    [SerializeField] private float m_sidewaysForce = 1f;
    [SerializeField] private float m_forceInterval = 2f;

    private float m_forceTimer;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_forceTimer = Random.Range(0f, m_forceInterval);
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (rb != null)
        {
            m_forceTimer -= Time.deltaTime;
            if (m_forceTimer <= 0f)
            {
                ApplyRandomForce();
                m_forceTimer = m_forceInterval;
            }
        }
    }
    private void ApplyRandomForce()
    {
        // Generate random Forces to apply to the object
        rb.AddForce(Vector3.up * Random.Range(0f, m_upwardForce), ForceMode.Impulse);

        Vector3 randomSidewaysForce = new (Random.Range(-m_sidewaysForce, m_sidewaysForce), 0, Random.Range(-m_sidewaysForce, m_sidewaysForce));
        rb.AddForce(randomSidewaysForce, ForceMode.Impulse);

    }
    public override void Interact(GameObject interactor)
    {
        // Add Oxygen to player
        PlayerStats.Instance.ChangeVitals(interactableData.Resources);
        // Destroy this
        Destroy(gameObject);

    }
}