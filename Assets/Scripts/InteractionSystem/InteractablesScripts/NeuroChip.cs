using UnityEngine;

public class NeuroChip : BaseInteractable
{
    public override void Interact(GameObject interactor)
    {
        PlayerStats.Instance.CollectNeuroChip();
        Destroy(gameObject);
    }
}
