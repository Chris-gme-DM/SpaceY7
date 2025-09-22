using UnityEngine;
using UnityEngine.Events;

public class ChestInventory : InventoryHolder, IInteractable
{
    public UnityAction<IInteractable> OnInteractionComplete {  get; set; }

    public void Interact(GameObject interactor)// called when we interact with the chest
    {
        OnDynamicInventoryDisplayRequested?.Invoke(inventorySystem); // call from inventoryHolder
    }
}
