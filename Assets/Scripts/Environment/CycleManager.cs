using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;


public class CycleManager : MonoBehaviour
{
    [SerializeField] private Material skyboxNight;
    [SerializeField] private Material skyboxSunrise;
    [SerializeField] private Material skyboxDay;
    [SerializeField] private Material skyboxSunset;

    public TextMeshProUGUI timeText;

    private int minutes;
    public int Minutes
    { get { return minutes;  } set { minutes = value; OnMinutesChange(value); } }

    private int hours;
    public int Hours
        { get { return hours; } set { hours = value; OnHoursChange(value);  } }

    private int days;
    public int Days
        { get { return days;  } set { days = value; OnDaysChange(value); } }

    private float tempSecond;

    private List<RessourceObject> ressourceObjects = new List<RessourceObject>();

    public static CycleManager instance;

    [SerializeField] private RessourceSO ressource;

    public string currentCycle;
    public bool night;
    public bool sunrise;
    public bool day;
    public bool sunset;

    //enum Cycle
    //{ 
    //    Night,
    //    Sunrise,
    //    Day,
    //    Sunset,
    //};

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        Time.timeScale = 1f;
        //timeText.text = "This is your timeDisplay.";
    }

    public void Start()
    {
        //Cycle cycle;
    }

    public void Update()
    {
        tempSecond += Time.deltaTime * 10;

        if(tempSecond >= 1)
        {
            Debug.Log("Ja, es ist Zeit vergangen.");

            Minutes += 1;
            tempSecond = 0;

            timeText.text = "Time is:" + days + "d " + hours + "h " + minutes + "min";
        }

        foreach (RessourceObject ressourceObject in ressourceObjects)
        {
            ressourceObject.CheckRessource();
        }
        Debug.Log("maxStage is: " + ressource.MaxStage);


        //timeText.text = "Time is:" + days + "d " + hours + "h " + minutes + "min";
        //timeText.text = Time.deltaTime.ToString();
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
            CheckCycle();
        }
        if (value == 6)
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, 10f));          // tag
            CheckCycle();
        }
        if (value == 18)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, 10f));           // sonnenuntergang
            CheckCycle();
        }
        if (value == 21)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, 10f));         // nacht
            CheckCycle();
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

    private IEnumerator LerpSkybox(Material a, Material b, float time)
    {
        RenderSettings.skybox = a;  // ("_Texture1", a);
        RenderSettings.skybox = b;  // ("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);

        for (float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox = b;  // ("_Texture1", b);
    }

    private void CheckCycle()
    {
        Debug.Log("Der aktuelle Cycle wird gecheckt");

        // was wenn es keinen cycle gibt du genie

        if (night)
        {
            sunrise = true;
            night = false;
            ChangeCycle("sunrise");
            return;
        }
        if (sunrise)
        {
            day = true;
            sunrise = false;
            ChangeCycle("day");
            return;
        }
        if (day)
        {
            sunset = true;
            day = false;
            ChangeCycle("sunset");
            return;
        }
        if (sunset)
        {
            night = true;
            sunset = false;
            ChangeCycle("night");
            return;
        }
    }

    public void ChangeCycle(string cycle)
    {
        currentCycle = cycle;
        Debug.Log("Es ist gerade " + cycle);
    }

    public void RegisterRessource(RessourceObject ressourceObject) 
    {
       ressourceObjects.Add(ressourceObject);
    }

    public void UnregisterRessource(RessourceObject ressourceObject)
    {
        ressourceObjects.Remove(ressourceObject);
    }
}
