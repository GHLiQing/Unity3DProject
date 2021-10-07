using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
          // LoadingManager.Instance.LoadScene("s03");
           LoadingManager_lightmap.Instance.LoadScene("s03");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
           // LoadingManager.Instance.LoadScene("s01");
            LoadingManager_lightmap.Instance.LoadScene("s01");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
          //  LoadingManager.Instance.LoadScene("s04");
            LoadingManager_lightmap.Instance.LoadScene("s04");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
           // LoadingManager.Instance.LoadScene("s05");
            LoadingManager_lightmap.Instance.LoadScene("s05");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            // LoadingManager.Instance.LoadScene("s05");
            LoadingManager_lightmap.Instance.LoadScene("s06");
        }
    }
}
