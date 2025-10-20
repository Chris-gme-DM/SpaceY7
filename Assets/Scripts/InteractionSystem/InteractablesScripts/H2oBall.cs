using UnityEngine;

public class H2oBall : BaseInteractable
{
    [Header("Movement Settings")]
    [SerializeField] private float m_sidewaysForce = 2f;
    [SerializeField] private float m_forceInterval = 5f;

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
        m_forceTimer -= Time.deltaTime;
        if (m_forceTimer <= 0f)
        {
            ApplyRandomForce();
            m_forceTimer = m_forceInterval;
        }
    }

    private void ApplyRandomForce()
    {
        // Generate random direction and apply force
        Vector3 randomForce = new(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        rb.AddForce(randomForce * m_sidewaysForce, ForceMode.Impulse);
    }

    public override void Interact(GameObject interactor)
    {
        PlayerStats.Instance.ChangeVitals(interactableData.Resources);
        Destroy(gameObject);
    }

}
