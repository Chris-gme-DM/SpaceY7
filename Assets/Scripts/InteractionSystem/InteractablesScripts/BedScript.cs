using UnityEngine;

public class BedScript : BaseInteractable
{
    public override void Interact(GameObject interactor)
    {
        // Replenish the player resources.
        PlayerStats.Instance.ChangeVitals(interactableData.Resources);
        // Timeskip
        // Ask Jos TimeManager
        // Save the game
        // WRITE THAT
        // Set the RespawnPoint in case of death
        PlayerStats.Instance.RespawnPoint = PlayerStats.Instance.gameObject.transform;
        // Write that
    }
}
