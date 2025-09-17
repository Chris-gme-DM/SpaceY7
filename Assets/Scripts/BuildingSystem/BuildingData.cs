using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that holds relevant data of the building options
/// </summary>
[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    public int BuildingID;
    public string BuildingName;
    [TextArea(4,4)]
    public string BuildingDescription;
    public GameObject BuildingPrefab;
    public Vector3 PlacementDimensions;
    public List<ResourceCost> Costs;
}
[Serializable]
public class ResourceCost
{
    public ResourceType type;
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

}