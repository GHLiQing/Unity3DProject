using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Text progress;
    public static string sceneName = "";
    public Slider slider;
    public float asyProgress,showProgress;
    AsyncOperation asy;
    public ProgressImage pig;

    public static bool once = true;

    private void Awake()
    {
        ProcessManager.Instance.StopProcess();
    }
    void Start()
    {
        if (!string.IsNullOrEmpty(sceneName))
            StartCoroutine(LoadScene());
        else
            Debug.LogError("sceneName null");
    }

    void Update()
    {
        if (showProgress < asyProgress)
        {
            showProgress= Mathf.Lerp(showProgress, asyProgress, Time.deltaTime);
            slider.value = showProgress;
            progress.text = (int)(showProgress*100) + "%";
            pig.ShowImage(showProgress);
            if (showProgress >= 0.99f)
            {
                showProgress = 1;
                slider.value = 1;
                progress.text = (int)(showProgress * 100) + "%";
                asy.allowSceneActivation = true;
                pig.ShowImage(showProgress);
            }
        }
    }
    IEnumerator LoadScene()
    {
         asy = SceneManager.LoadSceneAsync(sceneName);
         asy.allowSceneActivation = false;
        while (!asy.isDone)
        {
            //  progress.text = asy.progress.ToString("0.00");
            if (asy.progress < 0.9f)
            {
                asyProgress = asy.progress;
            }
            else
            {
                asyProgress = 1;
            }      
           
            yield return null;
        }
    }
}
