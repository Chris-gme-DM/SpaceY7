using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private bool wasWarned;
    [SerializeField] private GameObject player;
    private GameObject playerStats;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") && !wasWarned)
        {
            Debug.Log("WARNUNG: HÖHERSTEIGEN KANN ZU TOD FÜHREN");
            wasWarned = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && wasWarned)
        {
            Debug.Log("YOU'RE DEAD NOW.");
            wasWarned = false;

            player.GetComponent<PlayerStats>().Respawn();
        }
    }


}
