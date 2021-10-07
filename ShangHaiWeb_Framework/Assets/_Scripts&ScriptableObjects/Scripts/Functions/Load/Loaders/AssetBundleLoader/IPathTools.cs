using UnityEngine;
using System.Collections;
using System.IO;

public class IPathTools
{
    public static string GetPlatformFolderName(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "IOS";
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.WindowsEditor:
                return "Windows";
            default:
                return "Windows";
        }
    }
    public static string GetAssetBundlePath()
    {
        string platFolder = GetPlatformFolderName(Application.platform);
        string allPath = Application.streamingAssetsPath + "/" + platFolder;
        return allPath;
    }
    /// <summary>
    /// 返回各个平台的datapath路径
    /// </summary>
    /// <returns></returns>
    public static string GetApplicationDataPath()
    {
        string tmpStr = "";
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            tmpStr = "file:///" + Application.dataPath;
        }
        else
        {
            string tmpPath = Application.dataPath;
#if UNITY_ANDROID
            tmpStr="jar:file://"+tmpPath;
#elif UNITY_STANDALONE_WIN
            tmpStr = "file:///" + tmpPath;
#elif UNITY_WEBPLAYER
            tmpStr=tmpPath;
#elif UNITY_WEBGL
            tmpStr=tmpPath;
#elif UNITY_IPHONE
            tmpStr=tmpPath;
#endif
        }
        tmpStr += "/";
        return tmpStr;
    }
    public static string GetWWWAssetBundlePath()
    {
        string tmpStr = "";
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            tmpStr = "file:///" + GetAssetBundlePath();
        }
        else
        {
            string tmpPath = GetAssetBundlePath();
#if UNITY_ANDROID
            tmpStr="jar:file://"+tmpPath;
#elif UNITY_STANDALONE_WIN
            tmpStr = "file:///" + tmpPath;
#elif UNITY_WEBPLAYER
            tmpStr=tmpPath;
#elif UNITY_WEBGL
            tmpStr=tmpPath;
#elif UNITY_IPHONE
            tmpStr=tmpPath;
#endif

        }
        return tmpStr;
    }
    public static string GetAssetResourcePath()
    {
        return Application.dataPath + "/Resources/AssetRecord";
    }
}
