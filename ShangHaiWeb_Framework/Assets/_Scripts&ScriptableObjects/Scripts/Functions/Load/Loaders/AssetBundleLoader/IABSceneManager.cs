using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
public class IABSceneManager
{
    IABManager abManager;
    public IABSceneManager(string sceneName)
    {
        abManager = new IABManager(sceneName);
    }

    private Dictionary<string, string> allAssets = new Dictionary<string, string>();



    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">场景名</param>
    public void ReadConfiger(string sceneName)
    {
        string textFileName = "Record.txt";
        //
        string path = IPathTools.GetAssetBundlePath() + "/" + sceneName + textFileName;

        ReadConfig(path);
    }

    public void ReadConfigerResources(string sceneName)
    {
        string textFileName = "Record";
        string path = "AssetRecord/" + sceneName + textFileName;
        TextAsset content = Resources.Load(path) as TextAsset;
        if (content == null)
        {
            Debug.Log("not contains this scene:" + sceneName);
            return;
        }
        byte[] array = Encoding.ASCII.GetBytes(content.text);
        MemoryStream stream = new MemoryStream(array);
        StreamReader br = new StreamReader(stream);
        string line = br.ReadLine();
        int allCount = int.Parse(line);
        for (int i = 0; i < allCount; i++)
        {
            string tmpStr = br.ReadLine();
            string[] tmpArr = tmpStr.Split(" ".ToCharArray());
            allAssets.Add(tmpArr[0], tmpArr[1]);
            //Debug.Log(tmpArr[0] + " |||" + tmpArr[1]);

        }
        br.Close();

    }
    private void ReadConfig(string path)
    {
        Debug.Log(path);
        FileStream fs = new FileStream(path, FileMode.Open);
        StreamReader br = new StreamReader(fs);
        string line = br.ReadLine();
        int allCount = int.Parse(line);
        for (int i = 0; i < allCount; i++)
        {
            string tmpStr = br.ReadLine();
            string[] tmpArr = tmpStr.Split(" ".ToCharArray());
            allAssets.Add(tmpArr[0], tmpArr[1]);
            Debug.Log(tmpArr[0] + " |||" + tmpArr[1]);
        }
        br.Close();
        fs.Close();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bundleName">Load</param>
    /// <param name="progress"></param>
    /// <param name="Load"></param>
    public void LoadAsset(LoadType loadType, string bundleName, string resName, CallBackHandle callbackEvent, LoaderProgrecess progress, LoadAssetBundleCallBack Load, ProgressDelegate tmpDelegate, Dictionary<string, IABSceneManager> senceManagerDic)
    {
        if (allAssets.ContainsKey(bundleName))
        {
            string tmpValue = allAssets[bundleName];

            abManager.LoadAssetBundle(loadType, tmpValue, resName, callbackEvent, progress, Load, tmpDelegate, senceManagerDic);
        }
        else
        {
            Debug.Log("Dont contain the bundle ==" + bundleName);
        }
    }
    #region 由下层提供功能
    public Dictionary<string, IABRelationManager> GetLoadHelperList()
    {
        return abManager.GetLoadHelperList();
    }
    public Dictionary<string, AssetResObj> GetLoadedAsset()
    {
        return abManager.GetLoadedAsset();
    }
    public string GetBundleRelateName(string bundleName)
    {
        if (allAssets.ContainsKey(bundleName))
        {
            return allAssets[bundleName];
        }
        else
        {
            return null;
        }
    }
    public IEnumerator LoadAssetSys(string bundleName, Dictionary<string, IABSceneManager> senceManagerDic)
    {
        yield return abManager.LoadAssetBundles(bundleName, senceManagerDic);
    }

    public Object GetSingleResources(string bundleName, string resName)
    {
        if (allAssets.ContainsKey(bundleName))
        {
            return abManager.GetSingleResource(allAssets[bundleName], resName);
        }
        else
        {
            Debug.Log("Dont contain the bundle ==" + bundleName);
            return null;
        }
    }
    public Object[] GetMutiResources(string bundleName, string resName)
    {
        if (allAssets.ContainsKey(bundleName))
        {
            return abManager.GetMutiResource(allAssets[bundleName], resName);
        }
        else
        {
            Debug.Log("Dont contain the bundle ==" + bundleName);
            return null;
        }
    }
    public Object[] GetAllResources(string bundleName)
    {
        if (allAssets.ContainsKey(bundleName))
        {
            return abManager.GetAllResource(allAssets[bundleName]);
        }
        else
        {
            Debug.Log("Dont contain the bundle ==" + bundleName);
            return null;
        }
    }
    /// <summary>
    /// 释放单个资源
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="res"></param>
    public void DisposeResObj(string bundleName, string res)//!!!!!!!!!!!!!!!
    {
        if (allAssets.ContainsKey(bundleName))
        {
            abManager.DisposeResObj(allAssets[bundleName], res);
        }
        else
        {
            Debug.Log("Dont contain the bundle ==" + bundleName);
        }
    }
    /// <summary>
    /// 释放一个bundle包的资源
    /// </summary>
    /// <param name="bundleName"></param>
    public void DisposeBundleRes(string bundleName)//!!!!!!!!!!!!
    {
        if (allAssets.ContainsKey(bundleName))
        {
            abManager.DisposeResObj(allAssets[bundleName]);
        }
        else
        {
            Debug.Log("Dont contain the bundle ==" + bundleName);
        }
    }
    /// <summary>
    /// 卸载所有资源
    /// </summary>
    public void DisposeAllRes()
    {
        abManager.DisposeAllObj();
    }

    public void DisposeBundle(string bundleName)
    {
        if (allAssets.ContainsKey(bundleName))
        {
            //Debug.Log("2:" + bundleName);

            abManager.DisposeBundle(allAssets[bundleName]);
        }
    }
    public void DisposeAllBundle()
    {
        abManager.DisposeAllBundle();
    }
    public void DisposeAllBundleAndRes()
    {
        abManager.DisposeAllBundleAndRes();
    }

    public void DebugAllAsset()
    {
        List<string> keys = new List<string>();
        keys.AddRange(allAssets.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            abManager.DebugAssetBundle(allAssets[keys[i]]);
        }
    }
    //sceneOne/test.ld
    //bundleName=test
    public bool IsLoadingFinish(string bundleName)
    {
        if (allAssets.ContainsKey(bundleName))
        {
            return abManager.isLoadingFinish(allAssets[bundleName]);
        }
        else
        {
            Debug.Log("is not contain bundle ==" + bundleName);
        }
        return false;
    }
    public bool IsLoadingAssetBundle(string bundleName)
    {
        if (allAssets.ContainsKey(bundleName))
        {
            return abManager.IsLoadingAssetBundle(allAssets[bundleName]);
        }
        else
        {
            Debug.Log("is not contain bundle ==" + bundleName);
        }
        return false;
    }
    #endregion


}
