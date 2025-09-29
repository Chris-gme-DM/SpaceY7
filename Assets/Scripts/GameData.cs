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
    public Sprite ResourceIcon;

}
public enum ResourceType
{
    None,
    Energy,
    Oxygen,
    Water,
    Silicon,
    Fiber,
    Tape,
    Aluminium,
    Lithium,
    Copper,
    Neurochip,
    Microchip,
    Glühbirnchen,
}
#endregion
#region Interactables
public enum InteractableType
{
    Resource,
    Antenna,
    Bed,
    Door,
    Inventory,
    Rover,
    Other,
}
public interface IInteractable
{
    public abstract void Interact(GameObject interactor);
}
public enum InteractionType
{
    None,
    // Resources
    CollectPlant,
    ColelctRock,
    // Generic Actions
    Rover,
    Inventory,
    Door,

}
#endregion