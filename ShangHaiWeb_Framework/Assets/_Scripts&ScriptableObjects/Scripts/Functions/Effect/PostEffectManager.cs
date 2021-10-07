using UnityEngine;
//using UnityEngine.PostProcessing;
using DG.Tweening;

public class PostEffectManager : Singleton<PostEffectManager>
{
   // public PostProcessProfile currentProfiler;

    public delegate void MethodDelegate();


    //#region 全屏泛光

    ////全屏泛光开关
    //private void SwitchBloom(Camera camera, bool enable)
    //{
    //    currentProfiler = GetProfiler(camera);
    //    currentProfiler.bloom.enabled = enable;
    //}

    ///// <summary>
    ///// 关闭全屏泛光
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    //public void SetBloomOff(Camera camera)
    //{
    //    SwitchBloom(camera, false);
    //}

    ///// <summary>
    ///// 打开全屏泛光
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="intensity">强度</param>
    //public void SetBloomOn(Camera camera, float intensity)
    //{
    //    SwitchBloom(camera, true);
    //    BloomModel.Settings bloom = currentProfiler.bloom.settings;
    //    bloom.bloom.intensity = intensity;
    //    currentProfiler.bloom.settings = bloom;
    //}

    ///// <summary>
    ///// 打开全屏泛光-渐变
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="intensity">强度</param>
    ///// <param name="duration">达到目标强度所需的时间</param>
    ///// <param name="method">渐变完成后执行的事件(可为NULL)</param>
    //public void SetBloomOn_Duration(Camera camera, float intensity, float duration, MethodDelegate method)
    //{
    //    SwitchBloom(camera, true);
    //    BloomModel.Settings bloom = currentProfiler.bloom.settings;
    //    bloom.bloom.intensity = 0;

    //    DOTween.To(() => bloom.bloom.intensity, x => bloom.bloom.intensity = x, intensity, duration).OnUpdate(() =>
    //    {
    //        currentProfiler.bloom.settings = bloom;
    //    }).OnComplete(() => { if (method != null) method(); });
    //}

    //#endregion


    #region 黑屏

    //黑屏开关
    //private void SwitchBlack(Camera camera, bool enable)
    //{
    //    currentProfiler = GetProfiler(camera);
    //    currentProfiler.GetSetting<ColorGrading>().active = enable;
    //}

    ///// <summary>
    ///// 黑屏淡入
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="duration">淡入完成需要的时间</param>
    ///// <param name="method">淡入完成后执行的事件(可为NULL)</param>
    //public void SetBlack_FadeIn(Camera camera, float duration, MethodDelegate method)
    //{
    //    SwitchBlack(camera, true);
    //    ColorGrading black = currentProfiler.GetSetting<ColorGrading>();
    //    black.postExposure.value = 0.5f;

    //    DOTween.To(() => black.postExposure.value, x => black.postExposure.value = x, -10, duration).OnUpdate(() =>
    //    {
    //       // currentProfiler.colorGrading.settings = black;

    //    }).OnComplete(() => { if (method != null) method(); });
    //}

    ///// <summary>
    ///// 黑屏淡出
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="duration">淡出完成需要的时间</param>
    ///// <param name="method">淡出完成后执行的事件(可为NULL)</param>
    //public void SetBlack_FadeOut(Camera camera, float duration, MethodDelegate method)
    //{

    //    SwitchBlack(camera, true);
    //    ColorGrading black = currentProfiler.GetSetting<ColorGrading>();
    //    black.postExposure.value = -10f;

    //    DOTween.To(() => black.postExposure.value, x => black.postExposure.value = x, 0.5f, duration).OnUpdate(() =>
    //    {
    //       // currentProfiler.colorGrading.settings = black;
    //    }).OnComplete(() => 
    //    {
    //      //  currentProfiler.colorGrading.enabled = false;
    //        if (method != null) method();
    //    });
    //}

    #endregion


    //#region 画面模糊

    //// 模糊开关
    //public void SwitchBlur(Camera camera, bool enable)
    //{
    //    currentProfiler = GetProfiler(camera);
    //    currentProfiler.depthOfField.enabled = enable;
    //}

    ///// <summary>
    ///// 模糊淡入
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="duration">淡入完成需要的时间</param>
    ///// <param name="method">淡入完成后执行的事件(可为NULL)</param>
    //public void SetBlur_FadeIn(Camera camera, float duration, MethodDelegate method)
    //{
    //    SwitchBlur(camera, true);
    //    DepthOfFieldModel.Settings blur = currentProfiler.depthOfField.settings;
    //    blur.focusDistance = 2.8f;
    //    DOTween.To(() => blur.focusDistance, x => blur.focusDistance = x, 0.1f, duration).OnUpdate(() =>
    //    {
    //        currentProfiler.depthOfField.settings = blur;
    //    }).OnComplete(() => { if (method != null) method(); });
    //}

