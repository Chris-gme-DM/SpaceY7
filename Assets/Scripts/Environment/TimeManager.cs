using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TimeManager : MonoBehaviour
{
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;


    private int Minutes
    { get { return Minutes;  } set { Minutes = value; OnMinutesChange(value); } }

    private int Hours
        { get { return Hours;  } set { Hours = value; OnHoursChange(value);  } }
    private int Days
    { get { return Days;  } set { Days = value; OnDaysChange(value); } }


    private float tempSecond;

    public void Update()
    {
        tempSecond = Time.deltaTime;

        if(tempSecond >= 1)
        {
            Minutes += 1;
            tempSecond = 0;
        }
    }

    private void OnMinutesChange(int value)
    {
        if (value >= 60)
        {
            Hours++;
            Minutes = 0;
        }
        if (value >= 24)
        {
            Days++;
            Hours = 0;
        }
    }

    private void OnHoursChange(int value)
    {
        if (value == 3)
        {
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, 10f));
        }
        if (value == 6)
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, 10f));
        }
        if (value == 18)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, 10f));
        }
        if (value == 21)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, 10f));
        }
        // 0 - 3 nacht
        // 3 - 6 dämmerung
        // 6 - 6 tag
        // 6 - 9 dämmerung
        // 9 -12 nacht

    }

    private void OnDaysChange(int value)
    {
        //throw new NotImplementedException();
    }

    private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
    {
        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);

        for (float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox.SetTexture("_Texture1", b);
    }
}
