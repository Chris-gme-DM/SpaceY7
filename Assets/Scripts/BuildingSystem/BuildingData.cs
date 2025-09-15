using UnityEditor;
using UnityEngine;

/// <summary>
/// Script that holds relevant data of the building options
/// </summary>
[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    public int BuildingID;
    public string BuildingName;
    public string BuildingDescription;
    public GameObject BuildingPrefab;
    public Vector3 PlacementDimensions;

}
