using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This script is handling Inputs by the player. Inputs are send into the StateMachine that checks the environment of the playerObject
/// and controls in which state exactly the player object is.
/// It provides the developer with an interface to set Playerattributes and manipulate them later on.
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Inspector

    /// <summary>
    /// This section shows all the fields of relevant references to the PlayerController
    /// </summary>
    [Header("References")]  // I will automate the process to assign these references for the most part, but leave it here to let the developer see the assignments and adjust at will
    [SerializeField] private PlayerStateMachine playerStateMachine; // the statemachine that controls the state of the player
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody rb;

    #endregion
    #region Booleans
    private bool jumpPressed;
    #endregion
    #region PublicGetters
    /// <summary>
    /// All the Public Getter functions to enable private fields that are set in this script only but need references in other scripts
    /// </summary>
    public bool JumpPressed { get { return jumpPressed; } }
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Finde the relevant references and assign them
        if(rb == null) rb = GetComponent<Rigidbody>();
        if(playerInput == null) playerInput = GetComponent<PlayerInput>();
        if(playerStateMachine == null) playerStateMachine = GetComponent<PlayerStateMachine>();

        //Subscribe Actions to Input System
        playerInput.actions["Move"].performed += OnMove();
        playerInput.actions["Move"].canceled -= OnMove();
        playerInput.actions["Jump"].performed += OnJump();
        playerInput.actions["Interact"].performed += OnInteract();
        playerInput.actions[""]
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
