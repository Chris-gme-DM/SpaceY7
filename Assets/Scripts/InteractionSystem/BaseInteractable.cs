using UnityEngine;

[System.Serializable]
public class BaseInteractable : MonoBehaviour, IInteractable
{
    public InteractableData interactableData;

    public void Interact(GameObject interactor)
    {
        throw new System.NotImplementedException();
    }
}
