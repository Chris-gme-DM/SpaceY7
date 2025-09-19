using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Script that handles the player interaction with ojects in the world
/// </summary>
public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("The layer that is interactable")]
    [SerializeField] private LayerMask interactableLayer;
    [Tooltip("Maximum distance")]
    [SerializeField] private float m_interactionRange = 5f;

    private IInteractable m_currentInteractable;
    private PlayerController m_playerController;
    [SerializeField] private CinemachineBrain m_cameraBrain;
    private Camera m_camera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(m_camera == null) { m_camera = Camera.main; }
        if(m_playerController == null) { m_playerController = FindAnyObjectByType<PlayerController>(); }
        m_playerController.OnInteractAction += OnInteractAction;
    }

    // Update is called once per frame
    void Update()
    {
        CheckforInteractable();
    }

    private void CheckforInteractable()
    {
        // Raycast to the point the player is lokking at
        Ray ray = m_camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        // if the ray hits an interactable
        if (Physics.Raycast(ray, out RaycastHit hit, m_interactionRange, interactableLayer ))
        {
            // Get the interactbale by the hit
            IInteractable newInteractable = hit.collider.gameObject.GetComponentInParent<IInteractable>();
            // if the newInteractable is not the same switch
            if (newInteractable != null && newInteractable != m_currentInteractable)
            {
                m_currentInteractable = newInteractable;

            }
        }
        else
        {
            // If the player leaves the interaction range
            if (m_currentInteractable != null)
            {
                m_currentInteractable = null;
            }
        }
    }
    public void OnInteractAction(InputAction.CallbackContext context)
    {
        if (m_currentInteractable != null)
        {
            m_currentInteractable.Interact(gameObject);
        }
        else
        {
            Debug.Log("Nothing toi interact with");
        }
    }
}
