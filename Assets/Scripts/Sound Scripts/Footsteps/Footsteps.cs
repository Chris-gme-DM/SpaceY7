using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] protected PlayerController playerController;   // change it out

    void Start()
    {
        InvokeRepeating("PlayFootsteps", 0.1f, 0.6f);
    }

    public void PlayFootsteps()
    {
        if (playerController.IsGrounded && !playerController.IsIdle)
        {
            Debug.Log("crunchy cr0nch crinch");
            SoundEffectManager.Play("FS Stone");
        }

    }

    //public void Update()
    //{
    //    if (playerController.IsGrounded)
    //    {
    //        // is there grass around?
    //        // is there water around?

    //        // no grass, no water: sand
    //        PlayFootStepsSand();

    //        //
    //    }
    //}

    //    public void PlayFootStepsSand()
    //    {
    //        Debug.Log("crunchy cr0nch crinch");
    //        SoundEffectManager.Play("FS Sand");
    //    }
}

