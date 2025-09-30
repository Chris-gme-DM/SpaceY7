using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script holds information and configuration of the playerObject to keep it collected and accesable to other scripts
/// The attributes that reperesent the player health overall nee to be public to be accessed by the HUD
/// LAST UPDATE: Comments
/// PLAN: Expand once, HUD and stuff should be called to update the display or warn the player
/// </summary>
public class PlayerStats : MonoBehaviour
{
    #region References
    public static PlayerStats Instance {  get; private set; }
    /// <summary>
    /// Holds the references to managers and modules, relevant to the player
    /// </summary>
    [SerializeField] private PlayerController playerController; // probably unnecessary
    public Transform RespawnPoint;
    private int m_amountNeuroChips;
    public int AmountNeuroChips => m_amountNeuroChips;
    #endregion
    #region Attributes
    /// <summary>
    /// Holds information about the player attributes and serve as configuration option for developer
    /// all fields have their public getters assigned
    /// </summary>
    [Header("Survival")]
    // Introduced Maxima to clamp the values.
    [Tooltip("Max Energy. Starting value.")]
    [SerializeField] private float m_maxEnergy = 100f;
    [Tooltip("Max Oxygen. Starting value.")]
    [SerializeField] private float m_maxOxygen = 100f;
    [Tooltip("Max Water. Starting value.")]
    [SerializeField] private float m_maxWater = 100f;
    [Tooltip("Max Humantiy. starting value")]
    [SerializeField] private float m_maxHumanity = 100f;
    // Maybe we can introduce upgrades to the Suit, once everything else is established
    
    [SerializeField] private float m_playerEnergy; // Represents power available to the suit of the player, used over time and for specific actions, e.g. Jetpack
    public float PlayerEnergy => m_playerEnergy;
    [SerializeField] private float m_playerOxygen; // the oxygen available to the player
    public float PlayerOxygen => m_playerOxygen;
    [SerializeField] private float m_playerWater; // hydration of the player
    public float PlayerWater => m_playerWater;
    [SerializeField] private float m_playerHumanity; // very abstract resource that we still need to figure out how to truly represent
    public float PlayerHumanity => m_playerHumanity; // Since the player is the only human i thought i can leave Player away. Until we make it multiplayer

    [Header("Base Resource Drain")]
    [SerializeField] private float m_playerEnergyDrain = 0.02f; // Set relatively low for the start
    [SerializeField] private float m_playerOxygenDrain = 0.02f;
    [SerializeField] private float m_playerWaterDrain = 0.02f;
//    [SerializeField] private float m_playerHumanityDrain; // We still need to figure that one out
    // There are some ideas like hazardous zones or state of resources that afect this drain.
    // to replensih the can build stuff but we do not have enought stuff
    // to replenish we can use the resources as to say the player is healthy and that helps her keep her humanity and sanity up

    private float m_playerEnergyDrainModifier;
    private float m_playerOxygenDrainModifier;
    private float m_playerWaterDrainModifier;
 //   private float m_playerHumanityDrainModifier; // Currently dormant

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
        if (playerController != null)
        {
            playerController = FindAnyObjectByType<PlayerController>();
        }
        RespawnPoint = this.gameObject.transform;
        m_amountNeuroChips = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Apply passive drain with modifiers.
        m_playerEnergy -= (m_playerEnergyDrain * m_playerEnergyDrainModifier) * Time.deltaTime;
        m_playerOxygen -= (m_playerOxygenDrain * m_playerOxygenDrainModifier) * Time.deltaTime;
        m_playerWater -= (m_playerWaterDrain * m_playerWaterDrainModifier) * Time.deltaTime;
        if ( m_playerOxygen <= 0.1f || m_playerWater <= 0.1f || m_playerEnergy <= 0.1f )
        { OnDeath(); }
    }
    #endregion
    public void ChangeVitals(List<Resources> vitals)
    {
        foreach (var vital in vitals)
        {
            switch (vital.resourceType)
            {
                case ResourceType.Energy:
                    ChangeEnergy(vital.amount);
                    break;
                case ResourceType.Oxygen:
                    ChangeOxygen(vital.amount);
                    break;
                case ResourceType.Water:
                    ChangeWater(vital.amount);
                    break;
            }
        }
    }
    // Methods need to manage the resources of the player
    // Every resource has its own method and ways to manipulate them
    // these methods can be accessed by other scripts
    // each Update resources diminishes
    // They are fed modifiers by the environment for everything that uses resources, almost every player action requires a certain amount or modifies it
    // We need to depend the usage upon time. and stop it whenevever no time is passing.
    // I need to think.
    // Each script should call upon respective warning methods once threshholds are met
    ///<summary>
    ///The following regions contain methods that can be called by other objects, for example items that replenish resources
    ///Modifiers can be set to positive values to increase the drain, or negative to decrease it.
    ///Or we expand the methods, maybe.
    /// </summary>
    #region Modifiers
    public void SetEnergyDrainModifier(float modifier)
    {
        m_playerEnergyDrainModifier = modifier;
    }

    public void SetOxygenDrainModifier(float modifier)
    {
        m_playerOxygenDrainModifier = modifier;
    }

    public void SetWaterDrainModifier(float modifier)
    {
        m_playerWaterDrainModifier = modifier;
    }
