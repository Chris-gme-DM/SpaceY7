using UnityEngine;
using UnityEngine.Events;

public class ChestInventory : InventoryHolder, IInteractable
{
    public UnityAction<IInteractable> OnInteractionComplete {  get; set; }

    public void Interact(Interactor interactor, out bool interactSuccessful)    // called when we interact with the chest
    {
        OnDynamicInventoryDisplayRequested?.Invoke(inventorySystem); // call from inventoryHolder
        interactSuccessful = true;
    }

    public void EndInteraction()
    {

    }


}
