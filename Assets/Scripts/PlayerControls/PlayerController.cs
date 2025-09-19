using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

/// <summary>
/// LAST EDIT: Comments, (Chris), Focus Action removed, commented to lay it dormant
/// PLAN: Expand the movement methods to determine drainage on the player resources.
/// IMPORTANT: Switch ActionMaps MANUALLY! Keep that in mind when constructing other scripts
/// This script is handling Inputs by the player. 
/// It provides the developer with an interface to set Playerattributes and manipulate them later on.
/// It was redesigned after the recognition of a severe issue of the internal settings of the playerInput and I had to revert from using Interfaces
/// to manual Subscription of Actions.
/// The player can dynamically switch between the ActionMaps of the InputSystem and rely on the accustomed mapping for familiar or similar actions to ensure a smooth UX.
/// </summary>
public class PlayerController : MonoBehaviour, InputSystem_Actions.IExplorationActions, InputSystem_Actions.IBuilderActions
{ 
    #region References

    /// <summary>
    /// This section shows all the fields of relevant references to the PlayerController
    /// I made Public Getters for them in case i need them in other scripts. But they may be redundant in many cases.
    /// Don't forget to delete these, if they prove unnecessary
    /// </summary>
    public static PlayerController Instance { get; private set; }
    [Header("References")]  // I will automate the process to assign these references for the most part, but leave it here to let the developer see the assignments and adjust at will
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CinemachineBrain mainCameraBrain;
    [SerializeField] private CinemachineCamera m_mainCamera;
//    [SerializeField] private CinemachineCamera m_focusCamera;
    private InputSystem_Actions inputActions; // Ill do  it manually
    private LayerMask groundMask;
    #endregion
    #region Attributes
    [Header("Movement")]
    // Max Speed
    [Tooltip("A Maximum Speed that clamps all possible player movements, except falling of course")]
    [SerializeField] private float m_maxSpeed;
    public float PlayerMaxMoveSpeed => m_maxSpeed;
    // MovementSpeed
    [Tooltip("Normal Movement Speed in m/s")]
    [SerializeField] private float m_moveSpeed; 
    public float PlayerMoveSpeed => m_moveSpeed;
    // Running Speed
    [Tooltip("Running Speed in m/s")]
    [SerializeField] private float m_runSpeed; // velocity of accelerated movement on ground
    public float PlayerRunSpeed => m_runSpeed;
    [Tooltip("JumpForce, applied as Impulse. Be responsible")]
    // Jump Force
    [SerializeField] private float m_jumpForce; // Impulse Force of JumpAction
    public float PlayerJumpForce => m_jumpForce;
    // JetPack Force
    [Tooltip("Jetpack Force, applied by normal Force mode")]
    [SerializeField] private float m_jetPower; // Force of Jetpack thrusters, designed to keep the player afloat, at most float upwards a bit
    public float PlayerJetPower => m_jetPower;
    // Jetpack Force, applied when Sprint is active
    [Tooltip("Jetpack Force, designed to thrust the player upwards rapidly")]
    [SerializeField] private float m_jetBoost; // Force when JetPack is boosted, designed to thrust the player upwards rapidly
    public float JetBoost => m_jetBoost;

    [Header("DrainModifiers")]
    // Idle It needs to be rewarded to be lazy
    [Tooltip("Laziness needs reward")]
    [SerializeField] private float m_idleDrainModifier = 0.5f;
    // Sprinting
    [SerializeField] private float m_sprintEnergyModifier = 1.5f; // Numbers set in case i forget to adjust them
    [SerializeField] private float m_sprintOxygenModifier = 1.5f;
    [SerializeField] private float m_sprintWaterModifier = 1.5f;
    // Jetpack
    [SerializeField] private float m_jetpackEnergyModifier = 2.0f;
    [SerializeField] private float m_jetpackOxygenModifier = 2.0f;
    [SerializeField] private float m_jetpackWaterModifier = 2.0f;

    // Others
//    [SerializeField] private float m_focusedFov = 30f;
//    [SerializeField] private float m_focusSpeed = 5f;
    // move Input
    public Vector2 m_moveInput;
    public Vector2 MoveInput => m_moveInput;
    // move direction
    private Vector3 m_moveDirection;
    public Vector3 MoveDirection => m_moveDirection;
    
    #endregion
    #region Booleans
    // Idle
    private bool m_isIdle; // May only be interesting once we have animations for it, for now this is redundant
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
    // Focus
//    private bool m_isFocused;
    // Flashlight
    private bool m_flashLightActive;
    #endregion
    #region InputAction Subscription
    /// <summary>
    /// By implementing the Input Actions with Interfaces, that unity generates through the InputSystem settings, we ofload a lot of manual
    /// sub- and unsubscribing to input actions. The player can switch between the ActionMaps fluently by pressing or holding Q.
    /// 
    /// </summary>
    /// 

