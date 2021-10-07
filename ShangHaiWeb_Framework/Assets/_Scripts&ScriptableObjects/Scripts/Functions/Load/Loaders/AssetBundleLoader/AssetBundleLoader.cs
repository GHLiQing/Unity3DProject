using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine.UI;

public delegate void CallBackHandle(Object[] messages);

public delegate Object[] GetResourceHandler(params string[] tmpObj);

public enum LoadType
{
    Single,
    Muti,
    All
}
public class AssetBundleLoader : MonoBehaviour
{
    public bool isStreaming = true;

    private Dictionary<string, IABSceneManager> loadManager = new Dictionary<string, IABSceneManager>();
    void Awake()
    {
        //第一步 加载IABManifest
        StartCoroutine(IABManifestLoader.Instance.LoadMainfest());
    }

    void OnDestroy()
    {
        loadManager.Clear();
        System.GC.Collect();
    }

    NativeResCallBackManager callBackManager = null;

    NativeResCallBackManager CallBackManager
    {
        get
        {
            if (callBackManager == null)
            {
                callBackManager = new NativeResCallBackManager();
            }
            return callBackManager;
        }
    }

    #region 内置方法
    #region LoadCallBack
    /// <summary>
    /// 确定该bundle包没下载过之后，开始www下载
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    /// <param name="senceManagerDic"></param>
    private void LoadCallBack(string sceneName, string bundleName, Dictionary<string, IABSceneManager> senceManagerDic)
    {

        if (loadManager.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManager[sceneName];
            StartCoroutine(tmpManager.LoadAssetSys(bundleName, senceManagerDic));
        }
        else
        {
            Debug.Log("bundle name is not contain ==" + bundleName);
        }
    }

    /// <summary>
    /// 加载过程中执行的事件
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="progress"></param>
    /// <param name="progressDelegate"></param>
    private void LoaderProgress(string bundleName, float progress, ProgressDelegate progressDelegate)
    {
        if (!CallBackManager.isContainsBunlde(bundleName))
        {
            if (progressDelegate != null)
            {
                //此处这样写的原因是：当加载一个资源时，会先查看是否关联了其他bundle包里的资源，如果有则先下载其他包，最后下载包含资源的bundle包，再加载资源，这里-1就是指正在加载关联的bundle包
                progressDelegate(-1);
            }
            return;
        }

        if (progressDelegate != null)
        {
            progressDelegate(progress);
        }

        if (progress >= 1.0f)
        {
            //上层的回调
            StartCoroutine(CallBack_LoadFinish(bundleName));
        }
    }

    /// <summary>
    /// 加载完毕的callback
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    private IEnumerator CallBack_LoadFinish(string bundleName)
    {
        bool isUnloadedBundle = CallBackManager.isContainsBunlde(bundleName);

        yield return new WaitForSeconds(0.1f);

        CallBackManager.CallBackRes(bundleName);

        CallBackManager.Dispose(bundleName);

        if (isStreaming && isUnloadedBundle)
        {
            NativeResCallBackNode item = CallBackManager.GetloadingItem();

            if (item != null)
            {
                LoadAsset(item.loadType, item.sceneName, item.bundleName, item.resName, item.backMsgId, LoaderProgress, item.progressEventId);
            }
        }
    }

    #endregion
    #region Load相关方法
    //读取配置文件
    private void ReadConfiger(string sceneName)
    {
        if (!loadManager.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = new IABSceneManager(sceneName);

            //tmpManager.ReadConfiger(sceneName);
            tmpManager.ReadConfigerResources(sceneName);
            loadManager.Add(sceneName, tmpManager);
        }
    }

    //加载资源
    private void LoadAsset(LoadType loadType, string sceneName, string bundleName, string resName, CallBackHandle callbackEvent, LoaderProgrecess progress, ProgressDelegate tmpDelgegate)
    {
        IABSceneManager tmpManager = loadManager[sceneName];

        tmpManager.LoadAsset(loadType, bundleName, resName, callbackEvent, progress, LoadCallBack, tmpDelgegate, loadManager);
    }

    /// <summary>
    /// 判断是否下载完毕
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    private bool IsLoadingBundleFinish(string sceneName, string bundleName)
    {
        bool tmpBool = loadManager.ContainsKey(sceneName);
        if (tmpBool)
        {
            IABSceneManager tmpManager = loadManager[sceneName];
            return tmpManager.IsLoadingFinish(bundleName);
        }
        return false;
    }

    /// <summary>
    /// 判断是否下载过，正在下载也为true
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    private bool IsLoadingAssetBundle(string sceneName, string bundleName)
    {
        bool tmpBool = loadManager.ContainsKey(sceneName);
        if (tmpBool)
        {
            IABSceneManager tmpManager = loadManager[sceneName];
            return tmpManager.IsLoadingAssetBundle(bundleName);
        }
        return false;
    }

    private string GetBundleRelateName(string sceneName, string bundleName)
    {
        IABSceneManager tmpManager = loadManager[sceneName];
        if (tmpManager != null)
        {
            return tmpManager.GetBundleRelateName(bundleName);
        }
        return null;
    }


