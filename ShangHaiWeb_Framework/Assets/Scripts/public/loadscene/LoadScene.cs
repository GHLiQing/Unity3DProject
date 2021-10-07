//#define All
#define Single
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadScene : MonoBehaviour
{

    public Transform target = null;
    public LightmapScriptable[] renderinfo = null;
    public Texture2D[] lightmaps_color = null;
    public Texture2D[] lightmaps_dir = null;
    //public ReflectionProbe[] rp;
    //public ReflectionProbe[] rp_loaded;
    public string sceneName;
    void Awake()
    {
        //开启流式加载
        LoadAssetManager.Instance.isStreaming_AssetBundle = true;
        sceneName = Loading.sceneName;
    }
    void Start()
    {
#if Single
        LoadLightmap_Color((tempObj) =>
        {        
            LoadAssetManager.Instance.SetLightMapColor( lightmaps_color, null);
            LoadLightmapDatas((tempObj_2) => 
            {
                renderinfo = new LightmapScriptable[tempObj_2.Length];
                for (int i = 0; i < tempObj_2.Length; i++)
                {
                    int temp_I = i;
                    
                    renderinfo[temp_I] = (LightmapScriptable)tempObj_2[temp_I];
                   // Debug.Log(renderinfo[temp_I].name);
                    int index = renderinfo[temp_I].name.IndexOf("_");
                    string prefabName = renderinfo[temp_I].name.Substring(index+1);
              //      Debug.Log("renderinfo[temp_I].name:"+ renderinfo[temp_I].name+ "  prefabName:" + prefabName);
                    LoadPrefab(prefabName,(tempObj_3) => 
                    {
                        LoadAssetManager.Instance.SetRendererInfo(tempObj_3, renderinfo[temp_I]);
                        EventPool<StrData>.GetInstance.PostEvent(this,new StrData((int)EventID.OnLoadScene));
                    });

                }
              //  LoadAssetManager.Instance.SetLightMapColor(lightmaps_color, null);
            });
        });
#elif All
        LoadPrefab_all((obj) =>
        {
            LoadLightmapDatas_all((tempdatas) =>
            {
                renderinfo = (LightmapScriptable)tempdatas[0];
                LoadLightmap_Color_all((tempTexture) =>
                    {
                        LoadAssetManager.Instance.SetLightmaps(target, renderinfo, lightmaps_color, null);
                    });
            });
        });



#endif
    }
    #region single
    void LoadLightmapDatas(UnityAction<Object[]> callback = null)
    {
        LoadAssetManager.Instance.LoadAssetBundle(LoadType.All, sceneName, "LightmapDatas", "", (tempObj) =>
        {            
            Debug.Log("renderinfo:LoadDown length:"+ tempObj.Length);
            if (callback != null)
                callback(tempObj);
        },
      (progress) =>
      {
          if (progress == -1)
          {
            //  Debug.Log("renderinfo:正在加载关联bundle包");
          }
          else
          {
             // Debug.Log("renderinfo:LoadProgress:  " + progress);
          }
      });
    }

    void LoadLightmap_Color(UnityAction<Object[]> callback = null)
    {
        LoadAssetManager.Instance.LoadAssetBundle(LoadType.All, sceneName, "Lightmap_Color", "", (tempObjs) =>
        {
            lightmaps_color = new Texture2D[tempObjs.Length];

            for (int i = 0; i < lightmaps_color.Length; i++)
            {
                lightmaps_color[i] = tempObjs[i] as Texture2D;
            }

            Debug.Log("lightmaps:LoadDown");
        
            if (callback != null)
                callback(tempObjs);
        },
     (progress) =>
     {
         if (progress == -1)
         {
           //  Debug.Log("lightmaps:正在加载关联bundle包");
         }
         else
         {
           //  Debug.Log("lightmaps:LoadProgress:  " + progress);
         }
     });
    }

    void LoadPrefab(string prefabName,UnityAction<GameObject> callback = null)
    {
        LoadAssetManager.Instance.LoadAssetBundle(LoadType.Single, sceneName, "prefab", prefabName, (tempObj) =>
        {


            //target = Instantiate<GameObject>((GameObject)tempObj[0], Vector3.zero, Quaternion.identity).transform;

            //target.gameObject.name = target.gameObject.name.Replace("(Clone)", "");


            //Debug.Log("Target:LoadDown" + " " + target + renderinfo + lightmaps_color);

          //  LoadAssetManager.Instance.SetLightmaps(target, renderinfo, lightmaps_color, null);
            //===========
            Transform parent = GameObject.Find("Scene").transform;
            //GameObject[] prefabs = new GameObject[tempObj.Length];
            //Debug.Log("tempObj.Length:" + tempObj.Length);
            //for (int i = 0; i < prefabs.Length; i++)
            //{
            //    prefabs[i] = Instantiate<GameObject>((GameObject)tempObj[i], parent, false);
            //}
            //   LoadAssetManager.Instance.SetLightmaps(parent, renderinfo, lightmaps_color, null);
            //=================
            //  GameObject prefabs = GameObject)tempObj[0];

            GameObject prefabs = Instantiate<GameObject>((GameObject)tempObj[0], parent, false);
          
            if (callback != null)
                callback(prefabs);
        },
    (progress) =>
    {
        if (progress == -1)
        {
            //Debug.Log("Target:正在加载关联bundle包");
        }
        else
        {
           // Debug.Log("Target:LoadProgress:  " + progress);
        }
    });
    }
    #endregion


    #region  all
    void LoadLightmapDatas_all(UnityAction<Object[]> callback = null)
    {
        LoadAssetManager.Instance.LoadAssetBundle(LoadType.All, sceneName, "LightmapDatas", "", (tempObj) =>
        {

            Debug.Log("renderinfo:LoadDown length:" + tempObj.Length);
            if (callback != null)
                callback(tempObj);
        },
      (progress) =>
      {
          if (progress == -1)
          {
              Debug.Log("renderinfo:正在加载关联bundle包");
          }
          else
          {
              Debug.Log("renderinfo:LoadProgress:  " + progress);
          }
      });
    }

    void LoadLightmap_Color_all(UnityAction<Object[]> callback = null)
    {
        LoadAssetManager.Instance.LoadAssetBundle(LoadType.All, sceneName, "Lightmap_Color", "", (tempObjs) =>
        {
            lightmaps_color = new Texture2D[tempObjs.Length];

            for (int i = 0; i < lightmaps_color.Length; i++)
            {
                lightmaps_color[i] = tempObjs[i] as Texture2D;
            }

            Debug.Log("lightmaps:LoadDown");

            if (callback != null)
                callback(tempObjs);
        },
     (progress) =>
     {
         if (progress == -1)
         {
             Debug.Log("lightmaps:正在加载关联bundle包");
         }
         else
         {
             Debug.Log("lightmaps:LoadProgress:  " + progress);
         }
     });
    }

    void LoadPrefab_all( UnityAction<Transform> callback = null)
    {
        LoadAssetManager.Instance.LoadAssetBundle(LoadType.All, sceneName, "prefab", "", (tempObj) =>
        {


            target = Instantiate<GameObject>((GameObject)tempObj[0], Vector3.zero, Quaternion.identity).transform;

            target.gameObject.name = target.gameObject.name.Replace("(Clone)", "");


            Debug.Log("Target:LoadDown" + " " + target + renderinfo + lightmaps_color);

           

            if (callback != null)
                callback(target);
        },
    (progress) =>
    {
        if (progress == -1)
        {
            Debug.Log("Target:正在加载关联bundle包");
        }
        else
        {
            Debug.Log("Target:LoadProgress:  " + progress);
        }
    });
    }

    #endregion

}
