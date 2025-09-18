using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "Scriptable Objects/InteractableData")]
public class InteractableData : ScriptableObject, IInteractable
{
    public int InteractableID;
    public string InteractableName;
    [TextArea(4,4)]
    public string InteractableDescription;
    public GameObject InteractablePrefab;

    public void Interact(GameObject Interactable)
    {
        throw new System.NotImplementedException();
    }
}