    /// <summary>
    /// 从下载好的包中读取资源 单个
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    private Object[] GetSingleResources(params string[] loadDatas)
    {
        if (loadManager.ContainsKey(loadDatas[0]))
        {

            IABSceneManager tmpManager = loadManager[loadDatas[0]];
            return new Object[] { tmpManager.GetSingleResources(loadDatas[1], loadDatas[2]) };
        }
        else
        {
            Debug.Log("sceneName ==" + loadDatas[0] + " bundleName ==" + loadDatas[1] + " is not load");
            return null;
        }
    }


    /// <summary>
    /// 从下载好的包中读取资源 多个
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    private Object[] GetMutiResources(params string[] loadDatas)
    {
        if (loadManager.ContainsKey(loadDatas[0]))
        {
            IABSceneManager tmpManager = loadManager[loadDatas[0]];

            return tmpManager.GetMutiResources(loadDatas[1], loadDatas[2]);
        }
        else
        {
            Debug.Log("sceneName ==" + loadDatas[0] + " bundleName ==" + loadDatas[1] + " is not load");
            return null;
        }
    }
    private Object[] GetAllResources(params string[] loadDatas)
    {
        if (loadManager.ContainsKey(loadDatas[0]))
        {
            IABSceneManager tmpManager = loadManager[loadDatas[0]];

            return tmpManager.GetAllResources(loadDatas[1]);
        }
        else
        {
            Debug.Log("sceneName ==" + loadDatas[0] + " bundleName ==" + loadDatas[1] + " is not load");
            return null;
        }
    }
    #endregion
    #endregion

    #region 对外接口
    //释放资源
    /// <summary>
    /// 释放某一个资源
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    /// <param name="res"></param>
    public void ReleaseSingleObj(string sceneName, string bundleName, string res)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeResObj(bundleName, res);
        }
    }

    /// <summary>
    /// 释放整个Bundle包的资源
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    public void ReleaseBundleObj(string sceneName, string bundleName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeBundleRes(bundleName);
        }
    }

    /// <summary>
    /// 释放整个场景的objects
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    public void ReleaseSceneObj(string sceneName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeAllRes();
        }
    }

    /// <summary>
    /// 释放一个bundle
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    public void ReleaseSingleBundle(string sceneName, string bundleName)
    {
        //string tmpValue = allAssets[bundleName];
        if (loadManager.ContainsKey(sceneName))
        {
            //Debug.Log("1:" + bundleName);
            IABSceneManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeBundle(bundleName);
            System.GC.Collect();
        }
    }

    /// <summary>
    /// 释放该场景所有bundle
    /// </summary>
    /// <param name="sceneName"></param>
    public void ReleaseSceneBundles(string sceneName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeAllBundle();
            System.GC.Collect();
        }
    }

    /// <summary>
    /// 释放该场景的所有bundle和加载的资源
    /// </summary>
    /// <param name="sceneName"></param>
    public void ReleaseAll(string sceneName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeAllBundleAndRes();
            System.GC.Collect();
        }
    }

    /// <summary>
    /// Debug所有已下载的包
    /// </summary>
    /// <param name="sceneName"></param>
    public void DebugAllAssetBundle(string sceneName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSceneManager tmpManager = loadManager[sceneName];

            tmpManager.DebugAllAsset();
        }
    }

    /// <summary>
    /// 下载资源
    /// </summary>
    /// <param name="isSingle"></param>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    /// <param name="resName"></param>
    /// <param name="callbackEvent"></param>
    /// <param name="progressDelegate"></param>
    public void LoadAssetBundle(LoadType loadType, string sceneName, string bundleName, string resName, CallBackHandle callbackEvent, ProgressDelegate progressDelegate)
    {
        //Debug.Log("scene: " + sceneName + "  bundleName: " + bundleName + "  resName: " + resName + " isLoading: " + ILoaderManager.Instance.IsLoadingAssetBundle(sceneName, bundleName) + "isloaded: " + ILoaderManager.Instance.IsLoadingBundleFinish(sceneName, bundleName));
        //没有加载
        if (!IsLoadingAssetBundle(sceneName, bundleName))
        {
            if (!loadManager.ContainsKey(sceneName))
            {
                ReadConfiger(sceneName);
            }

            if (!isStreaming)
            {
                //没有下载的话，开始下载该bundle包
                LoadAsset(loadType, sceneName, bundleName, resName, callbackEvent, LoaderProgress, progressDelegate);
            }
            else if (CallBackManager.GetLoadingCount() == 0)
            {
                LoadAsset(loadType, sceneName, bundleName, resName, callbackEvent, LoaderProgress, progressDelegate);
            }

            AddtoCallBackManager(CallBackManager, loadType, sceneName, bundleName, resName, callbackEvent, progressDelegate);
        }
        //表示已经加载完成
        else if (IsLoadingBundleFinish(sceneName, bundleName))
        {
            switch (loadType)
            {
                case LoadType.Single:

                    Object tmpObj = GetSingleResources(sceneName, bundleName, resName)[0];

                    callbackEvent(new Object[] { tmpObj });

                    break;

                case LoadType.Muti:

                    Object[] tmpObjs = GetMutiResources(sceneName, bundleName, resName);

                    callbackEvent(tmpObjs);

                    break;

                case LoadType.All:

                    Object[] allTmpObjs = GetAllResources(sceneName, bundleName);

                    callbackEvent(allTmpObjs);

                    break;
            }
        }
        //已经加载但没有加载完成
        else
        {
            AddtoCallBackManager(CallBackManager, loadType, sceneName, bundleName, resName, callbackEvent, progressDelegate);
        }
    }
    #endregion
    private void AddtoCallBackManager(NativeResCallBackManager CallBackManager, LoadType loadType, string sceneName, string bundleName, string resName, CallBackHandle callbackEvent, ProgressDelegate progressDelegate)
    {
        string bundleFullName = GetBundleRelateName(sceneName, bundleName);

        if (bundleFullName != null)
        {
            GetResourceHandler handler = null;

            switch (loadType)
            {
                case LoadType.Single:
                    handler += GetSingleResources;
                    break;
                case LoadType.Muti:
                    handler += GetMutiResources;
                    break;
                case LoadType.All:
                    handler += GetAllResources;
                    break;
            }

            NativeResCallBackNode tmpNode = new NativeResCallBackNode(loadType, handler, sceneName, bundleName, resName, callbackEvent, progressDelegate, null);

            CallBackManager.AddBundle(bundleFullName, tmpNode);
        }
        else
        {
            Debug.LogWarning("Do not contain bundle == " + bundleName);
        }
    }

}

