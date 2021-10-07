using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DownloadedHandle(WWW loadedData);
public class WWWLoader : MonoBehaviour
{
    //存放当前下载的WWW包
    private Dictionary<string, WWW> loadedWWW_Dictionary = new Dictionary<string, WWW>();

    public int listCount = 0;
    void Update()
    {
        listCount = loadedWWW_Dictionary.Count;
    }
    #region 对外属性及接口
    /// <summary>
    /// 切换场景时调用此函数，释放此场景的WWW
    /// </summary>
    public void Init()
    {
        ReleaseAll();
    }
    [SerializeField]
    private bool isStreaming = true;//是否流式加载

    public bool IsStreaming
    {
        get
        {
            return isStreaming;
        }
        set
        {
            if (!isLoading)
            {
                isStreaming = value;
            }
        }
    }

    [SerializeField]
    private bool isLoading = false;

    /// <summary>
    /// 下载资源API，downloadedHandle为下载完毕事件，progressEventId为下载中update
    /// </summary>
    /// <param name="wwwPath"></param>
    /// <param name="downloadedHandle"></param>
    /// <param name="progressEventId"></param>
    public void Load_WWW(string wwwPath, DownloadedHandle downloadedHandle, ProgressDelegate progressEventId)
    {
        if (loadedWWW_Dictionary.ContainsKey(wwwPath))
        {
            Debug.Log(1);
            downloadedHandle(loadedWWW_Dictionary[wwwPath]);
        }
        else
        {
            if (isStreaming)
            {
                if (CallBackManager.GetCurrentNode() == null)
                {
                    StartCoroutine(CommonLoad(wwwPath, downloadedHandle, progressEventId, loadedWWW_Dictionary));
                }
                CallBackManager.AddLoadItem(wwwPath, downloadedHandle, progressEventId, true);
            }
            else
            {
                StartCoroutine(CommonLoad(wwwPath, downloadedHandle, progressEventId, loadedWWW_Dictionary));
            }
        }
    }
    private void LoadCurrentMsgNode()
    {
        WWWMsgNode node = CallBackManager.GetCurrentNode();

        if (node == null) return;

        StartCoroutine(CommonLoad(node.wwwPath, node.downloadedHandle, node.progressEventId, loadedWWW_Dictionary));
    }
    /// <summary>
    /// 释放对应的WWW
    /// </summary>
    /// <param name="wwwPath"></param>
    public void ReleaseWWW(string wwwPath)
    {
        if (loadedWWW_Dictionary.ContainsKey(wwwPath))
        {
            loadedWWW_Dictionary[wwwPath] = null;
            loadedWWW_Dictionary.Remove(wwwPath);
        }
    }
    /// <summary>
    /// 释放所有的WWW
    /// </summary>
    public void ReleaseAll()
    {
        if (loadedWWW_Dictionary != null)
        {
            string[] urls = new string[loadedWWW_Dictionary.Count];
            loadedWWW_Dictionary.Keys.CopyTo(urls, 0);
            for (int i = 0; i < urls.Length; i++)
            {
                loadedWWW_Dictionary[urls[i]] = null;
            }
            loadedWWW_Dictionary.Clear();
        }
    }
    #endregion

    #region 内置方法
    WWWNodeManager callBackManager = null;
    /// <summary>
    /// 流式加载的消息控制器
    /// </summary>
    WWWNodeManager CallBackManager
    {
        get
        {
            if (callBackManager == null)
            {
                callBackManager = new WWWNodeManager();
            }
            return callBackManager;
        }
    }

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="wwwPath"></param>
    /// <param name="downloadedHandle"></param>
    /// <param name="progressEventId"></param>
    /// <param name="loadedWWW_Dictionary"></param>
    /// <returns></returns>
    private IEnumerator CommonLoad(string wwwPath, DownloadedHandle downloadedHandle, ProgressDelegate progressEventId, Dictionary<string, WWW> loadedWWW_Dictionary)
    {
        isLoading = true;
        WWW wwwLoader = new WWW(wwwPath);
        while (!wwwLoader.isDone)
        {
            if (!string.IsNullOrEmpty(wwwLoader.error))
            {
                Debug.LogError(wwwLoader.error);
                break;
            }
            if (progressEventId != null)
            {
                progressEventId(wwwLoader.progress);
            }
            yield return wwwLoader.progress;
        }

        yield return null;

        if (!string.IsNullOrEmpty(wwwLoader.error))
        {
            Debug.LogError(wwwLoader.error);
        }
        else if (wwwLoader.isDone)
        {
            if (!isStreaming)
            {
                downloadedHandle(wwwLoader);
            }
            else
            {
                CallBack_LoadFinish(wwwLoader);
            }

            loadedWWW_Dictionary.Add(wwwPath, wwwLoader);
        }
        isLoading = false;
    }
    /// <summary>
    /// 下载完毕事件
    /// </summary>
    /// <param name="loader"></param>
    private void CallBack_LoadFinish(WWW loader)
    {

        CallBackManager.LoadedCallBack(loader);

        LoadCurrentMsgNode();
    }
    #endregion
}
/// <summary>
/// 下载的消息数据类
/// </summary>
public class WWWMsgNode
{
    public string wwwPath;

    public bool isLoading = false;

    public DownloadedHandle downloadedHandle;

    public ProgressDelegate progressEventId;
    public WWWMsgNode(string wwwPath, DownloadedHandle downloadedHandle, ProgressDelegate progressEventId, bool isloading)
    {
        this.wwwPath = wwwPath;
        this.downloadedHandle = downloadedHandle;
        this.progressEventId = progressEventId;
    }
    public void CallBack(WWW loader)
    {
        downloadedHandle(loader);
    }

    public void Dispose()
    {
        wwwPath = null;
        downloadedHandle = null;
        progressEventId = null;
    }

}
/// <summary>
/// 下载的消息管理类
/// </summary>
public class WWWNodeManager
{
    List<WWWMsgNode> manager = null;

    public WWWMsgNode GetCurrentNode()
    {
        if (manager != null && manager.Count > 0)
            return manager[0];

        return null;
    }

    public void AddLoadItem(string wwwPath, DownloadedHandle downloadedHandle, ProgressDelegate progressEventId, bool isloading)
    {
        if (manager == null)
            manager = new List<WWWMsgNode>();

        WWWMsgNode node = new WWWMsgNode(wwwPath, downloadedHandle, progressEventId, isloading);

        manager.Add(node);
    }

    public void LoadedCallBack(WWW loader)
    {
        if (manager != null && manager.Count > 0)
        {
            List<WWWMsgNode> nodeList = new List<WWWMsgNode>();

            string path = manager[0].wwwPath;

            for (int i = 0; i < manager.Count; i++)
            {
                if (i == 0 || manager[i].wwwPath == path)
                {
                    nodeList.Add(manager[i]);
                }
            }

            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].CallBack(loader);

                nodeList[i].Dispose();

                manager.Remove(nodeList[i]);
            }
        }
    }

}