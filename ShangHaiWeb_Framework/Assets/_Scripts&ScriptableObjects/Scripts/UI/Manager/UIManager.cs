
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class UIManager : Singleton<UIManager>
{
    #region 面板池相关
    /// <summary>
    /// 独立面板基类的列表
    /// </summary>
    private Dictionary<string, Panel> scenePanels;
    //注册脚本
    public void RegisetPanel(string key, Panel view)
    {
        if (scenePanels == null)
            scenePanels = new Dictionary<string, Panel>();

        if (!scenePanels.ContainsKey(key))
        {
            scenePanels.Add(key, view);
        }
    }
	/// <summary>
	/// 取消注册
	/// </summary>
	/// <param name="key"></param>
    public void UnRegisetPanel(string key)
    {
        if (scenePanels == null)
            return;

        if (scenePanels.ContainsKey(key))
        {
            scenePanels.Remove(key);
        }
    }
    //根据索引获取对应脚本
    public Panel GetPanel(string name)
    {
#if UNITY_EDITOR
        Debug.Log("当前注册面板数量：" + scenePanels.Count);
#endif
        if (scenePanels.ContainsKey(name))
        {
            return scenePanels[name];
        }
        else
        {
            return null;
        }
    }
    #endregion

  
    public override void Init()
    {
        scenePanels = new Dictionary<string, Panel>();

        //switch (SceneManager.GetActiveScene().buildIndex)
        //{
        //    case 0:
        //        Init_FirstScene();
        //        break;

        //    case 1:

        //        break;

        //    case 2:

        //        break;

        //    case 3:

        //        break;
        //}
    }


    public void GetMessage(string fromJs)
    {
        t.text = fromJs;
    }

    public Text t;
    public Button bt;
    void Init_FirstScene()
    {
        t = GameObject.Find("Canvas").transform.Find("Text").GetComponent<Text>();
        bt = GameObject.Find("Canvas").transform.Find("Button").GetComponent<Button>();
        bt.onClick.AddListener(()=> 
        {
            Application.ExternalCall("Test_open","www.baidu.com",this.transform.name);
        });
    }



}
