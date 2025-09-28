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
}
