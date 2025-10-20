using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private List<InventoryHolder> totalPlayerInventory;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public int GetTotalResourceCount(ResourceType type)
    {
        int totalCount = 0;
        foreach (InventoryHolder holder in totalPlayerInventory)
        {
            totalCount += holder.InventorySystem.GetResourceCount(type);
        }
        return totalCount;
    }

    public bool RemoveResources(Resources cost)
    {
        if (GetTotalResourceCount(cost.resourceType) < cost.amount) return false;

        int remainingItemsToRemove = cost.amount;

        foreach (InventoryHolder holder in totalPlayerInventory)
        {
            InventorySystem system = holder.InventorySystem;
            if (system == null) continue;

            for (int i = system.InventorySize - 1; i >= 0; i--)
            {
                InventorySlot slot = system.InventorySlots[i];
                if (slot.itemData != null && slot.itemData.type.resourceType == cost.resourceType)
                {
                    int amountToRemove = Mathf.Min(remainingItemsToRemove, slot.StackSize);
                    slot.RemoveFromStack(amountToRemove);
                    remainingItemsToRemove -= amountToRemove;
                    system.OnInventorySlotChanged?.Invoke(slot);
                    if (slot.StackSize <=0)
                    {
                        slot.ClearSlot();
                    }
                    if (remainingItemsToRemove <= 0)
                    {
                        return true;
                    }
                }
            }
        }
        return remainingItemsToRemove <= 0;
    }
}
