using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// This script randomly manipulates the global gravity over time. The idea is to simulate instability of the planet.
/// Once a gravitational shift is applied the subroutine determines new gravitational value to reach and the period of the time that this shift happens.
/// DO NOT OVER ENGINEER IT
/// After any transition is completed it determines a new values for the transition time and the target gravity.
/// It slowly adjusts the current gravity to the target gravity. And repeat the process after completion.
/// Last changes: Deleting unnecessary comments from development time.
/// </summary>
public class GravityManager : MonoBehaviour
{
    #region Inspector
    [Header("Gravity Settings")]
    [Tooltip("accleration in m/s²; 9.81 is usual earth gravity")]
    [SerializeField] private float baseGravity = 9.81f; // normal earth gravity
    // Set boundaries of the manipulation to make it not too disruptive
    // Players have feelings too
    [Tooltip("minimum percentage in comparison to base")]
    [SerializeField] private float minGravityPercentage = 0.9f; // the minimum gravity compared to base

    [Tooltip("maximum percentage in comparison to base")]
    [SerializeField] private float maxGravityPercentage = 1.1f; // the maximum gravity compared to base
    [Header("Timer Settings")]
    [Tooltip("Minimum  Transition time in sec")]
    [SerializeField] private float minTransitionTime = 120f; // in seconds

    [Tooltip("Maximum  Transition time in sec")]
    [SerializeField] private float maxTrasnitionTime = 240f; // in seconds

    #endregion

    [SerializeField] private float m_currentGravity;
    public float CurrentGravityDisplay => m_currentGravity/baseGravity; // HUD Manager calls this
    private float m_targetGravity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        m_currentGravity = -baseGravity;
        Physics.gravity = new Vector3(0, m_currentGravity, 0);
        StartCoroutine(ManageGravityRoutine());
    }

    private IEnumerator ManageGravityRoutine()
    {
        while (true)
        {
            // Determine target gravity
            float randomMultiplier = Random.Range(minGravityPercentage, maxGravityPercentage);
            m_targetGravity = -baseGravity * randomMultiplier;
            // Determine transition duration
            float transitionDuration = Random.Range(minTransitionTime, maxTrasnitionTime);
            float elapsedTime = 0;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;
                // adjust the gravity, by using Lerp we should experience a rather smooth tranisition and the player shouldnt feel disrupted.
                m_currentGravity = Mathf.Lerp(m_currentGravity, m_targetGravity, t);
                Physics.gravity = new Vector3(0, m_currentGravity, 0);
                yield return null;
            }
            m_currentGravity = m_targetGravity;
            Physics.gravity = new Vector3(0, m_currentGravity, 0);
            yield return new WaitForSeconds(3f);
        }
    }
    // For the HUDManager
    public float NormalizedGravityPosition
    {
        get
        {
            float currentMultiplier = m_currentGravity / -baseGravity;
            float gravityRange = maxGravityPercentage - minGravityPercentage;
            float normalizedValue = (currentMultiplier -minGravityPercentage) / gravityRange;
            return Mathf.Clamp01(normalizedValue);
        }
    }
}