    ///// <summary>
    ///// 模糊淡出
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="duration">淡出完成需要的时间</param>
    ///// <param name="method">淡出完成后执行的事件(可为NULL)</param>
    //public void SetBlur_FadeOut(Camera camera, float duration, MethodDelegate method)
    //{
    //    SwitchBlur(camera, true);
    //    DepthOfFieldModel.Settings blur = currentProfiler.depthOfField.settings;
    //    blur.focusDistance = 0.1f;
    //    DOTween.To(() => blur.focusDistance, x => blur.focusDistance = x, 2.8f, duration).OnUpdate(() =>
    //    {
    //        currentProfiler.depthOfField.settings = blur;
    //    }).OnComplete(() => { currentProfiler.depthOfField.enabled = false; if (method != null) method(); });
    //}

    //#endregion


    //#region 动态模糊

    ////打开或关闭动态模糊
    //private void SwitchMotionBlur(Camera camera, bool enable)
    //{
    //    currentProfiler = GetProfiler(camera);
    //    currentProfiler.motionBlur.enabled = enable;
    //}

    ///// <summary>
    ///// 关闭动态模糊
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    //public void SetMotionBlurOff(Camera camera)
    //{
    //    SwitchMotionBlur(camera, false);
    //}

    ///// <summary>
    ///// 开启并设置动态模糊
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="frameBlending">帧融合，数值越大效果越明显(0~1)</param>
    //public void SetMotionBlurOn(Camera camera, float frameBlending)
    //{
    //    SwitchMotionBlur(camera, true);
    //    MotionBlurModel.Settings motionBlur = currentProfiler.motionBlur.settings;
    //    motionBlur.frameBlending = frameBlending;
    //    currentProfiler.motionBlur.settings = motionBlur;
    //}

    //#endregion


    //#region 画面噪声

    ////画面噪声开关
    //private void SwitchImageNoise(Camera camera, bool enable)
    //{
    //    currentProfiler = GetProfiler(camera);
    //    currentProfiler.grain.enabled = enable;
    //}

    ///// <summary>
    ///// 关闭画面噪声
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    //public void SetImageNoiseOff(Camera camera)
    //{
    //    SwitchImageNoise(camera, false);
    //}

    ///// <summary>
    ///// 开启并设置画面噪声
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="intensity">强度(0~1)</param>
    ///// <param name="luminanceContribution">发光度(0-1),数值越小颗粒越大</param>
    ///// <param name="size">(0.3-3)数值越大颗粒越大</param>
    ///// <param name="colored">颗粒是否带颜色信息</param>
    //public void SetImageNoiseOn(Camera camera, float intensity, float luminanceContribution, float size, bool colored)
    //{
    //    SwitchImageNoise(camera, true);
    //    GrainModel.Settings grain = currentProfiler.grain.settings;
    //    grain.intensity = intensity;
    //    grain.luminanceContribution = luminanceContribution;
    //    grain.size = size;
    //    grain.colored = colored;
    //    currentProfiler.grain.settings = grain;
    //}

    ///// <summary>
    ///// 开启画面噪声,默认intensity=0.5, luminanceContribution=0.8, size=1, colored=false
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    //public void SetImageNoiseOn(Camera camera)
    //{
    //    SwitchImageNoise(camera, true);
    //    GrainModel.Settings grain = currentProfiler.grain.settings;
    //    grain.intensity = 0.5f;
    //    grain.luminanceContribution = 0.8f;
    //    grain.size = 1;
    //    grain.colored = false;
    //    currentProfiler.grain.settings = grain;
    //}

    //#endregion


    //#region 虚光照
    ////虚光照开关
    //private void SwitchVignette(Camera camera, bool enable)
    //{
    //    currentProfiler = GetProfiler(camera);
    //    currentProfiler.vignette.enabled = enable;
    //}

