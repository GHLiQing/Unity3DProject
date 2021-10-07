using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

public interface IAudio
{
    AudioSource GetAudioSource();
    void OnPlayAudio();
    void OnPlayAudio(int index);
    void OnPlayAudio(string audioName);
    void OnPlayAudio(AudioClip clip);

    void AddAudioClip(AudioClip clip);
    void AddAudioClips(AudioClip[] clips);

    void RemoveAudioClip(AudioClip clip);
    void RemoveAllAudioClip();

    void SetVolume(float value);
}
public interface IVideo
{
    VideoPlayer GetVideoPlayer();

    RawImage GetVideoImage();

    void OnPlayVideo();

    void OnPlayVideo(int index);

    void OnPlayVideo(string url);

    void OnPauseVideo();

    void OnStopVideo();

    void AddVideoUrl(string url);

    void AddVideoUrls(string[] url);

    void RemoveAllUrls();

    void RemoveUrl(int index);
}

