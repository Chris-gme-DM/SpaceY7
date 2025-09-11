using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
/// <summary>
/// This script is handling Inputs by the player. Inputs are send into the StateMachine that checks the environment of the playerObject
/// and controls in which state exactly the player object is.
/// It provides the developer with an interface to set Playerattributes and manipulate them later on.
/// The player can dynamically switch between the ActionMaps of the InputSystem and rely on the accustomed mapping for familiar or similar actions to ensure a smooth UX.
/// </summary>
public class PlayerController : MonoBehaviour
{ 
    #region References

    /// <summary>
    /// This section shows all the fields of relevant references to the PlayerController
    /// </summary>
    [Header("References")]  // I will automate the process to assign these references for the most part, but leave it here to let the developer see the assignments and adjust at will
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CinemachineBrain mainCamera;

    private InputSystem_Actions inputActions; // Ill do  it manually

    [Header("Movement")]
    [SerializeField] private float m_moveSpeed; // velocity clamp for the player while running on ground, airbourne movement is set with Forces
    public float PlayerMoveSpeed => m_moveSpeed;
    [SerializeField] private float m_runSpeed; // velocity of accelerated movement on ground
    public float PlayerRunSpeed => m_runSpeed;
    [SerializeField] private float m_jumpForce; // Impulse Force of JumpAction
    public float PlayerJumpForce => m_jumpForce;
    [SerializeField] private float m_jetPower; // Force of Jetpack thrusters
    public float PlayerJetPower => m_jetPower;

    // move Input
    [SerializeField] public Vector2 m_moveInput;
    public Vector2 MoveInput => m_moveInput;
    // move direction
    private Vector3 m_moveDirection;
    public Vector3 MoveDirection => m_moveDirection;

    private float m_linearVelocity;

