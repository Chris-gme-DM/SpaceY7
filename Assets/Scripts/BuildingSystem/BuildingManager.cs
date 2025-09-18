using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

/// <summary>
/// LAST EDIT: Chris, Adjusted the method to Check for valid placement to account for the bulding perfabs
/// This script should handle the logic of placement and manipulation of the buildings the player can interact with.
/// The player interacts with the rover to enter the building menu to select which building she wants to create.
/// The buildingUI handles which object to create and feed that to this script.
/// </summary>
public class BuildingManager : MonoBehaviour
{
    #region References
    // Hold all relevant references
    public static BuildingManager Instance { get; private set; }
    [Header("References")]
    [SerializeField] private CinemachineBrain m_cameraBrain;
    private Camera m_camera;
    private PlayerController m_playerController;
    // building data
    private BuildingData m_currentBuildingData;
    private GameObject m_currentGhostBuilding;
    private BaseBuilding m_currentManipulatedBuilding;


    [Header("Builder Settings")]
    // Layer that is eligible to be built upon
    // Can expand on floor modules it´f we can build higher up, but this will take a lot of work
    [SerializeField] private LayerMask m_placementLayer;
    [SerializeField] private float m_placementRange = 50f;
    [SerializeField] private float m_maxHeightAdjustment = 5f;
    [SerializeField] private float m_rotateBuildingDegree = 1f;
    // valid placement color
    [SerializeField] private Color m_validColor = Color.green;
    // Invalid placement color
    [SerializeField] private Color m_invalidColor = Color.red;
    #endregion
    #region Booleans
    // To check if a buidling is "held" by the player
    private bool m_isManipulatingBuilding;
    public bool IsManipulatingBuilding => m_isManipulatingBuilding;

    // Is it placeable
    private bool m_isBuildingPlaceable;
    public bool IsBuildingPlaceable => m_isBuildingPlaceable;
    #endregion
    #region Important
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
        if (m_playerController == null) { m_playerController = FindAnyObjectByType<PlayerController>(); }
        if (m_cameraBrain == null) { m_cameraBrain = FindAnyObjectByType<CinemachineBrain>(); }
        if (m_camera == null) { m_camera = m_cameraBrain.GetComponent<Camera>(); }
        // Subscribe this scripts logic to the input actions in the playercontroller
        if (m_playerController != null)
        {
            m_playerController.OnPlaceAction += OnPlaceAction;
            m_playerController.OnRotateAction += OnRotateAction;
            m_playerController.OnManipulateAction += OnManipulateAction;
            m_playerController.OnScrapAction += OnScrapAction;
            m_playerController.OnBuilderModeDisabled += CancelCurrentBuildingAction;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (m_currentGhostBuilding != null)
        {
            CheckPlacementValidity();
        }
    }

    public void SelectBuildingAction(BuildingData buildingData)
    {
        //    CancelCurrentBuildingAction();
        m_currentBuildingData = buildingData;
        m_currentGhostBuilding = Instantiate(m_currentBuildingData.BuildingPrefab);

    }
    #endregion
    #region Placement Logic
    // Determine desired placement accroding to current context
    // Grid or free? free
    // Raycast to check for hit collision. This will determine the origin point of the object.
    // Raycast may be determined by a relative angle between a straight raycast forward and the angluar raycast from the character perspective
    // We can set certain ranges to angles. to check for the normals of the ground
    // It was easier than expected
    public void OnPlaceAction(InputAction.CallbackContext context)
    {
        if (m_currentBuildingData == null || m_currentBuildingData == null || !m_isBuildingPlaceable) return;
        if (context.performed)
        {
            if (m_isBuildingPlaceable)
            {
                GameObject go;
                // In case the player creates a nw building
                if (m_currentManipulatedBuilding != null)
                {
                    m_currentManipulatedBuilding.gameObject.GetComponent<Renderer>().enabled = true;
                    m_currentManipulatedBuilding.gameObject.GetComponent<Collider>().enabled = true;
                    m_currentManipulatedBuilding.transform.SetPositionAndRotation(m_currentGhostBuilding.transform.position, m_currentGhostBuilding.transform.rotation);
                    go = m_currentManipulatedBuilding.gameObject;
                }
                // In case the player manipulates a building
                else 
                {
                    go = Instantiate(
                        m_currentBuildingData.BuildingPrefab,
                        m_currentGhostBuilding.transform.position,
                        m_currentGhostBuilding.transform.rotation);

                }
                // Find the BuildingBox collider on the placed building and disable it
                Transform sizingBoxTransform = go.transform.Find("BuildingBox");
                if (sizingBoxTransform != null)
                {
                    if (sizingBoxTransform.TryGetComponent<Collider>(out var sizingCollider))
                    {
                        sizingCollider.enabled = false;
                    }
                }
                CancelCurrentBuildingAction();
            }

        }
    }
    private void CheckPlacementValidity()
    {
        Transform sizingBoxTransform = m_currentGhostBuilding.transform.Find("BuildingBox");
        if(sizingBoxTransform == null)
        {
            Debug.Log("This building needs an object called BuildingBox with a box collider to make sure this works");
            m_isBuildingPlaceable = false; 
            return;
        }
        // Get the BoxCollider of BuildingBox
        // This is to enable automatic generation of colliders of more complex buildings without disrupting the building process
        BoxCollider sizingCollider = sizingBoxTransform.GetComponent<BoxCollider>();
        // Check the position of the Raycasthit
        Ray ray = m_camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        // Moves the ghost to the detected hit position
        if (Physics.Raycast(ray, out RaycastHit hit, m_placementRange, m_placementLayer))
        {
            m_currentGhostBuilding.transform.position = hit.point;

            float currentHeightAdjustment = 0f;
            m_isBuildingPlaceable = false;
            // It's fine as long as the AdjustmentHeight is in range
            while (currentHeightAdjustment <= m_maxHeightAdjustment)
            {
                // BoxCast to check for collisions at the current position
                if (!Physics.CheckBox(
                        m_currentGhostBuilding.transform.position,
                        sizingCollider.size / 2,
                        m_currentGhostBuilding.transform.rotation,
                        ~0, 
                        QueryTriggerInteraction.Ignore))
                {
                    m_isBuildingPlaceable = true;
                    break;
                }
                // If a collision is found, need to move the ghost up.
                // BoxCast non-allocating to get the hit information.
                Collider[] colliders = Physics.OverlapBox(
                    m_currentGhostBuilding.transform.position,
                    sizingCollider.size / 2,
                    m_currentGhostBuilding.transform.rotation,
                    ~0,
                    QueryTriggerInteraction.Ignore); // This one, took hours
                if (colliders.Length > 0)
                {
                    // Find the highest point of collision and adjust accordingly
                    float highestCollisionPoint = -Mathf.Infinity;
                    // Move up as long as any collision is detected
                    foreach (var collider in colliders)
                    {
                        if (collider.bounds.max.y > highestCollisionPoint)
                        {
                            highestCollisionPoint = collider.bounds.max.y;
                        }
                    }
                    float requiredMoveUp = highestCollisionPoint - m_currentGhostBuilding.transform.position.y;
                    m_currentGhostBuilding.transform.position += Vector3.up * (requiredMoveUp + 0.01f); // Add a small offset
                    currentHeightAdjustment += requiredMoveUp;
                }
            }
        }
        else
        {
            m_isBuildingPlaceable = false;
        }
        // if true turn it green
        // if false turn it red
        SetGhostMaterial(m_currentGhostBuilding, m_isBuildingPlaceable ? m_validColor : m_invalidColor);
        
    }

