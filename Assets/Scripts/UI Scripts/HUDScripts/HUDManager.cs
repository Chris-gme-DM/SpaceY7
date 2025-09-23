using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("Panels")]
    [SerializeField] private Canvas m_HUDCanvas;
    // Compass
    [SerializeField] private Panel m_compassPanel;
    [SerializeField] private Image m_compassImage;
    // Gravity
    [SerializeField] private Panel m_gravimeterPanel;
    [SerializeField] private Image m_gravimeterImage;
    // Energy
    [SerializeField] private Panel m_energyMeterPanel;
    [SerializeField] private Image m_energyBarImage;
    // Oxygen
    [SerializeField] private Panel m_oxygenMeterPanel;
    [SerializeField] private Image m_oxygenBarImage;
    // Water
    [SerializeField] private Panel m_waterMeterPanel;
    [SerializeField] private Image m_waterBarImage;
    
    private PlayerStats m_playerStats;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(m_playerStats == null) { m_playerStats = FindAnyObjectByType<PlayerStats>(); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
