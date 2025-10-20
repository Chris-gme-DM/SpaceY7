using UnityEngine;

public class DoorScript : BaseInteractable
{
    private Animator m_animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_animator == null) m_animator = GetComponent<Animator>();
    }
    public override void Interact(GameObject interactor)
    {
        m_animator.SetTrigger("InteractAction");
    }

}
