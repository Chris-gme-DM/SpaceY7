using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected InventorySlot_UI slotPrefab;

    public override void Start()
    {
        base.Start();
    }

    public void RefreshDynamicInventory(InventorySystem invToDisplay)
    {
        ClearSlots();
        inventorySystem = invToDisplay;
        AssignSlot(invToDisplay);
    }

    public override void AssignSlot(InventorySystem invToDisplay)
    {
        ClearSlots();

        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        if (invToDisplay == null) return;

        for (int i = 0; i < invToDisplay.InventorySize; i++)    // will create slots
        {
            var uiSlot = Instantiate(slotPrefab, transform);
            slotDictionary.Add(uiSlot, invToDisplay.InventorySlots[i]);
            uiSlot.Init(invToDisplay.InventorySlots[i]);
            uiSlot.UpdateUISlot();
        }
    }

    private void ClearSlots()
    {
        foreach (var item in transform.Cast<Transform>()) // getting all the children
        {
            Destroy(item.gameObject);   // why no pooling?
        }

        if (slotDictionary != null)
        {
            SlotDictionary.Clear();
        }
    }
}