//    public void SetHumanityDrainModifier(float modifier) // Depression may be a terrifying modifier, if the player doesnt do stuff to combat it
//    { 
//        m_playerHumanityDrainModifier = modifier;
//    }
    // A reset method to reset all the modifiers at once instead of making it complicated
    public void ResetDrainModifiers()
    {
        m_playerEnergyDrainModifier = 1.0f;
        m_playerOxygenDrainModifier = 1.0f;
        m_playerWaterDrainModifier = 1.0f;
//        m_playerHumanityDrainModifier = 1.0f;
    }
    #endregion
    #region Chunk changes
    /// <summary>
    /// Add or subtract a chunk of the resource. For example picking up certain resources takes a lot of effort or doing something else is increasing by a lot
    /// </summary>
    /// <param name="amount"></param>
    private void ChangeEnergy(int amount)
    {
        m_playerEnergy += amount;
        m_playerEnergy = Mathf.Clamp(m_playerEnergy, 0f, m_maxEnergy);

    }
    private void ChangeOxygen(int amount)
    {
        m_playerOxygen += amount;
        m_playerOxygen = Mathf.Clamp(m_playerOxygen, 0f, m_maxOxygen);

    }
    private void ChangeWater(int amount)
    {
        m_playerWater += amount;
        m_playerWater = Mathf.Clamp(m_playerWater, 0f, m_maxWater);

    }
//    private void ChangeHumanity(int amount) // I wish
//    {
//        m_playerHumanity += amount;
//        m_playerHumanity = Mathf.Clamp(m_playerHumanity, 0f, m_maxHumanity);
//    }
    public void CollectNeuroChip()
    {
        m_amountNeuroChips++;

        // Check if the BuildingManager instance is available before calling
        if (BuildingMenuUI.Instance != null)
        {
            BuildingMenuUI.Instance.CheckForNewBlueprints(AmountNeuroChips);
        }
    }
    #endregion
    #region Respawn
    public void OnDeath()
    {
        PlayerController.Instance.SwitchActionMap("UI");
        BuildingManager.Instance.CancelCurrentBuildingAction();
        UIManager.Instance.FadeToBlack(2.0f);
        StartCoroutine(RespawnTimer(3f));
    }
    private IEnumerator RespawnTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnRespawn();
    }
    public void OnRespawn()
    {
        // As soon as one of the vital stats reaches zero
        // Make the player lose all items in the inventory
        m_playerEnergy = m_maxEnergy/3;
        m_playerOxygen = m_maxOxygen/3;
        m_playerWater = m_maxWater/3;
        // Open the Menu to decide to respawn
        // Respawn locations are the bed in the Hub, if placed or the starting point if they failed to do so
        if (RespawnPoint != null)
        {
            transform.position = RespawnPoint.position;
            // Reset velocity to prevent ghost movement after teleport
            if (TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // How do we handle Saving and Loading? Maybe we can offer to use blueprint chips to save a progress at any time. and offer to Save and Exit to avoid frustration.
        // The Quicksave mechanic works the same way basically. Using a Neurochip
        // ReEnable Controls
        PlayerController.Instance.SwitchActionMap("Exploration");

    }
    #endregion
}