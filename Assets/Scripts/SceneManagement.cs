using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.ComponentModel;
using AsyncOperation = UnityEngine.AsyncOperation;

// [ async loading from https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.htm]
public class SceneManagement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI loadingText;
    public GameObject startButton;
    AsyncOperation asyncOperation;
    public int sceneID;
    public bool loadingInProgress = false;

    public void Awake()
    {
        sceneID = SceneManager.GetActiveScene().buildIndex;

        if (startButton != null && loadingText != null)
        {
            startButton.SetActive(false);
            loadingText.text = "test";
        }

    }

    public void Update()
    {
        if (!loadingInProgress) CheckScene();
    }

    public void CheckScene()
    {
        if (sceneID == 1)
        {
            StartCoroutine(WhileLoading());
            loadingInProgress = true;
        }
    }
    public void LoadingScreen()
    {
        SceneManager.LoadScene(1);      // TO-DO: async!
    }



    IEnumerator WhileLoading()
    {
        yield return null;

        asyncOperation = SceneManager.LoadSceneAsync(2); 

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            loadingText.text = "Loading: " + (asyncOperation.progress * 100) + "%";

            if (asyncOperation.progress >=0.9f)
            {
                startButton.SetActive(true);
                loadingText.text = "press play to play";
            }

            yield return null;
        }
    }

    public void StartGame()
    {
        asyncOperation.allowSceneActivation = true;

    }
}