    #endregion
    #region Booleans
    // Idle
    private bool m_isIdle;
    public bool IsIdle => m_isIdle;
    // Grounded
    private bool m_isGrounded;
    public bool IsGrounded => m_isGrounded;
    // Sprinting
    private bool m_isSprinting;
    public bool IsSprinting => m_isSprinting;
    // JetPack
    private bool m_jetPackActive;
    public bool JetPackActive => m_jetPackActive;
    // Flashlight
    private bool m_flashLightActive;
    #endregion
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Exploration.Enable();
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the relevant references and assign them automatically
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        if (playerStats == null) playerStats = GetComponent<PlayerStats>();
        if (mainCamera == null) mainCamera = FindAnyObjectByType<CinemachineBrain>();

    }
    private void OnEnable()
    {
        // Manually enable the action maps and subscribe to their events because unity had unresolvable issues with the implemantation of the interfaces.
        inputActions.Exploration.Enable();
        inputActions.Builder.Enable();

        // Locomotion
        inputActions.Exploration.Move.performed += onMove;
        inputActions.Exploration.Move.canceled += onMove;
        inputActions.Exploration.Jetpack.performed += OnJetpack;

        // ActionMapSwitch
        inputActions.Exploration.BuilderMode.performed += OnBuilderMode;
        inputActions.Builder.ExplorationMode.performed += OnExplorationMode;

        // Common Actions
        inputActions.Exploration.Sprint.performed += OnSprint;
        inputActions.Exploration.Flashlight.performed += OnFlashlight;
        inputActions.Exploration.Rover.performed += OnRover;
        inputActions.Exploration.Next.performed += OnNext;
        inputActions.Exploration.Previous.performed += OnPrevious;

        // Exploration Actions
        inputActions.Exploration.Interact.performed += OnInteract;
        inputActions.Exploration.Focus.performed += OnFocus;

        // Builder Actions
        inputActions.Builder.Manipulate.performed += OnManipulate;
        inputActions.Builder.Place.performed += OnPlace;
        inputActions.Builder.Rotate.performed += OnRotate;
    }

    private void OnDisable()
    {
        // Manually unsubscribe from all events
        inputActions.Exploration.Move.performed -= onMove;
        inputActions.Exploration.Move.canceled -= onMove;
        inputActions.Exploration.Jetpack.performed -= OnJetpack;

        // ActionMapSwitch
        inputActions.Exploration.BuilderMode.performed -= OnBuilderMode;
        inputActions.Builder.ExplorationMode.performed -= OnExplorationMode;

        // Common Actions
        inputActions.Exploration.Sprint.performed -= OnSprint;
        inputActions.Exploration.Flashlight.performed -= OnFlashlight;
        inputActions.Exploration.Rover.performed -= OnRover;
        inputActions.Exploration.Next.performed -= OnNext;
        inputActions.Exploration.Previous.performed -= OnPrevious;

        // Exploration Actions
        inputActions.Exploration.Interact.performed -= OnInteract;
        inputActions.Exploration.Focus.performed -= OnFocus;

        // Builder Actions
        inputActions.Builder.Manipulate.performed -= OnManipulate;
        inputActions.Builder.Place.performed -= OnPlace;
        inputActions.Builder.Rotate.performed -= OnRotate;

        inputActions.Exploration.Disable();
        inputActions.Builder.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        // Update moveDirection
        m_moveDirection = mainCamera.transform.right * m_moveInput.x + mainCamera.transform.forward * m_moveInput.y;
        m_moveDirection = Vector3.ProjectOnPlane(m_moveDirection, Vector3.up).normalized;
        CheckState();
    }
    // Handling physics related actions
    void FixedUpdate()
    {
        // Let the Physics engine handle shit
        // Determine which velocity to apply
        if (m_isSprinting) 
        {
            m_linearVelocity = m_runSpeed;
        }
        else 
        {
            m_linearVelocity = m_moveSpeed;
        }
        if (m_jetPackActive)
        {
            rb.AddForce(Vector3.up * m_jetPower, ForceMode.Force); // With sensible settings of the forces this should feel almost like the player is hovering on the place
        }
        if (m_isSprinting && m_jetPackActive)
        {
            rb.AddForce(Vector3.up * m_jetPower, ForceMode.Force);
        }
            Vector3 horizontalVelocity = m_moveDirection * m_linearVelocity;
            Vector3 newVelocity = new(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            rb.linearVelocity = newVelocity;
    }
    #region Helper Functions
    void CheckState()
    {
        if(rb.linearVelocity == Vector3.zero)
        { m_isIdle =true; }
        if(rb.linearVelocity != Vector3.zero)
        { m_isIdle = false; }
    }
    void CheckGround()
    {
        // Raycast down to check if the Player is ground
        // if the raycast hits the ground the player is grounded.
        // else the player is airbourne
    }

    #endregion
    #region ActionMapSwitch
    /// <summary>
    /// These methods enable/disable the currently active ActionMap of the PlayerInput
    /// </summary>
    /// <param name="context"></param>

    //Enter Builder Mode
    public void OnBuilderMode(InputAction.CallbackContext context)
    {
        playerInput.SwitchCurrentActionMap("Builder");
        Debug.Log("Build that stuff");
    }

    //Enter Exploration Mode
    public void OnExplorationMode(InputAction.CallbackContext context)
    {
        playerInput.SwitchCurrentActionMap("Exploration");
        Debug.Log("Explore that stuff");

    }

    #endregion
    #region Locomotive Inputs
    /// <summary>
    /// Every input related to be handled by a Locomotive StateMachine. These are related to movement.
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="System.NotImplementedException"></exception>


    public void onMove(InputAction.CallbackContext context)
    {
        m_moveInput = context.ReadValue<Vector2>();
        Debug.Log("Move Input: " + m_moveInput);
    }

    public void OnJetpack(InputAction.CallbackContext context)
    {
        if(context.interaction is TapInteraction)
        {
            rb.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            Debug.Log("$JUMP");
        } 
        else if(context.interaction is HoldInteraction)
        {
            // Since we would like to give full control to the player we set the froce of the jetpack reasonable enough to keep them afloat while its active only
            m_jetPackActive = !m_jetPackActive;
            Debug.Log("WE are flying, woohoo");

        }
    }

    #endregion
    #region Action Inputs
    /// <summary>
    /// The Action Inputs are seperated into Common Inputs, Exploration Inputs and Builder Inputs.
    /// </summary>
    /// <param name="context"></param>

    #region Common
    public void OnRover(InputAction.CallbackContext context)
    {
    // In principle this should call upon the Rover Menu, if it is in range.
    // If not in range, call the rover over until the distance is acceptable. then open the menu
    Debug.Log("Come over here");
    }
    // Depending on input it makes the player sprint or applies force to the thrusters
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            m_isSprinting = !m_isSprinting;
            Debug.Log("Gotta go fast");
        } else if (context.interaction is HoldInteraction)
        {
            // Jetpack active should keep the player afloat while this will boost that prospect
            rb.AddForce(Vector3.up * m_jetPower, ForceMode.Force);
            Debug.Log("Higher in the sky");
        }
    }
    // Toggle the Flashlight of the helmet
    public void OnFlashlight(InputAction.CallbackContext context)
    {
        m_flashLightActive = !m_flashLightActive;
        if(m_flashLightActive) 
        {
            // Flash Light
            Debug.Log("Blinded by the light");
            return;
        }

    }

    // switches through options of the Multitool
    public void OnPrevious(InputAction.CallbackContext context)
    {
        Debug.Log("Shift to the previous tool");
    }

    // switches through options of the Multitool
    public void OnNext(InputAction.CallbackContext context)
    {
        Debug.Log("Shift to the next tool");
    }
    #endregion
    #region Exploration

    // Interacts with various objects in the world if the InteractionManager recognizes an interactable object
    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact with the stuff");
    }
    // A function i would like to add, but is not important atm, The idea is to have it act like a scope for the player
    public void OnFocus(InputAction.CallbackContext context)
    {
        Debug.Log("Focus");
    }

    #endregion
    #region Builder

    // Dislodges an existing object, placed by the player and enables anewed manipulation to its placement or allows to scrap it
    public void OnManipulate(InputAction.CallbackContext context)
    {
        //Need to figure out what to do with it.
        //The Buidling Manager should probably find the object
        Debug.Log("Replacement method of buildings, initiated");
    }

    public void OnPlace(InputAction.CallbackContext context)
    {
        Debug.Log("Place it down");
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        Debug.Log("Rotate around");
    }

    #endregion
    #endregion
}
