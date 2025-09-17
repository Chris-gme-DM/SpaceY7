using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "Scriptable Objects/InteractableData")]
public class InteractableData : ScriptableObject
{
    public int InteractableID;
    public string InteractableName;
    [TextArea(4,4)]
    public string InteractableDescription;
    public GameObject InteractablePrefab;
}
