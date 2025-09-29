using System.Collections;
using UnityEngine;

public class ChestScript : BaseInteractable
{
    private Animator m_animator;
    private InventorySystem m_inventorySystem;
    [SerializeField] private float m_menuOpenDelay = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_animator == null) m_animator = GetComponent<Animator>();
        m_inventorySystem ??= this.gameObject.GetComponent<InventoryHolder>().InventorySystem;
    }

    public override void Interact(GameObject interactor)
    {
        m_animator.SetTrigger("InteractAction");
        StartCoroutine(OpenMenuAfterDelay());
    }
    private IEnumerator OpenMenuAfterDelay()
    {
        yield return new WaitForSeconds(m_menuOpenDelay);
        UIManager.Instance.OnDynamicInventoryDisplayRequested(m_inventorySystem);
    }
}
