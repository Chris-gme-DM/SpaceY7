using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    private PlayerStats m_playerStats;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(m_playerStats == null) { m_playerStats = FindAnyObjectByType<PlayerStats>(); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
