using UnityEngine;

public class Sounds : MonoBehaviour
{
    public void PlayMenuButtonSFX()
    {
        Debug.Log("Clickedi Clackedi");
        SoundEffectManager.Play("Click");
    }
}
