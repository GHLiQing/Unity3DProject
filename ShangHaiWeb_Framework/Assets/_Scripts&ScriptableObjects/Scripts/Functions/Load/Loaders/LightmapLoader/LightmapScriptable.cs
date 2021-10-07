using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RendererInfo
{
    public string name;
    public int lightmapIndex;
    public Vector4 lightmapOffsetScale;
}

public class LightmapScriptable : ScriptableObject 
{
    [SerializeField]
    public RendererInfo[] m_RendererInfo;
}
