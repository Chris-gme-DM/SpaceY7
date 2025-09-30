using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.Interactions;
using System.Collections;
using UnityEngine.Assertions.Must;

/// <summary>
/// This script turns the UIManager into the single source of truth, regarding UI Elements. It's a glorified LightSwitch Operator
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private PlayerController m_playerController;

    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_roverInventoryPanel;
    [SerializeField] private DynamicInventoryDisplay m_dynamicInventoryDisplay;
    [SerializeField] private CanvasGroup m_fader;

    private bool m_isRoverInventoryOpen = false;
    private bool m_isBuildingMenuOpen = false;
    private bool m_isDynamicInventoryOpen = false;
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
        CloseAllPanels();
    }

    public void Start()
    {
        if (m_playerController == null)  m_playerController = PlayerController.Instance;
        m_playerController.OnMenuAction += OnMenuAction;
        if(m_dynamicInventoryDisplay == null) m_dynamicInventoryDisplay = FindAnyObjectByType<DynamicInventoryDisplay>();

        if (m_playerController != null) m_playerController.OnMenuAction += OnMenuAction;
        m_fader.alpha = 0f;
        m_fader.blocksRaycasts = false;
    }
    private void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += OnDynamicInventoryDisplayRequested;
    }

    private void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= OnDynamicInventoryDisplayRequested;
        if (m_playerController != null) m_playerController.OnMenuAction -= OnMenuAction; 
    }
    private void SetState(bool rover, bool building, bool dynamic, InventorySystem invToDisplay)
    {
        m_isRoverInventoryOpen = rover;
        m_isBuildingMenuOpen = building;
        m_isDynamicInventoryOpen = dynamic;

        if (m_roverInventoryPanel != null)
            m_roverInventoryPanel.SetActive(rover);
        if (BuildingMenuUI.Instance != null)
            BuildingMenuUI.Instance.gameObject.SetActive(building);

        if (m_dynamicInventoryDisplay != null)
        {
            m_dynamicInventoryDisplay.gameObject.SetActive(dynamic);

            if (dynamic && invToDisplay != null)
            {
                m_dynamicInventoryDisplay.RefreshDynamicInventory(invToDisplay);
            }
        }

        UpdateGlobalState();
    }
    private void UpdateGlobalState()
    {
        bool isAnyPlayerMenuOpen = m_isBuildingMenuOpen || m_isRoverInventoryOpen || m_isDynamicInventoryOpen;
        bool isMainMenuOpen = (m_mainMenu != null && m_mainMenu.activeInHierarchy);

        bool isGamePaused = isMainMenuOpen || isAnyPlayerMenuOpen; // The idea was to let time pass while in inventory or building Menu, but that proved to be quite disruptive
        Time.timeScale = isGamePaused ? 0f : 1f;

        if (isAnyPlayerMenuOpen || isMainMenuOpen)
        { 
            if (HUDManager.Instance != null) HUDManager.Instance.gameObject.SetActive(false);
            if (m_playerController != null) m_playerController.SwitchActionMap("UI");
        }
        else
        {
            if (HUDManager.Instance != null) HUDManager.Instance.gameObject.SetActive(true);
            if (m_playerController != null) m_playerController.SwitchActionMap("Exploration");
        }
    }

    #endregion
    #region HelperMethods

    private void CloseAllPanels()
    {
        if (BuildingMenuUI.Instance != null)
            BuildingMenuUI.Instance.gameObject.SetActive(false);
        if (m_roverInventoryPanel != null)
            m_roverInventoryPanel.SetActive(false);
        if (m_dynamicInventoryDisplay != null)
            m_dynamicInventoryDisplay.gameObject.SetActive(false);
        if (m_mainMenu != null)
            m_mainMenu.SetActive(false);
    }

    public void OnMenuAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleMainMenu();
        }
    }
    public void OnDynamicInventoryDisplayRequested(InventorySystem invToDisplay)
    {
        if (m_mainMenu != null && m_mainMenu.activeInHierarchy) return;

        if (m_isDynamicInventoryOpen)
        {
            SetState(false, false, false, null); // Close all
        }
        else
        {
            SetState(true, false, true, invToDisplay);
        }
    }
    #endregion
    #region Menu Toggles

    public void TogglePlayerMenu()
    {
        if (m_mainMenu != null && m_mainMenu.activeInHierarchy) return;

        if (m_isDynamicInventoryOpen)
        {
            SetState(false, false, false, null);
        }
        else if (m_isBuildingMenuOpen)
        {
            SetState(false, false, false, null);
        }
        else
        {
            SetState(true, true, false, null);
        }
    }

    public void ToggleMainMenu()
    {
        if (m_isDynamicInventoryOpen || m_isBuildingMenuOpen || m_isRoverInventoryOpen)
        {
            SetState(false, false, false, null);
        }
        if(m_mainMenu != null)
        {
            m_mainMenu.SetActive(!m_mainMenu.activeInHierarchy);
        }
        UpdateGlobalState();
    }


    #endregion
    #region Death
    internal void FadeToBlack(float duration)
    {
        StartFade(1f, duration);
    }

    internal void FadeFromBlack(float duration)
    {
        StartFade(0f, duration);
    }

    private void StartFade(float targetAlpha, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DoFade(targetAlpha, duration));
    }

    private IEnumerator DoFade(float targetAlpha, float duration)
    {
        float startAlpha = m_fader.alpha;
        float timer = 0f;

        if (targetAlpha == 1f)
        {
            m_fader.blocksRaycasts = true;
        }
        while(timer <= duration)
        {
            timer += Time.deltaTime;
            m_fader.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }
        m_fader.alpha = targetAlpha;

        if (targetAlpha == 0)
        {
            m_fader.blocksRaycasts = false;
        }
    }

    #endregion
}