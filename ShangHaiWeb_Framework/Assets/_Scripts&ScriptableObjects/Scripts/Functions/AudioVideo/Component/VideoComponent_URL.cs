using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.UI;
[RequireComponent(typeof(RawImage))]
public class VideoComponent_URL : MonoBehaviour, IVideo
{
    #region 内部变量
    private VideoPlayer vPlayer
        ;
    private AudioSource source;

    private RawImage videoImage;

    private bool playOnPrepareCompleted = false;

    private string videoUrlDefault;

    private bool isInDrag = false;
    #endregion

    #region SerializeField
    [SerializeField]
    private List<string> videoUrlPool;

    public bool playOnAwake = false;

    public bool prepareOnAwake = true;


    public bool isLooping = false;

    public bool waitForFirstFrame = false;

    public bool isStreamingAssetsPath = false;

    public Graphic loadingUI;

    public Graphic coverUI;

    public Button playBtn;

    public Button stopBtn;

    public Button pauseBtn;

    public Slider sliderVideo;

    public Text text_Time;

    public Slider sliderSource;//音量

    public Text textSource;

    public Toggle voiceButton;
    #endregion

    public bool registeMyselfOnAwake = true;

    void Awake()
    {
        if(registeMyselfOnAwake)
        {
            if (AudioVideoManager.Instance != null)
                AudioVideoManager.Instance.RegistMySelf_Video(gameObject.name, this);
        }
        Init();
    }
    void OnDestroy()
    {
        if (AudioVideoManager.Instance != null)
            AudioVideoManager.Instance.UnRegistMySelf_Video(gameObject.name);
    }
    void Init()
    {
        videoImage = GetComponent<RawImage>();
        //一定要动态添加这两个组件，要不然会没声音
        vPlayer = gameObject.AddComponent<VideoPlayer>();
        source = gameObject.AddComponent<AudioSource>();

        vPlayer.isLooping = isLooping;
        vPlayer.waitForFirstFrame = waitForFirstFrame;

        //这3个参数不设置也会没声音 唤醒时就播放关闭
        vPlayer.playOnAwake = false;
        source.playOnAwake = false;
        source.Pause();


        //设置为URL模式
        vPlayer.source = VideoSource.Url;
        //设置渲染模式为APIOnly模式
        vPlayer.renderMode = VideoRenderMode.APIOnly;

        //在视频中嵌入的音频类型
        vPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //把声音组件赋值给VideoPlayer
        vPlayer.SetTargetAudioSource(0, source);

        if (playOnAwake)
        {
            if (playBtn)
            {
                playBtn.gameObject.SetActive(false);
            }
        }

        AddPlayFinishedEvent((videoplayer) =>
        {
            if (!vPlayer.isLooping)
            {
                OnStop();
            }
        });

        AddPrepareCompletedEvent((videoplayer) => { OnPrepareCompleted(playOnPrepareCompleted); });

        #region 添加按钮事件
        if (sliderSource != null)
        {
            sliderSource.value = source.volume;
            sliderSource.onValueChanged.AddListener(delegate { ChangeSource(sliderSource.value); });
        }
        if (sliderVideo != null)
        {
            sliderVideo.value = 0;
            EventTriggerListener.Get(sliderVideo.gameObject).onDown += (thisObj) => { isInDrag = true; };
            EventTriggerListener.Get(sliderVideo.gameObject).onUp += (thisObj) => { isInDrag = false; ChangeVideo(sliderVideo.value); };
        }
        #endregion

        vPlayer.errorReceived += (videoplayer,str) => { Debug.Log("error:"+ str); };
    }
    void Start()
    {
        //如果playOnAwake，则Prepare完毕后自动播放
        playOnPrepareCompleted = playOnAwake;

        if (playOnAwake || prepareOnAwake)
        {
            //开始预加载视频
            PrepareVideo(VideoUrlDefault);
        }
    }

