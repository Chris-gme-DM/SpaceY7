using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Events;
using System.Collections.Generic;

// to do: let this inherit tihngs from InteractableData (aka merge it)

public class RessourceObject : BaseInteractable, IInteractable
{
    [SerializeField] private RessourceSO ressource; 

    //public int InteractableID;
    public int currentStage;
    //private float currentTime;
    public GameObject currentRessource;

    public GameObject cycleManager;
    public string currentCycle;
    //public GameObject resourceSO;
    public string harvestCycle;
    public string respawnCycle;
    public bool isNotRespawned;

    private PlayerController m_playerController;
    private InteractionManager interactionManager;

    public InventoryItemData resourceItem; // temp for testing. and now for the final game bc i don't have the time
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
       // m_playerController.OnInteractAction += OnInteractAction;

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

        if (currentCycle == harvestCycle && currentStage < ressource.MaxStage)

        //und was wenn es schon in der zweiten stage ist? // && currentStage < ressource.MaxStage
        {
            //Debug.Log("READY FOR HARVESTING");
            currentStage++;

            Destroy(currentRessource);
            currentRessource = Instantiate(ressource.GetRessourceByStage(ressource.MaxStage), transform);

            isNotRespawned = true;
           // Debug.Log("isNotRespawned: " + isNotRespawned);

           // Debug.Log("It's Harvest Time!");

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
        //if (interactionManager.newGameObject == this && context.performed)
        if (context.performed)
        {
            Debug.Log("You are trying to interact with a possible resource.");

            //Debug.Log(resourceItem);
            //interactionManager.CheckforInteractable();



            //if (HasMaxLevel())
            //{
            //    playerInventory.InventorySystem.AddToInventory(resourceItem, resourceItem.type.amount);
            //    Destroy(currentRessource);

            //}
            //else
            //{
            //    Debug.Log("There is nothing to harvest here");
            //}
        }
    }

    public override void Interact(GameObject interactor)
    {
        Debug.Log("(INTERACT) You are interacting with a possible resource.");
        //interactionManager.CheckforInteractable();

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

}
