using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
public class HighlightingSystemManager : Singleton<HighlightingSystemManager>
{
    public Camera highLightCamera;
    public void Init(Camera targetCamera)
    {
        if (targetCamera.GetComponent<HighlightingRenderer>() == null)
        {
            targetCamera.gameObject.AddComponent<HighlightingRenderer>();
        }
        highLightCamera = targetCamera;
    }
    public void SetCameraHightlihgt(bool enable)
    {
        if (highLightCamera == null)
        {
            Debug.Log("未设置 HighLightCamera，请调用Init（Camera targetCamera）进行初始化");
            return;
        }

        highLightCamera.GetComponent<HighlightingRenderer>().enabled = enable;
    }

    public void AddHighlightComponent(Transform target, float delay, bool seeThrough, Color flashingStartColor, Color flashingEndColor)
    {
        if (target.GetComponent<HighlighterFlashing>())
            return;
        HighlighterFlashing flashing = target.gameObject.AddComponent<HighlighterFlashing>();
        flashing.seeThrough = seeThrough;
        flashing.flashingStartColor = flashingStartColor;
        flashing.flashingEndColor = flashingEndColor;
        flashing.flashingDelay = delay;
    }
    public void AddHighlightComponent(Transform target, float delay, bool seeThrough)
    {
        HighlighterFlashing flashing = target.gameObject.AddComponent<HighlighterFlashing>();
        flashing.seeThrough = seeThrough;
        flashing.flashingDelay = delay;
    }
    //移除高亮组件
    public void RemoveHighlightComponent(Transform target)
    {
        Destroy(target.GetComponent<HighlighterFlashing>());

        Destroy(target.GetComponent<Highlighter>());
    }

}
