using Mono.Cecil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuUI : MonoBehaviour
{
    public static BuildingMenuUI Instance { get; private set; }
    #region Inspector
    [Header("Building Data List")]
    [SerializeField] private List<BuildingData> m_allBlueprints;
    private BuildingData m_selectedData;
    private Dictionary<BuildingData, BlueprintButton> m_blueprintButtonMap = new();

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
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
        m_buildButton.onClick.AddListener(OnBuildButtonClicked);
        PopulateBlueprintList();
    }


    private void PopulateBlueprintList()
    {
        foreach (BuildingData data in m_allBlueprints)
        {
            GameObject buttonGO = Instantiate(m_blueprintButtonPrefab, m_blueprintContainer);
            BlueprintButton buttonComponent = buttonGO.GetComponent<BlueprintButton>();
            buttonComponent.Setup(data, this);
            m_blueprintButtonMap.Add(data, buttonComponent);
            buttonGO.SetActive(data.IsUnlocked);
        }
    }
    #endregion
    #region Important
    private void OnBuildButtonClicked()
    {
        // Pass the selected building data on to the BuildingManager for construction
        UIManager.Instance.TogglePlayerMenu();
        if (m_selectedData == null || !m_buildButton.interactable) return;
        BuildingManager.Instance.SelectBuildingAction(m_selectedData);
        // Remove the resources out of hte inventory

        // Close the UI
        this.gameObject.SetActive(false);
        // Enter Builder Mode
        if (PlayerController.Instance != null)
        {
            Time.timeScale = 1f;
            if (HUDManager.Instance != null) HUDManager.Instance.gameObject.SetActive(true);
            PlayerController.Instance.SwitchActionMap("Builder");
        }
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
            Destroy(child.gameObject);
        }

        foreach (var cost in costs)
        {
            int requiredAmount = cost.amount;
            int currentCount = InventoryManager.Instance.GetTotalResourceCount(cost.resourceType);
            bool hasEnough = currentCount >= requiredAmount;

            GameObject slotGO = Instantiate(m_buildingCostSlotPrefab, m_buildingCostPanel);

            if (slotGO.TryGetComponent<BuildingCostSlot>(out var costSlotScript))
            {
                costSlotScript.Setup(cost, hasEnough);
            }
        }

    }
    private void UpdateBuildButton(List<Resources> costs)
    {
        bool resourcesAvailable = true;
        foreach (var cost in costs)
        {
            if (InventoryManager.Instance.GetTotalResourceCount(cost.resourceType) < cost.amount)
            {
                resourcesAvailable = false;
                break;
            }
        }
        m_buildButton.interactable = resourcesAvailable;
    }

    internal void CheckForNewBlueprints(int AmountNeuroChips)
    {
        const int CHIPS_PER_UNLOCK = 2;
        int blueprintsAvailable = AmountNeuroChips / CHIPS_PER_UNLOCK;
        // 2. Determine the maximum index we should check (prevents IndexOutOfRangeException)
        int unlockLimit = Mathf.Min(blueprintsAvailable, m_allBlueprints.Count);

        // 3. Iterate through the list up to the required unlock limit
        for (int i = 0; i < unlockLimit; i++)
        {
            BuildingData blueprint = m_allBlueprints[i];

            // If the blueprint is not already unlocked, unlock it now.
            if (!blueprint.IsUnlocked)
            {
                // Set the state in the data
                blueprint.IsUnlocked = true;

                // CRUCIAL: Call the specific UI function to enable the button/display the icon
                EnableBlueprintButton(blueprint);

                Debug.Log($"NEW BLUEPRINT UNLOCKED: {blueprint.BuildingName}");
            }
        }
    }

    private void EnableBlueprintButton(BuildingData blueprint)
    {
        if (m_blueprintButtonMap.TryGetValue(blueprint, out BlueprintButton button))
        {
            button.gameObject.SetActive(true);
        }
    }
    #endregion
}