public class NativeResCallBackNode
{
    public LoadType loadType;

    public string sceneName;

    public string bundleName;

    public string resName;

    public CallBackHandle backMsgId;

    public ProgressDelegate progressEventId;

    public NativeResCallBackNode nextValue;

    public GetResourceHandler GetResource;
    public NativeResCallBackNode(LoadType loadType, GetResourceHandler getResourceHandle, string tmpSceneName, string tmpBundle, string tmpRes, CallBackHandle callbackEvent, ProgressDelegate progressEventId, NativeResCallBackNode tmpNode)
    {
        this.loadType = loadType;


        this.GetResource = getResourceHandle;

        this.sceneName = tmpSceneName;

        this.bundleName = tmpBundle;

        this.resName = tmpRes;

        this.backMsgId = callbackEvent;

        this.progressEventId = progressEventId;

        this.nextValue = tmpNode;
    }
    public void Dispose()
    {
        nextValue = null;
        this.sceneName = null;
        this.bundleName = null;
        this.resName = null;
        this.GetResource = null;
        this.backMsgId = null;
        this.progressEventId = null;

    }
    public void CallBack()
    {
        Object[] tmpObjs = GetResource(sceneName, bundleName, resName);
        backMsgId(tmpObjs);
    }
}

public class NativeResCallBackManager
{
    Dictionary<string, NativeResCallBackNode> manager = null;

    public void DebugAllBundle()
    {
        foreach (var item in manager)
        {
            Debug.Log("name: " + item.Key);
        }
    }

    public int GetLoadingCount()
    {
        return manager.Count;
    }

    public bool isContainsBunlde(string bundleName)
    {
        return manager.ContainsKey(bundleName);
    }

    public NativeResCallBackNode GetloadingItem()
    {
        if (GetLoadingCount() > 0)
        {
            foreach (var item in manager)
            {
                return item.Value;
            }
        }
        return null;
    }

    public NativeResCallBackManager()
    {
        manager = new Dictionary<string, NativeResCallBackNode>();
    }

    /// <summary>
    /// 来了请求添加的过程
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="currentNode"></param>
    public void AddBundle(string bundle, NativeResCallBackNode currentNode)
    {
        if (manager.ContainsKey(bundle))
        {
            NativeResCallBackNode tmpNode = manager[bundle];
            while (tmpNode.nextValue != null)
            {
                tmpNode = tmpNode.nextValue;
            }
            tmpNode.nextValue = currentNode;
        }
        else
        {
            manager.Add(bundle, currentNode);
        }
    }

    /// <summary>
    /// 加载完后 消息向上层传递完成了 就把这些缓存的命令删除
    /// </summary>
    /// <param name="bundle"></param>
    public void Dispose(string bundle)
    {
        if (manager.ContainsKey(bundle))
        {
            NativeResCallBackNode tmpNode = manager[bundle];

            while (tmpNode.nextValue != null)
            {
                NativeResCallBackNode curNode = tmpNode;
                tmpNode = tmpNode.nextValue;
                curNode.Dispose();
            }

            tmpNode.Dispose();

            manager.Remove(bundle);
        }
    }

    public void CallBackRes(string bundle)
    {
        if (manager.ContainsKey(bundle))
        {

            NativeResCallBackNode tmpNode = manager[bundle];

            do
            {
                tmpNode.CallBack();
                tmpNode = tmpNode.nextValue;
            }
            while (tmpNode != null);
        }
    }
}