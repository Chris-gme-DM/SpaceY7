using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RessourceSO", menuName ="Scriptable Objects/RessourceSO")]
public class RessourceSO : ScriptableObject
{
    public string RessourceName;
    public List<GameObject> RessourcePrefabs;

    public string HarvestCycle;
    public float HarvestRessource; // pls change the names
    public int MaxStage { get { return RessourcePrefabs.Count - 1; } }

    public GameObject GetRessourceByStage(int stage)
    {
        //if (stage >= MaxStage)
        //{
        //    return null;
        //}
        return RessourcePrefabs[stage];
    }

}
