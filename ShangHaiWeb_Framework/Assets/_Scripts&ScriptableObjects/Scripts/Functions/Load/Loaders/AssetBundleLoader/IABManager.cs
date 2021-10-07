using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public delegate void LoadAssetBundleCallBack(string sceneName, string bundleNmae, Dictionary<string, IABSceneManager> senceManagerDic);
/// <summary>
/// 管理一个场景的所有bundle包
/// </summary>
public class IABManager
{
    //把每个包都存起来
    Dictionary<string, IABRelationManager> loadHelper = new Dictionary<string, IABRelationManager>();
    Dictionary<string, AssetResObj> loadedObjsList = new Dictionary<string, AssetResObj>();
    public IABManager(string tmpSceneName)
    {
        sceneName = tmpSceneName;
    }
    string sceneName;

    /// <summary>
    ///  表示是否加载bundle
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public bool IsLoadingAssetBundle(string bundleName)
    {
        if (!loadHelper.ContainsKey(bundleName))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #region 释放缓存物体
    public void DisposeResObj(string bundleName, string resName)
    {
        if (loadedObjsList.ContainsKey(bundleName))
        {
            AssetResObj tmpObj = loadedObjsList[bundleName];
            tmpObj.ReleaseResObj(resName);
        }

    }

    public void DisposeResObj(string bundleName)
    {
        if (loadedObjsList.ContainsKey(bundleName))
        {
            AssetResObj tmpObj = loadedObjsList[bundleName];
            tmpObj.ReleaseAllResObj();
            loadedObjsList.Remove(bundleName);
        }

        Resources.UnloadUnusedAssets();
    }

    public void DisposeAllObj()
    {
        List<string> keys = new List<string>();
        keys.AddRange(loadedObjsList.Keys);
        for (int i = 0; i < loadedObjsList.Count; i++)
        {
            DisposeResObj(keys[i]);
        }
        loadedObjsList.Clear();
    }

    public void DisposeBundle(string bundleName)
    {

        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];
            List<string> depences = loader.GetDepedence();
            for (int i = 0; i < depences.Count; i++)
            {
                if (loadHelper.ContainsKey(depences[i]))
                {
                    IABRelationManager depedence = loadHelper[depences[i]];
                    if (depedence.RemoveRefference(bundleName))
                    {
                        DisposeBundle(depedence.GetBundleName());
                    }
                }
            }
            if (loader.GetRefference().Count <= 0)
            {
                loader.Dispose();

                loadHelper.Remove(bundleName);
            }
        }
    }
    public void DisposeAllBundle()
    {
        List<string> keys = new List<string>();
        keys.AddRange(loadHelper.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            IABRelationManager loader = loadHelper[keys[i]];
            loader.Dispose();
        }
        loadHelper.Clear();
    }
    public void DisposeAllBundleAndRes()
    {
        DisposeAllObj();
        DisposeAllBundle();
    }
    #endregion
    string[] GetDependences(string bundleName)
    {
        return IABManifestLoader.Instance.GetDepences(bundleName);
    }
    //对外接口
    public void LoadAssetBundle(LoadType loadType, string bundleName, string resName, CallBackHandle callbackEvent, LoaderProgrecess progress, LoadAssetBundleCallBack Load, ProgressDelegate tmpDelegate, Dictionary<string, IABSceneManager> senceManagerDic)
    {

        if (!loadHelper.ContainsKey(bundleName))
        {
            #region  检测其他场景的bundle清单，如果有该bundle包，直接加载资源
            Dictionary<string, IABRelationManager> loadedList = GetisLoadedRelationList(senceManagerDic, bundleName);
            if (loadedList != null)
            {
                IABRelationManager loader_Other = loadedList[bundleName];
                switch (loadType)
                {
                    case LoadType.Single:
                        Object tmpObj = loader_Other.GetSingleResource(resName);
                        callbackEvent(new Object[] { tmpObj });
                        break;
                    case LoadType.Muti:
                        Object[] tmpObjs = loader_Other.GetMutiResources(resName);
                        callbackEvent(tmpObjs);
                        break;
                    case LoadType.All:
                        Object[] alltmpObjs = loader_Other.GetAllResources();
                        callbackEvent(alltmpObjs);
                        break;
                }
                return;
            }
            #endregion

            //未下载过该包，下载该bundle包
            IABRelationManager loader = new IABRelationManager();
            loader.Initial(bundleName, progress, tmpDelegate);

            loadHelper.Add(bundleName, loader);
            Load(sceneName, bundleName, senceManagerDic);
        }
        else
        {
            Debug.Log("IABManager have contained bundle name ==" + bundleName);
        }
    }
    Dictionary<string, IABRelationManager> GetisLoadedRelationList(Dictionary<string, IABSceneManager> senceManagerDic, string bundleName)
    {
        foreach (var scenemanager in senceManagerDic)
        {
            string sceneName = scenemanager.Key;

            Dictionary<string, IABRelationManager> loaded = scenemanager.Value.GetLoadHelperList();

            if (loaded.ContainsKey(bundleName))
            {
                return loaded;
            }
        }
        return null;
    }
    Dictionary<string, AssetResObj> GetisLoadedList(Dictionary<string, IABSceneManager> senceManagerDic, string bundleName)
    {
        foreach (var scenemanager in senceManagerDic)
        {
            string sceneName = scenemanager.Key;
            Dictionary<string, IABRelationManager> loaded = scenemanager.Value.GetLoadHelperList();
            if (loaded.ContainsKey(bundleName))
            {
                return scenemanager.Value.GetLoadedAsset();
            }
        }
        return null;
    }
    public IEnumerator LoadAssetBundleDepedences(string bundleName, string refName, LoaderProgrecess progress, ProgressDelegate tmpDelegate, Dictionary<string, IABSceneManager> senceManagerDic)
    {
        Dictionary<string, IABRelationManager> loadedList = GetisLoadedRelationList(senceManagerDic, bundleName);


        if (loadedList == null)
        //if (!loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = new IABRelationManager();
            loader.Initial(bundleName, progress, tmpDelegate);
            if (refName != null)
            {
                loader.AddRefference(refName);
            }
            loadHelper.Add(bundleName, loader);
            yield return LoadAssetBundles(bundleName, senceManagerDic);
        }
        else
        {
            if (refName != null)
            {
                IABRelationManager loader = loadedList[bundleName];
                loader.AddRefference(bundleName);
            }
        }
    }
    /// <summary>
    /// 加载assetbundle 必须先加载 manifest
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public IEnumerator LoadAssetBundles(string bundleName, Dictionary<string, IABSceneManager> senceManagerDic)
    {
        //Debug.Log("Load: " + bundleName);
        while (!IABManifestLoader.Instance.IsLoadFinish())
        {
            yield return null;
        }
        IABRelationManager loader = loadHelper[bundleName];
        string[] depences = GetDependences(bundleName);
        loader.SetDepedences(depences);

        for (int i = 0; i < depences.Length; i++)
        {
            yield return LoadAssetBundleDepedences(depences[i], bundleName, loader.GetProgress(), loader.GetProgressDelegate(), senceManagerDic);
        }

        yield return loader.LoadAssetBundle();
    }

    #region 由下层提供API

    public Dictionary<string, IABRelationManager> GetLoadHelperList()
    {
        return loadHelper;
    }

    public Dictionary<string, AssetResObj> GetLoadedAsset()
    {
        return loadedObjsList;

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bundleName">scene1/test.prefab</param>
    public void DebugAssetBundle(string bundleName)
    {
        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];
            loader.DebugerAsset();
        }
    }

    public bool isLoadingFinish(string bundleName)
    {
     //   Debug.Log(bundleName + " " + loadHelper.Count + " " + sceneName);

        foreach (var pair in loadHelper)
        {
         //   Debug.Log(bundleName + " " + pair.Key);
        }
        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];
            return loader.isBundleLoadFinish();
        }
        else
        {
            Debug.Log("IABRelation no contain bundle ==" + bundleName);
            return false;
        }
    }

    public Object GetSingleResource(string bundleName, string resName)
    {
        //表示是否已经缓存了物体
        if (loadedObjsList.ContainsKey(bundleName))
        {
            AssetResObj tmpRes = loadedObjsList[bundleName];

            List<Object> tmpObj = tmpRes.GetResObj(resName);
            if (tmpObj != null)
            {
                return tmpObj[0];
            }
        }
        //表示已经加载过bundle
        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];
            Object tmpObj = loader.GetSingleResource(resName);
            AssetObj tmpAssetObj = new AssetObj(tmpObj);
            //缓存中是否已经有这个包
            if (loadedObjsList.ContainsKey(bundleName))
            {
                AssetResObj tmpRes = loadedObjsList[bundleName];
                tmpRes.AddResObj(resName, tmpAssetObj);
            }
            else
            {
                //没有加载过这个包
                AssetResObj tmpRes = new AssetResObj(resName, tmpAssetObj);
                loadedObjsList.Add(bundleName, tmpRes);
            }
            return tmpObj;
        }
        else
        {
            return null;
        }
    }
    public Object[] GetMutiResource(string bundleName, string resName)
    {
        //表示是否已经缓存了物体
        if (loadedObjsList.ContainsKey(bundleName))
        {
            AssetResObj tmpRes = loadedObjsList[bundleName];
            List<Object> tmpObj = tmpRes.GetResObj(resName);
            if (tmpObj != null)
            {
                return tmpObj.ToArray();
            }
        }

        //表示已经加载过bundle
        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];
            Object[] tmpObj = loader.GetMutiResources(resName);
            AssetObj tmpAssetObj = new AssetObj(tmpObj);
            //缓存中是否已经有这个包
            if (loadedObjsList.ContainsKey(bundleName))
            {
                AssetResObj tmpRes = loadedObjsList[bundleName];
                tmpRes.AddResObj(resName, tmpAssetObj);
            }
            else
            {
                //没有加载过这个包
                AssetResObj tmpRes = new AssetResObj(resName, tmpAssetObj);
                loadedObjsList.Add(bundleName, tmpRes);
            }
            return tmpObj;
        }
        else
        {
            return null;
        }
    }
    public Object[] GetAllResource(string bundleName)
    {
        List<Object> objs = new List<Object>();

        //表示是否已经缓存了物体
        if (loadedObjsList.ContainsKey(bundleName))
        {
            AssetResObj tmpRes = loadedObjsList[bundleName];

            List<Object> tmpObj = tmpRes.GetAllObj();

            if (tmpObj != null)
            {
                objs.AddRange(tmpObj);
                return objs.ToArray();
            }
        }

        //表示已经加载过bundle
        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];

            Object[] tmpObj = loader.GetAllResources();

            AssetObj[] tmpAssetObjs = new AssetObj[tmpObj.Length];

            for (int i = 0; i < tmpObj.Length; i++)
            {
                tmpAssetObjs[i] = new AssetObj(tmpObj[i]);
            }


            //缓存中是否已经有这个包
            if (loadedObjsList.ContainsKey(bundleName))
            {
                AssetResObj tmpRes = loadedObjsList[bundleName];
                for (int i = 0; i < tmpAssetObjs.Length; i++)
                {
                    tmpRes.AddResObj(tmpAssetObjs[i].objs[0].name, tmpAssetObjs[i]);
                }
            }
            else
            {
                //没有加载过这个包
                AssetResObj tmpRes = new AssetResObj();

                for (int i = 0; i < tmpAssetObjs.Length; i++)
                {
                    tmpRes.AddResObj(tmpAssetObjs[i].objs[0].name, tmpAssetObjs[i]);
                }

                loadedObjsList.Add(bundleName, tmpRes);
            }

            objs.AddRange(tmpObj);

            return objs.ToArray();
        }
        else if (objs.Count > 0)
        {
            return objs.ToArray();
        }
        else
        {
            return null;
        }
    }

    #endregion
}
//单个obj
public class AssetObj
{
    public List<Object> objs;
    public AssetObj(params Object[] tmpObj)
    {
        objs = new List<Object>();
        objs.AddRange(tmpObj);
    }
    public void ReleaseObj()
    {
        for (int i = 0; i < objs.Count; i++)
        {
            if (!objs[i].GetType().ToString().Contains("GameObject"))
            {
                Resources.UnloadAsset(objs[i]);
            }
        }
    }
}
//存的是一个bundle包里的obj
public class AssetResObj
{
    public Dictionary<string, AssetObj> resObjs;
    public AssetResObj(string name, AssetObj tmp)
    {
        resObjs = new Dictionary<string, AssetObj>();
        resObjs.Add(name, tmp);
    }
    public AssetResObj()
    {
        resObjs = new Dictionary<string, AssetObj>();
    }
    //添加
    public void AddResObj(string name, AssetObj tmpObj)
    {
        if (!resObjs.ContainsKey(name))
        {
            resObjs.Add(name, tmpObj);
        }
    }
    //释放多个
    public void ReleaseAllResObj()
    {
        List<string> keys = new List<string>();
        keys.AddRange(resObjs.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            ReleaseResObj(keys[i]);
        }
    }
    //释放单个
    public void ReleaseResObj(string name)
    {
        if (resObjs.ContainsKey(name))
        {
            AssetObj tmpObj = resObjs[name];
            tmpObj.ReleaseObj();
        }
        else
        {
            Debug.Log("release Object name is not exit ==" + name);
        }
    }

    public List<Object> GetResObj(string name)
    {
        if (resObjs.ContainsKey(name))
        {
            AssetObj tmpObj = resObjs[name];
            return tmpObj.objs;
        }
        else return null;
    }
    public List<Object> GetAllObj()
    {

        if (resObjs != null && resObjs.Count > 0)
        {
            List<AssetObj> objList = new List<AssetObj>();

            objList.AddRange(resObjs.Values);

            Debug.Log("RangeCount: " + objList.Count);

            List<Object> objs = new List<Object>();

            for (int i = 0; i < objList.Count; i++)
            {
                objs.AddRange(objList[i].objs);
            }

            return objs;
        }

        return null;
    }

}
