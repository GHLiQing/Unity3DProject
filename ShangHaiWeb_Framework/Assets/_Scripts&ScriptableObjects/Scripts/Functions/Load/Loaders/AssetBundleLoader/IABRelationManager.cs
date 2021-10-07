using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class IABRelationManager
{
    /// <summary>
    ///           --------->yy
    ///       xx                  xx  dependence yy AA
    ///           -------->AA
    /// </summary>
    List<string> depedenceBundle;

    /// <summary>
    ///   表示 yy aa   refer  xx
    /// </summary>
    List<string> referBundle;

    IABLoader assetLoader;

    LoaderProgrecess loaderProgress;

    ProgressDelegate progressDelegate;

    string theBundleName;

    public IABRelationManager()
    {
        depedenceBundle = new List<string>();
        referBundle = new List<string>();
    }
    //添加 ref 关系
    public void AddRefference(string bundleName)
    {
        referBundle.Add(bundleName);
    }
    //获取 ref 关系
    public List<string> GetRefference()
    {
        return referBundle;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns>表示是否释放自己</returns>
    public bool RemoveRefference(string bundleName)
    {
        for (int i = 0; i < referBundle.Count; i++)
        {
            if (bundleName.Equals(referBundle[i]))
            {
                referBundle.RemoveAt(i);
            }
        }
        if (referBundle.Count <= 0)
        {
            Dispose();
            return true;
        }
        return false;
    }

    public void SetDepedences(string[] depence)
    {
        if (depence.Length > 0)
        {
            depedenceBundle.AddRange(depence);
        }
    }

    public List<string> GetDepedence()
    {
        return depedenceBundle;
    }

    public void RemoveDepence(string bundleName)
    {
        for (int i = 0; i < depedenceBundle.Count; i++)
        {
            if (bundleName.Equals(depedenceBundle[i]))
            {
                depedenceBundle.RemoveAt(i);
            }
        }
    }
    bool isLoadFinish;
    public void BundleLoadFinish(string bundleName)
    {
        isLoadFinish = true;
    }
    public bool isBundleLoadFinish()
    {
        return isLoadFinish;
    }

    public string GetBundleName()
    {
        return theBundleName;
    }

    public void Initial(string bundle, LoaderProgrecess progress, ProgressDelegate tmpDelegate)
    {
        isLoadFinish = false;

        theBundleName = bundle;

        loaderProgress = progress;

        progressDelegate = tmpDelegate;

        assetLoader = new IABLoader(progress, tmpDelegate, BundleLoadFinish);

        //设置包名
        //scene1/test.prefab
        assetLoader.SetBundleName(bundle);

        string bundlePath = IPathTools.GetWWWAssetBundlePath() + "/" + bundle;

        assetLoader.LoadResources(bundlePath);
    }
    public LoaderProgrecess GetProgress()
    {
        return loaderProgress;
    }
    public ProgressDelegate GetProgressDelegate()
    {
        return progressDelegate;
    }
    #region 由下层提供API
    public void DebugerAsset()
    {
        if (assetLoader != null)
        {
            assetLoader.DebugerLoader();
        }
        else
        {
            Debug.Log("asset load is null");
        }
    }

    // unity3d 5.3以上 协程 才可以
    public IEnumerator LoadAssetBundle()
    {
        yield return assetLoader.CommonLoad();
    }

    // 释放 过程
    public void Dispose()
    {
        //Debug.Log("4:");

        assetLoader.DisPose();
    }

    public Object GetSingleResource(string bundleName)
    {
        //assetLoader.DebugerLoader();
        //Debug.Log("assetLoader  " + );
        return assetLoader.GetResources(bundleName);
    }

    public Object[] GetMutiResources(string bundleName)
    {
        return assetLoader.GetMutiRes(bundleName);
    }

    public Object[] GetAllResources()
    {
        return assetLoader.GetAllRes();
    }
    #endregion
}
