using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioComponent : MonoBehaviour, IAudio
{
    public bool registeMyselfOnAwake = true;

    public bool is3D = false;
    void Awake()//将自己注册到Manager中，其他模块可从Manager中获取此对象
    {
        if (registeMyselfOnAwake)
        {
            if (AudioVideoManager.Instance != null)
                AudioVideoManager.Instance.RegistMySelf_Audio(gameObject.name, this);
        }
        Init();
    }
    void OnDestroy()
    {
        if (AudioVideoManager.Instance != null)
            AudioVideoManager.Instance.UnRegistMySelf_Audio(gameObject.name);
    }
    void Init()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = is3D ? 1 : 0;
    }
    private AudioSource audioSource;

    [SerializeField]
    private List<AudioClip> AudioPool = new List<AudioClip>();
    //索引器必须以this关键字定义，其实这个this就是类实例化之后的对象
    public AudioClip this[int index]
    {
        //实现索引器的get方法
        get
        {
            if (AudioPool != null && AudioPool.Count > 0)
            {
                return AudioPool[index];
            }
            return null;
        }
    }
    public AudioClip AudioClipDefault//默认Clip，调用OnPlayAudio,不传入参数时，Play这个clip，非只读
    {
        get
        {
            if (AudioPool != null && AudioPool.Count > 0)
            {
                return AudioPool[0];
            }
            return null;
        }
        set
        {
            if (AudioPool == null)
            {
                AudioPool = new List<AudioClip>();
            }
            if (!AudioPool.Contains(value))
            {
                AudioPool.Insert(0, value);
            }
            else
            {
                AudioPool.Remove(value);
                AudioPool.Insert(0, value);
            }
        }
    }
    public AudioSource GetAudioSource()
    {
        return this.GetComponent<AudioSource>();
    }
    public void OnPlayAudio()
    {
        if (!this.audioSource.clip)
        {
            this.audioSource.clip = AudioClipDefault;
        }
        if (this.audioSource.clip == null)
        {
            Debug.Log("未添加audioClip");
            return;
        }
        audioSource.Play();
    }
    public void OnPlayAudio(int index)
    {
        this.audioSource.clip = AudioPool[index];
        OnPlayAudio();
    }
    public void OnPlayAudio(string audioName)
    {
        this.audioSource.clip = null;
        for (int i = 0; i < AudioPool.Count; i++)
        {
            if (AudioPool[i].name == audioName)
            {
                this.audioSource.clip = AudioPool[i];
                break;
            }
        }
        OnPlayAudio();
    }
    public void OnPlayAudio(AudioClip clip)
    {
        this.audioSource.clip = clip;
        if (!AudioPool.Contains(clip))
        {
            AudioPool.Add(clip);
        }
        OnPlayAudio();
    }

    public void AddAudioClip(AudioClip clip)
    {
        if (!AudioPool.Contains(clip))
        {
            AudioPool.Add(clip);
        }
        else
        {
            Debug.Log("arealdy contains clip:" + clip.name);
        }
    }
    public void AddAudioClips(AudioClip[] clips)
    {
        for (int i = 0; i < clips.Length; i++)
        {
            AddAudioClip(clips[i]);
        }
    }
    public void RemoveAudioClip(string clipName)
    {
        for (int i = 0; i < AudioPool.Count; i++)
        {
            if (AudioPool[i].name == clipName)
            {
                AudioPool.Remove(AudioPool[i]);
                return;
            }
        }
        Debug.Log("未找到AudioClip:" + clipName);
    }
    public void RemoveAudioClip(AudioClip clip)
    {
        if (AudioPool != null)
        {
            AudioPool.Remove(clip);
        }
    }
    public void RemoveAllAudioClip()
    {
        if (AudioPool != null)
        {
            AudioPool.Clear();
        }
    }

    public void SetVolume(float value)
    {
        audioSource.volume = value;
    }
}
