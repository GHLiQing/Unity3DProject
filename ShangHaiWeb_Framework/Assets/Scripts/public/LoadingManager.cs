using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : Singleton<LoadingManager>
{
    public  string SceneName = "";
    private Text text_process;

    private bool beginLoad = false;

    public void LoadScene(string sceneName)
    {
        Debug.Log("target scene:" + sceneName);
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("sceneLoaded += loadedEve");

            beginLoad = false;
            SceneManager.LoadScene("Loading");

            SceneName = sceneName;
        }
    }

    //public void LoadScene(string sceneName)
    //{
    //    Debug.Log("target scene:" + sceneName);
    //    if (!string.IsNullOrEmpty(sceneName))
    //    {


    //        SceneName = sceneName;
    //        loadedEve();
    //    }

    //}



    void Update()
    {
        if (!beginLoad&&SceneManager.GetActiveScene().name.Equals("Loading"))
        {
            beginLoad = true;
            loadedEve();
        }
    }


     void loadedEve()
    {
     
        //    SceneManager.sceneLoaded -= loadedEve;
            Debug.Log("scenename:" + SceneName);
            LoadAssetManager.Instance.LoadAssetBundle(LoadType.Single, SceneName, SceneName, "", (tempObj) =>
            {
                Debug.Log("success load:"+ tempObj.Length);
                SceneManager.LoadScene(SceneName);
            },
            (progress) =>
            {
                if (progress == -1)
                {

                }
                else
                {
                    Debug.Log("LoadProgress:  " + progress);
                    if (text_process == null)
                    {
                        text_process = GameObject.Find("progressText").GetComponent<Text>();
                    }
                    text_process.text = "process:" + progress + "%";
                }
            });
        
    }

}
