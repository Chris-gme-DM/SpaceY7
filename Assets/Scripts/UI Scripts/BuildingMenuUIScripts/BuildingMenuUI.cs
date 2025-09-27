using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuUI : MonoBehaviour
{
    public static BuildingMenuUI Instance;
    #region Inspector
    [Header("Building Data List")]
    public List<BuildingData> AvailableBlueprints;
    private BuildingData m_selectedData;

    [Header("Blueprint List Panel")]
    [SerializeField] private Transform m_blueprintContainer;
    [SerializeField] private GameObject m_blueprintButtonPrefab;

    [Header("Blueprint Detail Panel")]
    [SerializeField] private TextMeshProUGUI m_buildingNameText;
    [SerializeField] private TextMeshProUGUI m_buildingDescriptionText;
    [SerializeField] private Transform m_buildingCostPanel;
    [SerializeField] private GameObject m_buildingCostSlotPrefab;
    [SerializeField] private Button m_buildButton;
    #endregion
    #region Setup
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
        m_buildButton.onClick.AddListener(OnBuildButtonClicked);
        PopulateBlueprintList();
    }


    private void PopulateBlueprintList()
    {
        foreach (BuildingData data in AvailableBlueprints)
        {
            GameObject buttonGO = Instantiate(m_blueprintButtonPrefab, m_blueprintContainer);
            buttonGO.GetComponent<BlueprintButton>().Setup(data, this);
        }
    }
    #endregion
    #region Important
    private void OnBuildButtonClicked()
    {
        // Pass the selected building data on to the BuildingManager for construction
        // Close the UI
        // Enter Builder Mode
    }
    #endregion
    #region BlueprintPanel
    public void DisplayBlueprint(BuildingData data)
    {
        m_selectedData = data;
        m_buildingNameText.text = data.BuildingName;
        m_buildingDescriptionText.text = data.BuildingDescription;
        // Populate the Costs
        PopulateBuildingCostSlots(data.BuildingCosts);
        // Check inventory and update the building button accordingly
        UpdateBuildButton(data.BuildingCosts);

    }

    private void PopulateBuildingCostSlots(List<Resources> costs)
    {
        // Clear previous Costs
        foreach (Transform child in m_buildingCostPanel)
        {
            Destroy(child);
        }

        bool resourcesAvailable = true;

        // Check jos inventory system thoroughly
        foreach (var i in costs)
        {
            
        }
    }
    private void UpdateBuildButton(List<Resources> costs)
    {
        throw new NotImplementedException();
    }
    #endregion
}