    private void SetGhostMaterial(GameObject ghost, Color color)
    {
        Renderer[] renderers = ghost.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material tempMaterial = new(renderer.material)
            {
                color = new Color(color.r, color.g, color.b, 0.5f)
            };
            renderer.material = tempMaterial;
        }

    }
    #endregion
    #region Manipulation Logic
    public void OnManipulateAction(InputAction.CallbackContext context)
    {
        if (m_currentBuildingData != null) { return; }
        if (context.performed && context.interaction is TapInteraction)
        {
            // Raycasthit to a placed object. 
            Ray ray = m_camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, m_placementRange))
            {
                // Hit delivers object data
                BaseBuilding buildingToManipulate = hit.collider.GetComponentInParent<BaseBuilding>();
                // Dislodge the object from its placement
                if (buildingToManipulate != null)
                {
                    m_isManipulatingBuilding = true;
                    m_currentManipulatedBuilding = buildingToManipulate;
                    m_currentBuildingData = buildingToManipulate.BuildingData;
                    m_currentGhostBuilding = Instantiate(m_currentBuildingData.BuildingPrefab,
                        m_currentManipulatedBuilding.transform.position,
                        m_currentManipulatedBuilding.transform.rotation);
                    // Holds it
                    m_currentManipulatedBuilding.gameObject.GetComponent<Renderer>().enabled = false;
                    m_currentManipulatedBuilding.gameObject.GetComponent<Collider>().enabled = false;
                }
            }

        }
    }
    public void OnScrapAction(InputAction.CallbackContext context)
    {
        // Scrap it
        // Scrap only possible if the player holds the object.
        if (context.interaction is HoldInteraction && m_isManipulatingBuilding)
        {
            // Scrap
            // TODO:
            // Destroys the technically existing object and refunds the resources used to the player
            // Refund the resources
            // Destroy the object
            Destroy(m_currentManipulatedBuilding.gameObject);
            CancelCurrentBuildingAction();
        }
    }

    public void OnRotateAction(Vector2 rotateValue)
    {
        // Rotate
        // Rotate the building along y axis. Transfer input Vector 2 to clock or counterclockwise rotation
        if(m_currentGhostBuilding != null)
        {
            // Counterclockwise Rotation if rotateValue.x < 0
            // Clockwise Rotation if rotateValue.x > 0
            m_currentGhostBuilding.transform.Rotate(0, rotateValue.x * m_rotateBuildingDegree *Time.deltaTime, 0);
        }
        // According to the value rotate the object along the y-axis
    }
    #endregion
    public void CancelCurrentBuildingAction()
    {
        if (m_currentGhostBuilding != null)
        {
            Destroy(m_currentGhostBuilding);
        }
        if (m_currentManipulatedBuilding != null)
        {
            m_currentManipulatedBuilding.gameObject.GetComponent<Renderer>().enabled = true;
            m_currentManipulatedBuilding.gameObject.GetComponent<Collider>().enabled = true;
            m_currentManipulatedBuilding = null;
        }
        m_currentBuildingData = null;
    }

}
