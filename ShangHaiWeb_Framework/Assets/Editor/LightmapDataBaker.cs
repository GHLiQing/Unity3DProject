using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class LightmapDataBaker : MonoBehaviour
{

    [UnityEditor.MenuItem("Itools/Bake Prefab Lightmaps")]
    static void GenerateLightmapInfo()
    {
        if (UnityEditor.Lightmapping.giWorkflowMode != UnityEditor.Lightmapping.GIWorkflowMode.OnDemand)
        {
            Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
            return;
        }

        GameObject[] prefabs = Selection.gameObjects;

        foreach (var instance in prefabs)
        {
            GameObject tempGameObj = instance.gameObject;

            var rendererInfos = new List<RendererInfo>();

            var lightmaps = new List<Texture2D>();

            GenerateLightmapInfo(tempGameObj, rendererInfos, lightmaps);

            LightmapScriptable lsa = ScriptableObject.CreateInstance<LightmapScriptable>();

            lsa.m_RendererInfo = rendererInfos.ToArray();

            var targetPrefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(tempGameObj) as GameObject;

            if (targetPrefab != null)
            {
                //UnityEditor.Prefab  
                UnityEditor.PrefabUtility.ReplacePrefab(tempGameObj, targetPrefab);
            }

            string path = "Assets/LightmapDatas";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "/lightmapData_" + tempGameObj.name + ".asset";

            int index = 0;

            while (File.Exists(path))
            {
                string replaceText = ".asset";
                if (index > 0)
                {
                    replaceText = (index - 1).ToString() + ".asset";
                }

                path = path.Replace(replaceText, index.ToString() + ".asset");

                index++;
            }

            UnityEditor.AssetDatabase.CreateAsset(lsa, path);

            UnityEditor.AssetDatabase.Refresh();
        }
    }

    static void GenerateLightmapInfo(GameObject root, List<RendererInfo> rendererInfos, List<Texture2D> lightmaps)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>();
      //  Debug.Log("root.name"+root.name+ "count:"+ renderers.Length);
        foreach (MeshRenderer renderer in renderers)
        {
           // Debug.Log("renderer :"+ renderer.name+"  index:" + renderer.lightmapIndex);
            if (renderer.lightmapIndex != -1)
            {
                if (!isHasTag("LightBaked"))
                {
                    UnityEditorInternal.InternalEditorUtility.AddTag("LightBaked");
                }
                renderer.gameObject.tag = "LightBaked";
                Debug.Log("renderer.gameObject" + renderer.name + "  renderer.gameObject.tag:" + renderer.gameObject.tag);
                RendererInfo info = new RendererInfo();

                info.name = renderer.transform.name;

                info.lightmapOffsetScale = renderer.lightmapScaleOffset;

                info.lightmapIndex = renderer.lightmapIndex;

                rendererInfos.Add(info);
            }
        }
    }
    static bool isHasTag(string tag)
    {
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
        {
            if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
                return true;
        }
        return false;
    }

}
