using UnityEngine;
using System.Collections;

public class IABManifestLoader
{
    public AssetBundleManifest assetManifest;

    public AssetBundle manifestLoader;

    public string manifestPath;

    private bool isLoadFinish;

    private IABManifestLoader()
    {
        assetManifest = null;
        manifestLoader = null;
        isLoadFinish = false;
        manifestPath = IPathTools.GetWWWAssetBundlePath() + "/" + IPathTools.GetPlatformFolderName(Application.platform);
    }
    public IEnumerator LoadMainfest()
    {
        WWW manifest = new WWW(manifestPath);
        yield return manifest;
        if (!string.IsNullOrEmpty(manifest.error))
        {
            Debug.LogError(manifest.error);
        }
        else
        {
            if (manifest.progress >= 1.0f)
            {
                manifestLoader = manifest.assetBundle;
                assetManifest = manifestLoader.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                isLoadFinish = true;

            }
        }
    }
    public string[] GetDepences(string name)
    {
        //string[] s = assetManifest.GetAllDependencies(name);
        //Debug.Log("depences.Length:" + s.Length);
        //for (int i = 0; i < s.Length; i++)
        //{
        //    Debug.Log(s[i]);
        //}
        return assetManifest.GetAllDependencies(name);
    }
    public void UnloadManifest()
    {
        manifestLoader.Unload(true);
    }
    public void SetManifestPath(string path)
    {
        manifestPath = path;
    }
    private static IABManifestLoader instance = null;
    public static IABManifestLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new IABManifestLoader();
            }
            return instance;
        }
    }
    public bool IsLoadFinish()
    {
        return isLoadFinish;
    }
}
