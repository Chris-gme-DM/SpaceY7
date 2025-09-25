using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class UIManagement : MonoBehaviour
{
    public static UIManagement Instance { get; private set; }

    public GameObject menu;
    private PlayerController m_playerController;

    //public event Action<InputAction.CallbackContext> OnMenuAction;

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
        menu.SetActive(false);
    }

    public void Start()
    {
        if (m_playerController == null) { m_playerController = FindAnyObjectByType<PlayerController>(); }
        m_playerController.OnMenuAction += OnMenuAction;
    }

    //public void OnEnable()
    //{
    //    inputActions.Exploration.Menu.performed += ctx => OnMenuAction?.Invoke(ctx);
    //}

    //public void OnDisable()
    //{
    //    inputActions.Exploration.Menu.performed -= ctx => OnMenuAction?.Invoke(ctx);
    //}

    public void OnMenuAction(InputAction.CallbackContext context)
    {
        //if (menu.activeInHierarchy && context.performed)
        //{
        //    Debug.Log("Closing the Menu");
        //    menu.SetActive(false);
        //    //return;
        //}

        if (!menu.activeInHierarchy && context.performed)
        {
            Debug.Log("Opening the Menu");
            menu.SetActive(true);
            //return;
        }

    }
}
