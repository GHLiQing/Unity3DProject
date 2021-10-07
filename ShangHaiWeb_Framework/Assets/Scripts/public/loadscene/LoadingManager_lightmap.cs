using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager_lightmap : Singleton<LoadingManager_lightmap>
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadScene(string sceneName)
    {

        if (!string.IsNullOrEmpty(sceneName))
        {
            Loading.sceneName = sceneName;
            SceneManager.LoadScene("Loading");
        }
        else
        {
            Debug.LogError("???");
        }

    }
}
