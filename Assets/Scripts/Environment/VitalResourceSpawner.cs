using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VitalResourceSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_Prefab;
    [SerializeField] private float m_spawnRadius = 5f;
    [SerializeField] private float m_minSpawnTime = 20f;
    [SerializeField] private float m_maxSpawnTime = 40f;

    private float m_currentSpawnTimer;
    private float m_nextSpawnTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetTimer();
    }


    // Update is called once per frame
    void Update()
    {
        m_currentSpawnTimer += Time.deltaTime;
        if(m_currentSpawnTimer >= m_nextSpawnTime)
        {
            SpawnResource();
            ResetTimer();
        }
    }

    private void SpawnResource()
    {
        Vector3 randomCircle = Random.insideUnitSphere * m_spawnRadius;
        randomCircle.y = 0;
        Vector3 spawnPoint = transform.position + randomCircle;
        Instantiate(m_Prefab, spawnPoint, Quaternion.identity);
    }

    private void ResetTimer()
    {
        m_nextSpawnTime = Random.Range(m_minSpawnTime, m_maxSpawnTime);
        m_currentSpawnTimer = 0;
    }
}
