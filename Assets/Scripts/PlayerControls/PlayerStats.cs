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
    
    #endregion
    #region Attributes
    /// <summary>
    /// Holds information about the player attributes and serve as configuration option for developer
    /// all fields have their public getters assigned
    /// </summary>
    [Header("Survival")]
    [SerializeField] private float m_playerEnergy = 100f; // Represents power available to the suit of the player, used over time and for specific actions, e.g. Jetpack
    public float PlayerEnergy => m_playerEnergy;
    [SerializeField] private float m_playerOxygen = 100f; // the oxygen available to the player
    public float PlayerOxygen => m_playerOxygen;
    [SerializeField] private float m_playerWater = 100f; // hydrastion of the player
    public float PlayerWater => m_playerWater;

    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerController != null)
        {
            playerController = FindAnyObjectByType<PlayerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
