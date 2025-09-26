using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Events;

public class RessourceObject : MonoBehaviour, IInteractable
{
    [SerializeField] private RessourceSO ressource;

    public int currentStage;
    //private float currentTime;
    private GameObject currentRessource;

    public GameObject cycleManager;
    public string currentCycle;
    //public GameObject resourceSO;
    public string harvestCycle;
    public string respawnCycle;
    public bool isNotRespawned;

    private PlayerController m_playerController;

    public InventoryItemData resourceItem; // temp for testing
    public InventoryHolder playerInventory;

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    private void Awake()
    {
        string currentCycle = cycleManager.GetComponent<CycleManager>().currentCycle; // i mean, you can already access cycleManager...
        //string harvestCycle = FindFirstObjectByType<RessourceSO>().HarvestCycle;
        string harvestCycle = ressource.HarvestCycle;
        string respawnCycle = ressource.RespawnCycle;

    }

    public void Start()
    {
        CycleManager.instance.RegisterRessource(this);
        currentRessource = Instantiate(ressource.GetRessourceByStage(currentStage), transform);

        if (m_playerController == null) { m_playerController = FindAnyObjectByType<PlayerController>(); }
        m_playerController.OnInteractAction += OnInteractAction;

        //Debug.Log("Starting");
        //for (int i = 0; i < ressource.MaxStage; i++)
        //{
        //    GameObject obj = Instantiate(ressource.GetRessourceByStage(i), transform);
        //}
    }

    public void CheckRessource() // �berfl�ssig, change it
    {
        CheckRessource(harvestCycle);
    }

    public void CheckRessource(string harvestCycle)
    {
        currentCycle = cycleManager.GetComponent<CycleManager>().currentCycle;

        if (currentCycle == harvestCycle && currentStage < ressource.MaxStage)   //und was wenn es schon in der zweiten stage ist? // && currentStage < ressource.MaxStage
        {
            Debug.Log("READY FOR HARVESTING");
            //harvestCycle = null;
            currentStage++;

            Destroy(currentRessource);
            currentRessource = Instantiate(ressource.GetRessourceByStage(ressource.MaxStage), transform);

            isNotRespawned = true;
            Debug.Log("isNotRespawned: " + isNotRespawned);

            Debug.Log("It's Harvest Time!");

            //if (currentStage >= ressource.MaxStage)
            //{
            //    CycleManager.instance.UnregisterRessource(this);
            //}
        }
        if (currentCycle == respawnCycle && isNotRespawned)
        {
            { 
            Destroy(currentRessource);
            
                currentRessource = Instantiate(ressource.GetRessourceByStage(ressource.FirstStage), transform);
                isNotRespawned = false;
            }
        }
    }

    public bool HasMaxLevel()
    {
        return currentCycle == harvestCycle;
    }

    public void OnInteractAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("You are trying to interact with a possible resource.");
            Debug.Log(resourceItem);

            if (HasMaxLevel())
            {
                playerInventory.InventorySystem.AddToInventory(resourceItem, resourceItem.type.amount);
                Destroy(currentRessource);

                //todo jo: boolean for isHarvested on RessourceSO, then respawn that shit with a new
                // respawnCycle
            }
            else
            {
                Debug.Log("There is nothing to harvest here");
            }
        }
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("(INTERACT) You are interacting with a possible resource.");

            if (HasMaxLevel())
            {
                playerInventory.InventorySystem.AddToInventory(resourceItem, resourceItem.type.amount);
                Destroy(currentRessource);
            }
            else
            {
                Debug.Log("There is nothing to harvest here");
            }
        }

    public void Harvest(GameObject gameObject)
    {

    }


}
