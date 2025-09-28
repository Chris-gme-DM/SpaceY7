using UnityEngine;


/// <summary>
/// quick and dirty script for the sound of footsteps
/// </summary>
public class Footsteps : MonoBehaviour
{
    [SerializeField] protected PlayerController playerController;   // change it out

    void Start()
    {
        InvokeRepeating("PlayFootsteps", 0.1f, 0.6f);
    }

    public void PlayFootsteps()
    {
        // only play the footsteps when the player is on ground and not idle
        if (playerController.IsGrounded && !playerController.IsIdle)
        {
            //Debug.Log("crunchy cr0nch crinch");
            SoundEffectManager.Play("FS Stone");        // TO-DO: CHANGE THIS OUT IT'S HORRENDOUS
        }

    }
}

