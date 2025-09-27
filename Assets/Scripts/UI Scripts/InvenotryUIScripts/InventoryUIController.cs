using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InventoryUIController : MonoBehaviour
{
    public DynamicInventoryDisplay inventoryPanel;

    private void Awake()
    {
        inventoryPanel.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
    }

    private void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
    }

    void Update()
    {
        //if (Keyboard.current.bKey.wasPressedThisFrame)
        //{
        //    Debug.Log("B was pressed, opening Dynamic Inventory.");
        //    DisplayInventory(new InventorySystem(Random.Range(3, 9)));
        //}

        if (inventoryPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("Escape was pressed, closing Dynamic Inventory.");
            inventoryPanel.gameObject.SetActive(false);
        }
    }

    void DisplayInventory (InventorySystem invToDisplay)
    {
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.RefreshDynamicInventory(invToDisplay);
    }
}