    void LateUpdate()
    {
        if (vPlayer.isPlaying && text_Time != null && sliderVideo)
        {

            float time = (float)vPlayer.time;

            if (!isInDrag)
            {
                sliderVideo.value = time;
            }

            int hour = (int)time / 60;

            int mint = (int)time % 60;

            int hout_Count = (int)sliderVideo.maxValue / 60;

            int mint_Count = (int)sliderVideo.maxValue % 60;

            text_Time.text = string.Format("{0}:{1}/{2}:{3}", hour.ToString("0"), mint.ToString("00"), hout_Count.ToString("0"), mint_Count.ToString("00"));

        }
    }

    #region 内置方法
    /// <summary>
    ///     改变音量大小
    /// </summary>
    /// <param name="value"></param>
    private void ChangeSource(float value)
    {
        source.volume = value;
        if (textSource != null)
        {
            textSource.text = string.Format("{0:0}%", value * 100);
        }
    }

    /// <summary>
    ///     改变视频进度
    /// </summary>
    /// <param name="value"></param>
    private void ChangeVideo(float value)
    {
       // Debug.Log("change process:"+value);
        vPlayer.time = value;
    }
    private string GetStreamingAssetsPath(string path)
    {
        if (path.Contains("StreamingAssets/"))
        {
            return path;
        }
        return Application.streamingAssetsPath + "/" + path;
    }
    private void PrepareVideo(string url)
    {
        //设置URL并进行预加载
        this.vPlayer.url = url;

        if (this.vPlayer.url == null)
        {
            Debug.Log("未添加URL");
            return;
        }
        Debug.Log("url:"+vPlayer.url);
        vPlayer.Prepare();

        OnPrepare();
    }
    private void OnPrepare()
    {
        Debug.Log("OnPrepare");
        //先将视频RawImage的透明度设为0，开始播放后再调回1
        videoImage.color = new Color(1, 1, 1, 0);
        //打开loading的UI
        if (!vPlayer.isPrepared && loadingUI)
        {
            loadingUI.gameObject.SetActive(true);
        }
        //如果Prepare完毕直接播放的话，就关闭封面UI和播放按钮
        if (playOnPrepareCompleted)
        {
            if (coverUI)
            {
                coverUI.gameObject.SetActive(false);
            }
            if (playBtn)
            {
                playBtn.gameObject.SetActive(false);
            }
        }
    }
    private void OnPlay()
    {
        Debug.Log("OnPlay");
        //把图像赋给RawImage
        videoImage.texture = vPlayer.texture;
        if(sliderVideo!=null )
        {
            //帧数/帧速率=总时长    如果是本地直接赋值的视频，我们可以通过VideoClip.length获取总时长
            sliderVideo.maxValue = vPlayer.frameCount / vPlayer.frameRate;
        }

        MyUtilities.DelayToDo(0.15f, () => { videoImage.color = new Color(1, 1, 1, 1); });

        if (coverUI)
        {
            coverUI.gameObject.SetActive(false);
        }

        if (playBtn)
        {
            playBtn.gameObject.SetActive(false);
        }

        if (stopBtn)
        {
            stopBtn.gameObject.SetActive(true);
        }

        if (pauseBtn)
        {
            pauseBtn.gameObject.SetActive(true);
        }

        if (sliderVideo)
        {
            sliderVideo.gameObject.SetActive(true);
        }
        if (voiceButton)
        {
            voiceButton.gameObject.SetActive(true);
        }
    }
    private void OnPrepareCompleted(bool isPlayOnPrepareCompleted)
    {
        Debug.Log("OnPrepareCompleted");
        //关闭LoadingUI
        if (loadingUI)
        {
            loadingUI.gameObject.SetActive(false);
        }
        //如果isPlayOnPrepareCompleted为true的话 播放视频
        if (isPlayOnPrepareCompleted)
        {
            OnPlayVideo();
        }
    }
    private void OnPause()
    {
        Debug.Log("OnPause");

        if (playBtn)
        {
            playBtn.gameObject.SetActive(true);
        }

        if (pauseBtn)
        {
            pauseBtn.gameObject.SetActive(false);
        }

    }
    private void OnStop()
    {
        Debug.Log("OnStop");
        if (stopBtn)
        {
            stopBtn.gameObject.SetActive(false);
        }
        if (playBtn)
        {
            playBtn.gameObject.SetActive(true);
        }
        if (pauseBtn)
        {
            pauseBtn.gameObject.SetActive(false);
        }
        if (sliderVideo)
        {
            sliderVideo.gameObject.SetActive(false);
        }
        if (voiceButton)
        {
            voiceButton.gameObject.SetActive(false);
        }
        if (coverUI)
        {
            coverUI.gameObject.SetActive(true);
        }
    }
    #endregion

