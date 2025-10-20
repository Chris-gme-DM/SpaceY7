using UnityEngine;

[RequireComponent (typeof(SphereCollider))]

// script for pickung up items
public class ItemPickUp : MonoBehaviour
{
    public float PickUpRadius = 5;
    public InventoryItemData ItemData;

    private SphereCollider myCollider;

    // setting everything up
    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myCollider.radius = PickUpRadius;
    }

    /// <summary>
    /// on collision, will look for the InventoryHolder Component
    /// if found, it will add itself to the corresponding inventory and destroy the gameObject
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.transform.GetComponent<InventoryHolder>();

        if (!inventory) return;
        if (inventory.InventorySystem.AddToInventory(ItemData, 1))
        {
            SoundEffectManager.Play("PickUp");
            //Debug.Log("Item was destroyed successfully");
            Destroy(this.gameObject);
        }
    }
}
