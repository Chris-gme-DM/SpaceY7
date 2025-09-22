using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class TimeManager : MonoBehaviour
{
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;

    public TextMeshProUGUI timeText;

    private int minutes;
    public int Minutes
    { get { return minutes;  } set { minutes = value; OnMinutesChange(value); } }

    private int hours;
    public int Hours
        { get { return hours;  } set { hours = value; OnHoursChange(value);  } }

    private int days;
    public int Days
    { get { return days;  } set { days = value; OnDaysChange(value); } }


    private float tempSecond;

    public void Awake()
    {
        Time.timeScale = 1f;
        //timeText.text = "This is your timeDisplay.";
    }

    public void Update()
    {
        tempSecond = Time.deltaTime * 100;      // atm this is framerate dependant PLEASE CHANGE IT

        if(tempSecond >= 1)
        {
            Debug.Log("Ja, es ist Zeit vergangen.");

            Minutes += 1;
            tempSecond = 0;

            timeText.text = "Time is:" + days + "d " + hours + "h " + minutes + "min";
        }

        //timeText.text = "Time is:" + days + "d " + hours + "h " + minutes + "min";
        //timeText.text = Time.deltaTime.ToString();
    }

    public void Start()
    {
        // timeText.text = "This is your timeDisplay.";
    }

    private void OnMinutesChange(int value)
    {
        if (value >= 60)
        {
            Hours++;
            Minutes = 0;
        }
        if (Hours >= 24)
        {
            Hours = 0;
            Days++;
        }
    }

    private void OnHoursChange(int value)
    {
        if (value == 3)
        {
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, 10f));        // sonnenaufgang
        }
        if (value == 6)
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, 10f));          // tag
        }
        if (value == 18)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, 10f));           // sonnenuntergang
        }
        if (value == 21)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, 10f));         // nacht
        }
        // 0 - 3 nacht
        // 3 - 6 dämmerung
        // 6 - 6 tag
        // 6 - 9 dämmerung
        // 9 -12 nacht

    }

    private void OnDaysChange(int value)
    {
        Debug.Log("It's a new day!");
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
