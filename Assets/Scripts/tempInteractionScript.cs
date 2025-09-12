using UnityEngine;

public class tempInteractionScript : MonoBehaviour
{
    [SerializeField] private GameObject roverInventory;

    private void Awake()
    {
        {
            roverInventory.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision was detected");
        roverInventory.SetActive(true);

        //if (collision.gameObject.tag == "player") roverInventory.SetActive(true);

        //if (collision.gameObject.tag == "player")
        //{
        //    roverInventory.SetActive(true);
        //    Debug.Log("Beep Beep Boop Beep");
        //}
        //if (collision.gameObject.tag == "player" && roverInventory.activeInHierarchy)
        //{
        //    roverInventory.SetActive(false);
        //    Debug.Log("Beep Beep Boop Beep");
        //}
    }

    private void OnTriggerExit(Collider collision)
    {
        roverInventory.SetActive(false);
    }
}
