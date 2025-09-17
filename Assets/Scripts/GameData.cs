using System;
using UnityEngine;

/// <summary>
/// This script holds Data of public enums and structs that are used throughout the entire project 
/// </summary>

#region Resources
[Serializable]
public class Resources
{
    public ResourceType resourceType;
    public int amount;

}
public enum ResourceType
{
    Energy,
    Oxygen,
    Water,
    Silicon,
    Fiber,
    Metal,
    Tape,
    Aluminium,
    Lithium,
}
#endregion
#region Interactables
[Serializable]
public class Interactable: MonoBehaviour, IInteractable
{
    public InteractableType interactableType;

    public void Interact(GameObject Interactable)
    {
        throw new NotImplementedException();
    }
}
public enum InteractableType
{
    Resource,
    Door,
    Inventory,
    Other,
}
public interface IInteractable
{
    public void Interact(GameObject Interactable);
}
#endregion
#region Collectables
public interface ICollectable
{
    void Pickup(GameObject Interactable);
}
#endregion