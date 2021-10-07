/*
 * LoadAssetManager
 * 2017 年 12 月 11 日 19: 00:55.39
 * 南京农业大学土地质量检测
 * 莱医特
 * .cs
 * NK
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVideoManager : Singleton<AudioVideoManager>
{
    public override void Init()
    {
        sceneAudios = new Dictionary<string, IAudio>();
        sceneVideos = new Dictionary<string, IVideo>();
    }
    #region Audio相关注册、获取
    private Dictionary<string, IAudio> sceneAudios;
    public void RegistMySelf_Audio(string name, IAudio audio)
    {
        if (sceneAudios == null)
            return;
        if (!sceneAudios.ContainsKey(name))
        {
            sceneAudios.Add(name, audio);
        }
        else
        {
            Debug.Log("已添加audio：" + name + " ,请检查是否重名");
        }
    }
    public void UnRegistMySelf_Audio(string name)
    {
        if (sceneAudios == null)
            return;
        if (sceneAudios.ContainsKey(name))
        {
            sceneAudios.Remove(name);
        }
        else
        {
            Debug.Log("未找到audio：" + name);
        }
    }
    public IAudio GetAudioComponent(string name)
    {
        if (sceneAudios.ContainsKey(name))
        {
            return sceneAudios[name];
        }
        Debug.Log("未找到IAudio:" + name);
        return null;
    }
    /// <summary>
    /// 统一设置场景中所有Audio的音量
    /// </summary>
    /// <param name="value"></param>
    public void SetSceneAudiosVolume(float value)
    {
        foreach (var item in sceneAudios)
        {
            item.Value.SetVolume(value);
        }
    }
    #endregion
    #region Video相关注册、获取
    private Dictionary<string, IVideo> sceneVideos;
    public void RegistMySelf_Video(string name, IVideo video)
    {
        if (sceneVideos == null)
            return;
        if (!sceneVideos.ContainsKey(name))
        {
            sceneVideos.Add(name, video);
        }
        else
        {
            Debug.Log("已添加video：" + name + " ,请检查是否重名");
        }
    }
    public void UnRegistMySelf_Video(string name)
    {
        if (sceneVideos == null)
            return;
        if (sceneVideos.ContainsKey(name))
        {
            sceneVideos.Remove(name);
        }
        else
        {
            Debug.Log("未找到video：" + name);
        }
    }
    public IVideo GetVideoComponent(string name)
    {
        if (sceneVideos.ContainsKey(name))
        {
            return sceneVideos[name];
        }
        Debug.Log("未找到IVideo:" + name);
        return null;
    }
    #endregion
}
