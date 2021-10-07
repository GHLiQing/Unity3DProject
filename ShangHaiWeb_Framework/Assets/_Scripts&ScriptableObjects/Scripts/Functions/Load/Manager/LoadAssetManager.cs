using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class LoadAssetManager : Singleton<LoadAssetManager>
{
    public override void Init()
    {
        base.Init();

        if (m_WWWLoader != null)
            m_WWWLoader.Init();
    }
    #region AssetBundle部分所有对外接口
    AssetBundleLoader assetBundleLoader;
    public AssetBundleLoader m_AssetBundleLoader
    {
        get
        {
            if (assetBundleLoader == null)
            {
                assetBundleLoader = GetComponent<AssetBundleLoader>();

                if (assetBundleLoader == null)
                {
                    assetBundleLoader = gameObject.AddComponent<AssetBundleLoader>();
                }
            }
            return assetBundleLoader;
        }
    }
    /// <summary>
    /// 是否流式加载Bundle包
    /// </summary>
    public bool isStreaming_AssetBundle
    {
        get { return m_AssetBundleLoader.isStreaming; }
        set { m_AssetBundleLoader.isStreaming = value; }
    }
    /// <summary>
    /// 加载bundle包
    /// </summary>
    /// <param name="isSingle">是否为单个资源，多个资源如切割的sprite</param>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    /// <param name="resName"></param>
    /// <param name="callbackEvent">加载完毕执行的事件</param>
    /// <param name="progressDelegate">加载中的update</param>
    public void LoadAssetBundle(LoadType loadType, string sceneName, string bundleName, string resName, CallBackHandle callbackEvent, ProgressDelegate progressDelegate)
    {
        m_AssetBundleLoader.LoadAssetBundle(loadType, sceneName, bundleName, resName, callbackEvent, progressDelegate);
    }
    /// <summary>
    /// Debug场景中的所有已加载的包
    /// </summary>
    /// <param name="sceneName"></param>
    public void DebugAllAssetBundle(string sceneName)
    {
        m_AssetBundleLoader.DebugAllAssetBundle(sceneName);
    }
    /// <summary>
    /// 卸载单个已加载的资源
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    /// <param name="res"></param>
    public void ReleaseSingleObj(string sceneName, string bundleName, string res)
    {
        m_AssetBundleLoader.ReleaseSingleObj(sceneName, bundleName, res);
    }
    /// <summary>
    /// 卸载整个包中已加载的资源
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    public void ReleaseBundleObj(string sceneName, string bundleName)
    {
        m_AssetBundleLoader.ReleaseBundleObj(sceneName, bundleName);
    }
    /// <summary>
    /// 卸载整个场景中已加载的资源
    /// </summary>
    /// <param name="sceneName"></param>
    public void ReleaseSceneObj(string sceneName)
    {
        assetBundleLoader.ReleaseSceneObj(sceneName);
    }
    /// <summary>
    /// 卸载单个bundle包，须先卸载资源才有用
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    public void ReleaseSingleBundle(string sceneName, string bundleName)
    {
        m_AssetBundleLoader.ReleaseSingleBundle(sceneName, bundleName);
    }
    /// <summary>
    /// 卸载场景所包含的所有bundle包，须卸载资源才有用
    /// </summary>
    /// <param name="sceneName"></param>
    public void ReleaseSceneBundles(string sceneName)
    {
        m_AssetBundleLoader.ReleaseSceneBundles(sceneName);
    }
    /// <summary>
    /// 卸载所有资源和bundle包
    /// </summary>
    /// <param name="sceneName"></param>
    public void ReleaseAll(string sceneName)
    {
        m_AssetBundleLoader.ReleaseAll(sceneName);
    }
    #endregion
    #region 动态加载光照贴图
    LightmapLoader lightmapLoader;
    public LightmapLoader m_LightmapLoader
    {
        get
        {
            if (lightmapLoader == null)
            {
                lightmapLoader = GetComponent<LightmapLoader>();

                if (lightmapLoader == null)
                {
                    lightmapLoader = gameObject.AddComponent<LightmapLoader>();
                }
            }
            return lightmapLoader;
        }
    }

    public void SetLightmaps(Transform target, LightmapScriptable renderinfo, Texture2D[] lightmaps_color, Texture2D[] lightmaps_dir = null)
    {
        m_LightmapLoader.SetLightMapData(target, renderinfo, lightmaps_color, lightmaps_dir);
    }

    public void SetLightMapColor( Texture2D[] lightmaps_color, Texture2D[] lightmaps_dir = null)
    {
        m_LightmapLoader.SetLightMapColor( lightmaps_color, lightmaps_dir);
    }
    public void SetRendererInfo(GameObject target, LightmapScriptable renderinfo)
    {
        m_LightmapLoader.SetRendererInfo(target, renderinfo);
    }
    #endregion
    #region WWW相关接口
    WWWLoader wwwLoader;
    public WWWLoader m_WWWLoader
    {
        get
        {
            if (wwwLoader == null)
            {
                wwwLoader = GetComponent<WWWLoader>();

                if (wwwLoader == null)
                {
                    wwwLoader = gameObject.AddComponent<WWWLoader>();
                }
            }
            return wwwLoader;
        }
    }
    /// <summary>
    /// 设置WWW是否流式加载
    /// </summary>
    public bool isStreaming_WWWLoad
    {
        get { return m_WWWLoader.IsStreaming; }
        set { m_WWWLoader.IsStreaming = value; }
    }
    /// <summary>
    /// www下载资源
    /// </summary>
    /// <param name="wwwPath"></param>
    /// <param name="downloadedHandle"></param>
    /// <param name="progressEventId"></param>
    public void Load_WWW(string wwwPath, DownloadedHandle downloadedHandle, ProgressDelegate progressEventId)
    {
        m_WWWLoader.Load_WWW(wwwPath, downloadedHandle, progressEventId);
    }
    /// <summary>
    /// 释放对应的www
    /// </summary>
    /// <param name="wwwPath"></param>
    public void ReleaseWWW(string wwwPath)
    {
        m_WWWLoader.ReleaseWWW(wwwPath);
    }
    /// <summary>
    /// 释放所有WWW包
    /// </summary>
    public void ReleaseAllWWW()
    {
        m_WWWLoader.ReleaseAll();
    }
    #endregion
}