using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using System;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject m_HUDPanel;
    // Compass
    [SerializeField] private Panel m_compassPanel;
    [SerializeField] private Image m_compassImage;
    // Gravity
    [SerializeField] private Panel m_gravimeterPanel;
    [SerializeField] private Image m_gravimeterImage;
    [SerializeField] private TextMeshProUGUI m_gravimeterText;
    [SerializeField] private float m_gravimeterRange = 300f;
    // Energy
    [SerializeField] private Panel m_energyMeterPanel;
    [SerializeField] private Image m_energyBarImage;
    [SerializeField] private TextMeshProUGUI m_energyText;
    // Oxygen
    [SerializeField] private Panel m_oxygenMeterPanel;
    [SerializeField] private Image m_oxygenBarImage;
    [SerializeField] private TextMeshProUGUI m_oxygenText;
    // Water
    [SerializeField] private Panel m_waterMeterPanel;
    [SerializeField] private Image m_waterBarImage;
    [SerializeField] private TextMeshProUGUI m_waterText;
    
    private PlayerStats m_playerStats;
    private GravityManager m_gravityManager;
    [SerializeField] private Transform m_playerCameraTransform;
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
        if (m_HUDPanel == null) m_HUDPanel = GameObject.FindGameObjectWithTag("HUDPanel");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        if (m_playerStats == null) m_playerStats = FindAnyObjectByType<PlayerStats>(); 
        if (m_gravityManager == null) m_gravityManager = FindAnyObjectByType<GravityManager>();
        if (m_playerCameraTransform == null) m_playerCameraTransform = Camera.main.transform;
        m_gravityManager = GravityManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGravimeterDisplay();
        UpdateCompassDisplay();
        UpdateEnergyDisplay(m_playerStats.PlayerEnergy);
        UpdateOxygenDisplay(m_playerStats.PlayerOxygen);
        UpdateWaterDisplay(m_playerStats.PlayerWater);
    }
    private void UpdateEnergyDisplay(float EnergyLevel)
    {
        m_energyBarImage.fillAmount = Mathf.Clamp01(EnergyLevel / 100f);
        m_energyText.text = $"{EnergyLevel:F0}%";
    }
    private void UpdateOxygenDisplay(float OxygenLevel) 
    {
        m_oxygenBarImage.fillAmount = Mathf.Clamp01(OxygenLevel / 100f);
        m_oxygenText.text = $"{OxygenLevel:F0}%";

    }
    private void UpdateWaterDisplay(float WaterLevel) 
    {
        m_waterBarImage.fillAmount = Mathf.Clamp01(WaterLevel / 100f);
        m_waterText.text = $"{WaterLevel:F0}%";
    }
    private void UpdateCompassDisplay()
    {
        float cameraY = m_playerCameraTransform.eulerAngles.y;
        m_compassImage.rectTransform.localRotation = Quaternion.Euler(0, 0, -cameraY);
    
    }

    private void UpdateGravimeterDisplay()
    {

        float targetY = Mathf.Lerp(
            -m_gravimeterRange / 2f,
            m_gravimeterRange / 2f,
            (1f -m_gravityManager.NormalizedGravityPosition)
        );
        // Determine new position
        Vector3 newPosition = m_gravimeterImage.rectTransform.localPosition;
        newPosition.y = targetY;
        m_gravimeterImage.rectTransform.localPosition = newPosition;
        m_gravimeterText.text = $"{-m_gravityManager.CurrentGravityDisplay:F2}G";

    }
}
