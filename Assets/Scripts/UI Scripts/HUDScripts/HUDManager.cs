using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using System;

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
    private Transform m_playerCameraTransform;
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
        if (m_playerStats == null) { m_playerStats = FindAnyObjectByType<PlayerStats>(); }
        if (m_gravityManager == null) { m_gravityManager = FindAnyObjectByType<GravityManager>(); }
        if (m_playerCameraTransform == null) { m_playerCameraTransform = Camera.main.transform; }
        if (m_HUDCanvas == null)
        {
            GameObject canvasObject = GameObject.FindWithTag("HUDCanvas");
            
            if (canvasObject.TryGetComponent<Canvas>(out m_HUDCanvas))
            {
                // Compass
                Transform compassTransform = m_HUDCanvas.transform.Find("CompassPanel/CompassImage");
                m_compassImage = compassTransform.GetComponent<Image>();
                // Gravimeter
                Transform graviMeterTransform = m_HUDCanvas.transform.Find("GravitationPanel/GravitationBallImage");
                m_gravimeterImage = graviMeterTransform.GetComponent<Image>();
                m_gravimeterText = graviMeterTransform.GetComponentInChildren<TextMeshProUGUI>();
                // EnergyBar
                Transform energyBarTransform = m_HUDCanvas.transform.Find("EnergyPanel/EnergyBarImage");
                m_energyBarImage = energyBarTransform.GetComponent<Image>();
                m_energyText = energyBarTransform.GetComponentInChildren<TextMeshProUGUI>();
                // OxygenBar
                Transform oxygenBarTransform = m_HUDCanvas.transform.Find("OxygenPanel/OxygenBarImage");
                m_oxygenBarImage = oxygenBarTransform.GetComponent<Image>();
                m_oxygenText = oxygenBarTransform.GetComponentInChildren<TextMeshProUGUI>();
                // WaterBar
                Transform waterBarTransform = m_HUDCanvas.transform.Find("WaterPanel/WaterBarImage");
                m_waterBarImage = waterBarTransform.GetComponent<Image>();
                m_waterText = waterBarTransform.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
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
