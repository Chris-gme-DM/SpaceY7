using UnityEngine;


/// <summary>
/// This script holds information and configuration of the playerObject to keep it collected and accesable to other scripts
/// </summary>
public class PlayerStats : MonoBehaviour
{
    #region References
    /// <summary>
    /// Holds the references to managers and modules, relevant to the player
    /// </summary>
    [SerializeField] private PlayerController playerController; // probably unnecessary
    [SerializeField] private PlayerStateMachine playerStateMachine; // probably not necessary
    
    #endregion
    #region Attributes
    /// <summary>
    /// Holds information about the player attributes and serve as configuration option for developer
    /// </summary>
    [Header("Survival")]
    [SerializeField] private float playerEnergy = 100f; // Represents power available to the suit of the player, used over time and for specific actions, e.g. Jetpack
    [SerializeField] private float playerOxygen = 100f; // the oxygen available to the player
    [SerializeField] private float playerWater = 100f; // hydrastion of the player

    [Header("Movement")]
    [SerializeField] private float playerMoveSpeed = 10f; // velocity clamp for the player while running on ground, airbourne movement is set with Forces
    [SerializeField] private float playerRunSpeed = 15f; // velocity of accelerated movement on ground
    [SerializeField] private float playerJumpForce = 5f; // Impulse Force of JumpAction
    [SerializeField] private float playerJetPower = 40f; // Force of Jetpack thrusters
    [SerializeField] private float playerStoppingPower = 50f; // Deceleration Force while player doesn't move
    #endregion
    #region Public Getters
    /// <summary>
    /// Getter functions for other scripts to access
    /// </summary>
    /// <returns> Energy, Oxygen, Water, MoveSpeed, RunSpeed, JetPower, StoppingPower</returns>
    public float GetPlayerEnergy() { return playerEnergy; }
    public float GetPlayerOxygen() { return playerOxygen; }
    public float GetPlayerWater() { return playerWater; }
    public float GetPlayerMoveSpeed() { return playerMoveSpeed; }
    public float GetPlayerRunSpeed() { return playerRunSpeed; }
    public float GetPlayerJumpForce() { return playerJumpForce; }
    public float GetPlayerJetPower() { return playerJetPower; }
    public float GetPlayerStoppingPower() { return playerStoppingPower; }

    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerController != null)
        {
            playerController = FindAnyObjectByType<PlayerController>();
            playerStateMachine = FindAnyObjectByType<PlayerStateMachine>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
