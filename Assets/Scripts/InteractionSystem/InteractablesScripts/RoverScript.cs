using UnityEngine;

public class RoverScript : BaseInteractable
{
    private Rigidbody m_rb;
    private float m_maxDistanceToPlayer;
    private float m_minDistanceToPlayer; // may be redundant
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Follow the player 
    }
    public override void Interact(GameObject interactor)
    {
        // Open the Rover Menu;
    }

}
