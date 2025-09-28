using UnityEngine;

/// <summary>
///  simple script for playing sfx sound when clicking on menu buttons
/// </summary>
public class Sounds : MonoBehaviour
{
    public void PlayMenuButtonSFX()
    {
        Debug.Log("Clickedi Clackedi");
        SoundEffectManager.Play("Click");
    }
}
