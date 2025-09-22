using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "Scriptable Objects/InteractableData")]
public class InteractableData : ScriptableObject
{
    public int InteractableID;
    public string InteractableName;
    [TextArea(4,4)]
    public string InteractableDescription;
    public GameObject InteractablePrefab;
    public InteractableType InteractableType;
// ADD Sound the interactable makes here if available and write the logic
}