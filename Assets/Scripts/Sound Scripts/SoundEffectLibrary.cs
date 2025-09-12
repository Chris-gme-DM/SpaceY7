using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectLibrary : MonoBehaviour
{
    [SerializeField] private SoundEffectGroup[]soundEffectGroups;
    private Dictionary<string, List<AudioClip>> soundDictionary;
    // string = name sound

    private void Awake()
    {
        InitializeDictonary();

    }

    private void InitializeDictonary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (SoundEffectGroup soundEffectGroup in soundEffectGroups)
        {
            soundDictionary[soundEffectGroup.name] = soundEffectGroup.audioClips;
        }
    }

    public AudioClip GetRandomClip(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            List<AudioClip> audioClips = soundDictionary[name];
            if (audioClips.Count > 0)
            {
                return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
            }
        }
        return null;
    }
}
[System.Serializable]

// to populate
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClips;
}