    ///// <summary>
    ///// 虚光照淡入(centerX=0.5, centerY=0.5, intensity=0.45 , smoothness=0.2 , roundness=1 , rounded=false)
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="color">目标颜色</param>
    ///// <param name="duration">淡入完成所需时间</param>
    ///// <param name="method">淡入完成后执行的事件(可为NULL)</param>
    //public void SetVignette_FadeIn(Camera camera, Color color, float duration, MethodDelegate method)
    //{
    //    SwitchVignette(camera, true);
    //    VignetteModel.Settings vignette = currentProfiler.vignette.settings;
    //    vignette.color = Color.white;
    //    vignette.center = new Vector2(0.5f, 0.5f);
    //    vignette.intensity = 0.4f;
    //    vignette.smoothness = 0.4f;
    //    vignette.roundness = 1;

    //    Material mat = new Material(Shader.Find("Standard"));
    //    mat.color = Color.white;
    //    mat.DOColor(color, duration).OnUpdate(() => { vignette.color = mat.color; currentProfiler.vignette.settings = vignette; }).OnComplete(() =>
    //    {
    //        if (method != null)
    //            method();
    //    });
    //}

    ///// <summary>
    ///// 虚光照淡入
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="color">目标颜色</param>
    ///// <param name="center">中心点</param>
    ///// <param name="intensity">强度(0~1)</param>
    ///// <param name="smoothness">平滑程度(0.01~1)</param>
    ///// <param name="roundness">圆度(0~1)</param>
    ///// <param name="rounded">是否为圆形</param>
    ///// <param name="duration">淡入完成所需时间</param>
    ///// <param name="method">淡入完成后执行的事件(可为NULL)</param>
    //public void SetVignette_FadeIn(Camera camera, Color color, Vector2 center, float intensity, float smoothness, float roundness, bool rounded, float duration, MethodDelegate method)
    //{
    //    SwitchVignette(camera, true);
    //    VignetteModel.Settings vignette = currentProfiler.vignette.settings;
    //    vignette.color = Color.white;
    //    vignette.center = center;
    //    vignette.intensity = intensity;
    //    vignette.smoothness = smoothness;
    //    vignette.roundness = roundness;
    //    vignette.rounded = rounded;
    //    Material mat = new Material(Shader.Find("Standard"));
    //    mat.color = Color.white;
    //    mat.DOColor(color, duration).OnUpdate(() => { vignette.color = mat.color; currentProfiler.vignette.settings = vignette; }).OnComplete(() =>
    //    {
    //        if (method != null)
    //            method();
    //    });
    //}

    ///// <summary>
    ///// 虚光照淡出
    ///// </summary>
    ///// <param name="camera">脚本所挂载的摄像机</param>
    ///// <param name="duration">淡出完成所需时间</param>
    ///// <param name="method">淡出完成后执行的事件</param>
    //public void SetVignette_FadeOut(Camera camera, float duration, MethodDelegate method)
    //{
    //    SwitchVignette(camera, true);
    //    VignetteModel.Settings vignette = currentProfiler.vignette.settings;
    //    //vignette.color = Color.white;
    //    Material mat = new Material(Shader.Find("Standard"));
    //    mat.color = vignette.color;
    //    mat.DOColor(Color.white, duration).OnUpdate(() => { vignette.color = mat.color; currentProfiler.vignette.settings = vignette; }).OnComplete(() =>
    //    {
    //        if (method != null)
    //            method();
    //    });
    //}
    //#endregion


    //private PostProcessProfile GetProfiler(Camera camera)
    //{
    //    if (camera.GetComponent<PostProcessVolume>() == null)
    //    {
    //        PostProcessLayer pl = camera.gameObject.AddComponent<PostProcessLayer>();
    //        PostProcessVolume p = camera.gameObject.AddComponent<PostProcessVolume>();

    //        pl.enabled = false;
    //        p.enabled = false;

    //        pl.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
    //        pl.volumeTrigger = camera.transform;
    //        //  pl.volumeLayer = LayerMask.NameToLayer("PostProcessing");
    //        pl.volumeLayer = (1 << 8);
    //        PostProcessProfile ppp = Resources.Load<PostProcessProfile>("Post-processing Profile_start");
    //        return p.profile = ppp;
    //    }
    //    else if (camera.GetComponent<PostProcessVolume>().profile == null)
    //    {
    //        return camera.GetComponent<PostProcessVolume>().profile = new PostProcessProfile();
    //    }
    //    else
    //    {
    //        return camera.GetComponent<PostProcessVolume>().profile;
    //    }
    //}


    private void OnApplicationQuit()
    {
       // SwitchBlack(Camera.main, false);
        //SwitchBlur(Camera.main, false);
        //SwitchVignette(Camera.main, false);
        //SetMotionBlurOff(Camera.main);
        //SetImageNoiseOff(Camera.main);
        //SetBloomOff(Camera.main);
    }
}
