using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class ItemPickUp : MonoBehaviour
{
    public float PickUpRadius = 5;
    public InventoryItemData ItemData;

    private SphereCollider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myCollider.radius = PickUpRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.transform.GetComponent<InventoryHolder>();

        if (!inventory) return;
        // if i added myself successfully, destroy me
        if (inventory.InventorySystem.AddToInventory(ItemData, 1))
        {
            SoundEffectManager.Play("PickUp");
            Debug.Log("Item was destroyed successfully");
            Destroy(this.gameObject);
        }
    }
}
