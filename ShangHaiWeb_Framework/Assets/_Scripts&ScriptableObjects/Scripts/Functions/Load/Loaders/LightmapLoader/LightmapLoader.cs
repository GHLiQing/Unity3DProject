using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 动态加载光照贴图组件，如果场景不动态加载，直接挂这个组件用就行，如需要动态加载，则直接在LoadAssetManager中调用对应API
/// </summary>
public class LightmapLoader : MonoBehaviour
{
    //public LightmapScriptable rendererInfo;

    //public Texture2D[] lightmaps_Color;

    //public Texture2D[] lightmaps_Directional;

    //public Transform target;

    public bool setLightmapsOnAwake = false;
    private void Start()
    {
        if (setLightmapsOnAwake) SetLightMapData();
    }
    public void SetLightMapData()
    {
      //  SetLightMapData(target, rendererInfo, lightmaps_Color, lightmaps_Directional);
    }

    public void SetLightMapColor(  Texture2D[] m_Lightmaps_Color, Texture2D[] m_lightmaps_Directional = null)
    {
      //  Debug.Log("SetLightMapColor");
        if ( m_Lightmaps_Color == null)
        {
            Debug.LogError("对应参数为空！ m_Lightmaps_Color:" + m_Lightmaps_Color);
            return;
        }   
        var lightmapDatas = LightmapSettings.lightmaps;

       // int oldLength = lightmapDatas.Length;
        int oldLength = 0;
        var combinedLightmaps = new LightmapData[oldLength + m_Lightmaps_Color.Length];

        lightmapDatas.CopyTo(combinedLightmaps, 0);

        for (int i = 0; i < m_Lightmaps_Color.Length; i++)
        {
            combinedLightmaps[i + oldLength] = new LightmapData();
            combinedLightmaps[i + oldLength].lightmapColor = m_Lightmaps_Color[i];
            if (m_lightmaps_Directional != null)
            {
                combinedLightmaps[i + oldLength].lightmapDir = m_lightmaps_Directional[i];
            }
        }
        if (m_lightmaps_Directional != null)
        {
            LightmapSettings.lightmapsMode = LightmapsMode.CombinedDirectional;
        }
        else
        {
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        }

        LightmapSettings.lightmaps = combinedLightmaps;
    }

    public void SetRendererInfo(GameObject target, LightmapScriptable rendererInfo)
    {
        var m_RendererInfo = rendererInfo.m_RendererInfo;

        if (m_RendererInfo == null || m_RendererInfo.Length == 0)
            return;

        var renderers = target.GetComponentsInChildren<MeshRenderer>();

        var lightmapDatas = LightmapSettings.lightmaps;

        int oldLength = 0;//???

        int dataIndex = 0;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].gameObject.tag != "LightBaked")
                continue;
         //   Debug.Log("renderers[i].name:" + renderers[i].name+ "   renderers[i].lightmapIndex:" + renderers[i].lightmapIndex);
            renderers[i].lightmapIndex = m_RendererInfo[dataIndex].lightmapIndex + oldLength;

            renderers[i].lightmapScaleOffset = m_RendererInfo[dataIndex].lightmapOffsetScale;

            dataIndex++;
        }
    }

    public void SetLightMapData(Transform target, LightmapScriptable rendererInfo, Texture2D[] m_Lightmaps_Color, Texture2D[] m_lightmaps_Directional = null)
    {
        Debug.Log("set all");
        if (target == null || rendererInfo == null || m_Lightmaps_Color == null)
        {
            Debug.LogError("对应参数为空！target:"+ target+ " rendererInfo"+ rendererInfo+ " m_Lightmaps_Color:"+ m_Lightmaps_Color);
            return;
        }
        var m_RendererInfo = rendererInfo.m_RendererInfo;

        var lightmapDatas = LightmapSettings.lightmaps;

           int oldLength = lightmapDatas.Length;
      
        var combinedLightmaps = new LightmapData[lightmapDatas.Length + m_Lightmaps_Color.Length];

        lightmapDatas.CopyTo(combinedLightmaps, 0);

        for (int i = 0; i < m_Lightmaps_Color.Length; i++)
        {
            combinedLightmaps[i + lightmapDatas.Length] = new LightmapData();
            combinedLightmaps[i + lightmapDatas.Length].lightmapColor = m_Lightmaps_Color[i];
            if (m_lightmaps_Directional != null)
            {
                combinedLightmaps[i + lightmapDatas.Length].lightmapDir = m_lightmaps_Directional[i];
            }
        }
        if (m_lightmaps_Directional != null)
        {
            LightmapSettings.lightmapsMode = LightmapsMode.CombinedDirectional;
        }
        else
        {
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        }

        LightmapSettings.lightmaps = combinedLightmaps;

        if (m_RendererInfo == null || m_RendererInfo.Length == 0)
            return;

        var renderers = target.GetComponentsInChildren<MeshRenderer>();

        int dataIndex = 0;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].gameObject.tag != "LightBaked")
                continue;

            renderers[i].lightmapIndex = m_RendererInfo[dataIndex].lightmapIndex + oldLength;

            renderers[i].lightmapScaleOffset = m_RendererInfo[dataIndex].lightmapOffsetScale;

            dataIndex++;
        }
    }
}