    public event Action<InputAction.CallbackContext> OnInteractAction;
    public event Action<InputAction.CallbackContext> OnPlaceAction;
    public event Action<InputAction.CallbackContext> OnRotateAction;
    public event Action<InputAction.CallbackContext> OnManipulateAction;
    public event Action<InputAction.CallbackContext> OnScrapAction;

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
        inputActions = new InputSystem_Actions();
        inputActions.Exploration.SetCallbacks(this);
        inputActions.Builder.SetCallbacks(this);
    }
    private void OnEnable()
    {
        inputActions.Exploration.Interact.performed += ctx => OnInteractAction?.Invoke(ctx);
        inputActions.Builder.Place.performed += ctx => OnPlaceAction?.Invoke(ctx);
        inputActions.Builder.Rotate.performed += ctx => OnRotateAction?.Invoke(ctx);
        inputActions.Builder.Manipulate.performed += ctx => OnManipulateAction?.Invoke(ctx);
        inputActions.Builder.Manipulate.performed += ctx => OnScrapAction?.Invoke(ctx);

        // Enable the exploration Action map as default
        inputActions?.Exploration.Enable();
    }
    private void OnDisable()
    {
        inputActions.Exploration.Interact.performed -= ctx => OnInteractAction?.Invoke(ctx);
        inputActions.Builder.Place.performed -= ctx => OnPlaceAction?.Invoke(ctx);
        inputActions.Builder.Rotate.performed -= ctx => OnRotateAction?.Invoke(ctx);
        inputActions.Builder.Manipulate.performed -= ctx => OnManipulateAction?.Invoke(ctx);
        inputActions.Builder.Manipulate.performed -= ctx => OnScrapAction?.Invoke(ctx);

        // Disable both when switching to UI
        inputActions?.Exploration.Disable();
        inputActions?.Builder.Disable();
    }
    #endregion
    #region Important
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the relevant references and assign them automatically
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        if (playerStats == null) playerStats = GetComponent<PlayerStats>();
        if (mainCameraBrain == null) mainCameraBrain = FindAnyObjectByType<CinemachineBrain>();
