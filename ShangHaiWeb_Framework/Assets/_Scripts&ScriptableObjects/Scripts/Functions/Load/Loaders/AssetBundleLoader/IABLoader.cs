using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//每一帧的回调
public delegate void LoaderProgrecess(string bundle, float progress, ProgressDelegate method);

public delegate void ProgressDelegate(float progress);
//load完回调
public delegate void LoadFinish(string bundle);
public class IABLoader
{
    private string bunldeName;
    private string commonBundlePath;
    //public static string commonBundlePath;
    private WWW commonLoader;
    private float commResLoaderProcess;
    //public static float commResLoaderProcess;
    private IABResLoader abResLoader;

    private LoaderProgrecess loadProgress;

    private ProgressDelegate progressDelegate;

    private LoadFinish loadFinish;

    public IABLoader(LoaderProgrecess tmpProgress, ProgressDelegate tmpDelegate, LoadFinish tmpFinish)
    {
        commonBundlePath = "";
        bunldeName = "";
        commResLoaderProcess = 0;
        abResLoader = null;

        loadProgress = tmpProgress;
        loadFinish = tmpFinish;
        progressDelegate = tmpDelegate;
    }
    //设置包名
    //scene1/test.prefab
    public void SetBundleName(string bundleName)
    {
        this.bunldeName = bundleName;
    }
    /// <summary>
    /// 要求上层传递完整路径
    /// </summary>
    /// <param name="path"></param>
    public void LoadResources(string path)
    {
        commonBundlePath = path;
    }
    //协程加载
    public IEnumerator CommonLoad()
    {
        commonLoader = new WWW(commonBundlePath);
        while (!commonLoader.isDone)
        {
            commResLoaderProcess = commonLoader.progress;

            if (loadProgress != null && commResLoaderProcess < 1)
            {
                loadProgress(bunldeName, commResLoaderProcess, progressDelegate);
            }

            yield return commonLoader.progress;

            commResLoaderProcess = commonLoader.progress;
        }
        if (commResLoaderProcess >= 1.0f)//加载完成
        {
            abResLoader = new IABResLoader(commonLoader.assetBundle);

            if (loadProgress != null)
            {

                loadProgress(bunldeName, commResLoaderProcess, progressDelegate);
            }

            if (loadFinish != null)
            {
                loadFinish(bunldeName);
            }
        }
        else
        {
            //MsgCenter.cube.GetComponent<Renderer>().material.color = Color.yellow;
            Debug.LogError("Load Bundle error ==" + bunldeName);
        }
        commonLoader = null;
    }

    #region 下层提供功能
    //Debug
    public void DebugerLoader()
    {
        if (commonLoader != null)
        {
            abResLoader.DebugAllRes();
        }
    }


    //获取单个资源
    public Object GetResources(string name)
    {
        if (abResLoader != null)
        {
            return abResLoader[name];
        }
        else return null;
    }
    //获取多个资源
    public Object[] GetMutiRes(string name)
    {
        if (abResLoader != null)
        {
            return abResLoader.LoadResources(name);
        }
        else return null;
    }
    //获取全部资源
    public Object[] GetAllRes()
    {
        if (abResLoader != null)
        {
            return abResLoader.LoadAll();
        }
        else return null;
    }
    //释放功能
    public void DisPose()
    {
        if (abResLoader != null)
        {
            //Debug.Log("4:"  );
            abResLoader.DebugAllRes();
            abResLoader.Dispose();
            abResLoader = null;
        }
    }

    //卸载单个资源
    public void UnLoadAssetRes(Object tmpObj)
    {
        if (abResLoader != null)
        {
            abResLoader.UnloadRes(tmpObj);
        }
    }
    #endregion
}
