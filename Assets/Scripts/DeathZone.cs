using UnityEngine;
using TMPro;

/// <summary>
/// will get a warning when entering the object, will respawn when exiting
/// because we don't want them to see the void around the crater
/// </summary>
public class DeathZone : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private GameObject playerStats;

    [SerializeField] private GameObject warningPanel;

    [SerializeField] private GameObject warningZone;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private GameObject deathZone;


    private void OnTriggerEnter(Collider other)
    {
        if (this == warningZone && other.CompareTag("Player"))
        {
            warningPanel.SetActive(true);
            warningText.text = "DANGER! Flying higher will result in certain death.";

            Debug.Log("Enterging the warningZone");
        }

        if (this == deathZone && other.CompareTag("Player"))
        {
            warningText.text = "Whoopsie";
            warningPanel.SetActive(false);

            player.GetComponent<PlayerStats>().Respawn();
        }
    }
}