    #region 对外接口
    public string VideoUrlDefault//默认Clip，调用OnPlayAudio,不传入参数时，Play这个clip，非只读
    {
        get
        {
            if (videoUrlDefault != null && videoUrlDefault != "")
            {
                if (!isStreamingAssetsPath)
                {

                    return videoUrlDefault;

                }
                else
                {

                    return GetStreamingAssetsPath(videoUrlDefault);

                }
            }
            if (videoUrlPool != null && videoUrlPool.Count > 0)
            {
                if (!isStreamingAssetsPath)
                {

                    return videoUrlPool[0];

                }
                else
                {

                    return GetStreamingAssetsPath(videoUrlPool[0]);

                }
            }
            return null;
        }
        set
        {
            videoUrlDefault = value;

            if (videoUrlPool == null)
            {
                videoUrlPool = new List<string>();
            }

            if (!videoUrlPool.Contains(value))
            {
                videoUrlPool.Add(value);
            }
        }
    }
    //索引器必须以this关键字定义，其实这个this就是类实例化之后的对象
    public string this[int index]
    {
        //实现索引器的get方法
        get
        {
            if (videoUrlPool != null && videoUrlPool.Count > 0)
            {
                return videoUrlPool[index];
            }
            return null;
        }
    }
    public VideoPlayer GetVideoPlayer()
    {
        return vPlayer;
    }
    public RawImage GetVideoImage()
    {
        return videoImage;
    }
    #region Play按钮事件，对外接口
    public void OnPlayVideo()
    {
        if (vPlayer.url == VideoUrlDefault && vPlayer.isPrepared)
        {
            //如果加载完毕则直接播放
            vPlayer.Play();
            OnPlay();
        }
        else //如果没有预加载完毕则开始预加载
        {
            //因为是点击按钮事件，所以这里直接把playOnPrepareCompleted设为true，Prepare完毕后直接播放
            playOnPrepareCompleted = true;
            PrepareVideo(VideoUrlDefault);
        }
    }
    public void OnPlayVideo(int index)
    {
        if (videoUrlPool != null && videoUrlPool.Count >= index + 1)
        {
            VideoUrlDefault = videoUrlPool[index];
        }
        else
        {
            Debug.Log("该索引不存在");
            return;
        }
        OnPlayVideo();
    }
    public void OnPlayVideo(string url)
    {
        VideoUrlDefault = url;
        OnPlayVideo();
    }

    #endregion
    #region 暂停、停止按钮事件，对外接口
    public void OnPauseVideo()
    {
        vPlayer.Pause();
        OnPause();
    }
    public void OnStopVideo()
    {
        vPlayer.Stop();
        OnStop();
    }
    #endregion
    #region 添加URL到池中

    public void AddVideoUrl(string url)
    {
        if (videoUrlPool == null)
        {
            videoUrlPool = new List<string>();
        }
        if (!videoUrlPool.Contains(url))
        {
            videoUrlPool.Add(url);
        }
    }

    public void AddVideoUrls(string[] url)
    {
        for (int i = 0; i < url.Length; i++)
        {
            AddVideoUrl(url[i]);
        }
    }

    #endregion
    #region  从池中移除URL
    public void RemoveUrl(int index)
    {
        if (videoUrlPool != null && videoUrlPool.Count > index)
        {
            videoUrlPool.RemoveAt(index);
        }
    }
    public void RemoveAllUrls()
    {
        if (videoUrlPool != null)
        {
            videoUrlPool.Clear();
        }
    }
    #endregion
    #region 添加相关事件
    public void AddPrepareCompletedEvent(VideoPlayer.EventHandler prepareCompleteEvent)
    {
        vPlayer.prepareCompleted += prepareCompleteEvent;
    }
    public void AddPlayFinishedEvent(VideoPlayer.EventHandler finishEvent)
    {
        vPlayer.loopPointReached += finishEvent;
    }
    #endregion
    #endregion
}
