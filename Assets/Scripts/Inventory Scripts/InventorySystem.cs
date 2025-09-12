using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using UnityEngineInternal;
using UnityEditor.ShaderKeywordFilter;
using Unity.VisualScripting.Dependencies.Sqlite;

[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => InventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;   // event will fire when we change sth

    // constructor for empty inventory slots - will take the size and construct that many
    public InventorySystem(int size)
    {
        inventorySlots = new List<InventorySlot>();

        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(InventoryItemData itemToAdd, int amountToAdd)
    {
        Debug.Log("Item is trying to be added to inventory");
       // checks if the item is already in the inventory
       if (ContainsItem(itemToAdd, out List<InventorySlot> invSlot))
        {
            foreach( var slot in invSlot)
            {
                Debug.Log("There is already the same item in the inventory.");

                if (slot.RoomLeftInStack(amountToAdd))
                {
                    Debug.Log("But there is room left in the stack.");
                    slot.AddToStack(amountToAdd);
                    OnInventorySlotChanged?.Invoke(slot);
                    return true;
                }
            }
        }
       // get the first free slot
       if (HasFreeSlot(out InventorySlot freeSlot))
        {
            Debug.Log("We have a free slot.");
            //Debug.Log(itemToAdd.MaxStackSize);
            freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
            OnInventorySlotChanged?.Invoke(freeSlot);
            return true;
        }

        // inventory doesn't have the item nor a free slot
        Debug.Log("No item was added.");
        return false;
    }

    // checks if there are already slots with this item and gives out a list with them
    public bool ContainsItem(InventoryItemData itemToAdd, out List<InventorySlot> invSlot)
    {
        invSlot = InventorySlots.Where(i => i.itemData == itemToAdd).ToList();

        return invSlot == null ? false : true;
    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = InventorySlots.FirstOrDefault(i  => i.itemData == null);
        return freeSlot == null ? false : true;
    }
}