//       if (m_focusCamera != null)
//       {
//           m_focusCamera.gameObject.SetActive(false);
//       }

        groundMask = LayerMask.GetMask("Ground");
    }
    // Update is called once per frame
    void Update()
    {
        // Update moveDirection
        m_moveDirection = mainCameraBrain.transform.right * m_moveInput.x + mainCameraBrain.transform.forward * m_moveInput.y;
        m_moveDirection = Vector3.ProjectOnPlane(m_moveDirection, Vector3.up).normalized;
//        if (m_mainCamera != null)
//        {
//            float targetFov = m_isFocused ? m_focusedFov : 61f; // 61f is the default FOV
//            m_mainCamera.Lens.FieldOfView = Mathf.Lerp(m_mainCamera.Lens.FieldOfView, targetFov, Time.deltaTime * m_focusSpeed);
//        }

    }
    // Handling physics related actions
    void FixedUpdate()
    {
        // State Checks to determine available options and set modifiers to the Resource Drain accordingly
        IdleCheck();
        CheckGround();
        EvaluateDrainModifiers();
        // Let the Physics engine handle
        // Determine which velocity to apply
        float currentMoveSpeed = m_isSprinting ? m_runSpeed : m_moveSpeed; // If sets the moveSpeed to apply to the current MoveSpeed
        Vector3 horizontalVelocity = m_moveDirection * currentMoveSpeed; // Sets the velocity, although i am not happy
        // Add Jetpack if active
        if (m_jetPackActive)
        {
            // Increase drain for jetpack.
            // Add Jetpack Force
            float currentJetPower = m_jetPower;
            if (m_isSprinting)
            {
                currentJetPower += m_jetBoost;
            }
            rb.AddForce(Vector3.up * currentJetPower, ForceMode.Force);
        }
        Vector3 newVelocity = new(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        rb.linearVelocity = newVelocity;
    }
    #endregion
    #region Helper Methods
    // We will need the check state method only if we fail to visual script the state machine of animations
    // Maybe a Check State can help to determine which modifiers to apply to the drainage of the Player resources
    // or can be done in the update functions.
    
   private bool IdleCheck() //Stops the player motions once the threshhold is reached
   {
       if(rb.linearVelocity.magnitude <= 0.01f)
       {
           m_isIdle = true;
           rb.linearVelocity = Vector3.zero;
       }
       else
       {
           m_isIdle= false;
       }
       return m_isIdle;
   }
    private void EvaluateDrainModifiers()
    {
        playerStats.ResetDrainModifiers();

        float _energyDrainModifier = 1f;
        float _oxygenDrainModifier = 1f;
        float _waterDrainModifier = 1f;

        // Apply modifiers based on current state
        // Idle
        if (m_isIdle) // Halfing the drain seems fair enough to give the player rest
        {
            _energyDrainModifier *= m_idleDrainModifier;
            _oxygenDrainModifier *= m_idleDrainModifier;
            _waterDrainModifier *= m_idleDrainModifier;
        }
        // Sprinting
        if (m_isSprinting)
        {
            _energyDrainModifier *= m_sprintEnergyModifier;
            _oxygenDrainModifier *= m_sprintOxygenModifier;
            _waterDrainModifier *= m_sprintWaterModifier;
        }
        // Jetpack
        if (m_jetPackActive)
        {
            _energyDrainModifier *= m_jetpackEnergyModifier;
            _oxygenDrainModifier *= m_jetpackOxygenModifier;
            _waterDrainModifier *= m_jetpackWaterModifier;
        }
        // Since boosting with the Jetpack results in a much higher Drain, using the Jetpack becomes a relevant decision of the player
        // Set calculated modifiers on the PlayerStats script
        playerStats.SetEnergyDrainModifier(_energyDrainModifier);
        playerStats.SetOxygenDrainModifier(_oxygenDrainModifier);
        playerStats.SetWaterDrainModifier(_waterDrainModifier);

    }
    private bool CheckGround()
    {
        // Raycast down to check if the Player is ground
        // if the raycast hits the ground the player is grounded.
        // else the player is airbourne
        float offset = GetComponent<Collider>().bounds.extents.y;
        Vector3 checkPosition = new (rb.position.x, rb.position.y - offset, rb.position.z);
        m_isGrounded = Physics.CheckSphere(checkPosition, 0.1f, groundMask);
        return m_isGrounded;
    }

    #endregion
    #region ActionMapSwitch
    /// <summary>
    /// These methods enable/disable the currently active ActionMap of the PlayerInput.
    /// The SwitchCurrentActionMap method had a conflict i couldnt resolve, so i control the switch manually
    /// </summary>
    /// <param name="context"></param>

    //Enter Builder Mode
    public void OnBuilderMode(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            playerInput.SwitchCurrentActionMap("Builder");
//           inputActions.Exploration.Disable();
//           inputActions.Builder.Enable();
            Debug.Log("Build that stuff");
        }
    }

    //Enter Exploration Mode
    public event Action OnBuilderModeDisabled;
    public void OnExplorationMode(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnBuilderModeDisabled?.Invoke();
            playerInput.SwitchCurrentActionMap("Exploartion");
 //           inputActions.Builder.Disable();
 //           inputActions.Exploration.Enable();
            Debug.Log("Explore that stuff");
        }
    }

    #endregion
    #region Locomotive Inputs
    /// <summary>
    /// Every input related to be handled by a Locomotive StateMachine. These are related to movement.
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="System.NotImplementedException"></exception>


    public void OnMove(InputAction.CallbackContext context)
    {
        m_moveInput = context.ReadValue<Vector2>();
        Debug.Log("Move Input: " + m_moveInput);
    }

    public void OnJetpack(InputAction.CallbackContext context)
    {
        if(context.performed && context.interaction is TapInteraction && m_isGrounded)
        {
            rb.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            Debug.Log("$JUMP");
        } 
        else if(context.performed && context.interaction is HoldInteraction)
        {
            // Since we would like to give full control to the player we set the froce of the jetpack reasonable enough to keep them afloat while its active only
            m_jetPackActive = !m_jetPackActive;
            Debug.Log("WE are flying, woohoo");

        }
    }
    // Depending on input it makes the player sprint or applies force to the thrusters of the jetpack
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            m_isSprinting = !m_isSprinting;
            Debug.Log("Gotta go fast");
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
    // Toggle the Flashlight of the helmet
    public void OnFlashlight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            m_flashLightActive = !m_flashLightActive;
            if (m_flashLightActive)
            {
                // Flash Light
                Debug.Log("Blinded by the light");
                return;
            }
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
    #region Exploration Actions
    // Interacts with various objects in the world if the InteractionManager recognizes an interactable object
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Interact with the stuff");
            OnInteractAction?.Invoke(context);
        }
    }
    // A function i would like to add, but is not important atm, The idea is to have it act like a scope for the player
//    public void OnFocus(InputAction.CallbackContext context)
//    {
//        if (context.performed)
//        {
//            Debug.Log("Focus");
//            // Toggle the focus camera on/off
//            m_isFocused = !m_isFocused;
//        }
//    }

    #endregion
    #region Builder Actions

    // Dislodges an existing object, placed by the player and enables anewed manipulation to its placement or allows to scrap it
    public void OnManipulate(InputAction.CallbackContext context)
    {
        // Need to figure out what to do with it.
        // The Buidling Manager should probably find the object
        if(context.performed && context.interaction is TapInteraction)
        {
        Debug.Log("Replacement method of buildings, initiated");
            OnManipulateAction.Invoke(context);
        }
        // Scrap
        if (context.performed && context.interaction is HoldInteraction)
        {
            Debug.Log("Scrapt it");
            OnScrapAction.Invoke(context);
        }
    }

    public void OnPlace(InputAction.CallbackContext context)
    {
        Debug.Log("Place it down");
        OnPlaceAction.Invoke(context);
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        // Read the value
        // negative for counterclockwise Rotation
        // positive for clockwise Rotation
        Vector2 rotateValue = context.ReadValue<Vector2>();
        if (context.performed)
        {
            Debug.Log("Rotate around");
            OnRotateAction.Invoke(context);
        }
    }

    #endregion
    #endregion
}