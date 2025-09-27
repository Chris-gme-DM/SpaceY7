using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] private InventorySlot_UI[] slots;

    public override void Start()
    {
        base.Start();

        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.InventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        else Debug.LogWarning($"No inventory assigned to {this.gameObject}");

        AssignSlot(inventorySystem);
    }

    public override void AssignSlot(InventorySystem invToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

       // if (slotDictionary.Count != inventorySystem.InventorySize) Debug.Log("Inventory slots out of sync on {this.gameObject}");
       // Debug.Log(slotDictionary.Count);
       // does not work as intended, Length is not available will come back to it tho

        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }
}
