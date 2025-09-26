using UnityEngine;

public class RoverAnimationController : MonoBehaviour
{
    private Animator m_animator;
    private RoverScript m_roverScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_animator == null) { m_animator = GetComponent<Animator>(); }
        if (m_roverScript == null) { m_roverScript = GetComponent<RoverScript>(); }
    }

    // Update is called once per frame
    void Update()
    {
        m_animator.SetBool("IsHovering", m_roverScript.IsHovering);
    }
}
