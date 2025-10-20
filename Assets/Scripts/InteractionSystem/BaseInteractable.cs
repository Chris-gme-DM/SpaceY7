using UnityEngine;
using UnityEngine.Scripting;

[System.Serializable]
public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    public InteractableData interactableData;
    public InteractionType interactionType;

    public abstract void Interact(GameObject interactor);
}
