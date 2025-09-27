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
    public Sprite PreviewImage;
    public List<Resources> BuildingCosts;
    public List<InteractableData> EmbeddedInteractableIDs;
}
