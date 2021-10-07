using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShowHudType
{
    全局开,
    射线检测,
    全局关
}
public class HUDPanel : Panel
{
    public ShowHudType showHudType;//显示Hud的方式

    private ShowHudType currentShowHudType;

    public GameObject[] hudPrefabs;  // hud预制体拖放顺序要与Constant中HUDType对应

    public Transform hudParent;

    private RayDetect rayDt;

    protected override void Awake()
    {
        base.Awake();

    //    UIManager.Instance.OnAddTool += AddHUD;
     //   UIManager.Instance.OnReduceTool += RemoveHUD;

        if (showHudType == ShowHudType.全局开)    //此处延迟执行是为了等待Tool注册完毕
        {
            MyUtilities.DelayToDo(0.05f, () =>
            {
                IsShowAllHUD(true);
            });
        }

        rayDt = new RayDetect((tar) =>
        {
            if (tar.GetComponent<Tool>())
            {
                IsShowHUD(tar.GetComponent<Tool>(), true);//鼠标移入显示标签
            }
        }, (tar) =>
        {
            if (tar.GetComponent<Tool>())
            {
                IsShowHUD(tar.GetComponent<Tool>(), false);//鼠标移出隐藏标签
            }
        });
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (UIManager.Instance != null)
        {
         //   UIManager.Instance.OnAddTool -= AddHUD;
         //   UIManager.Instance.OnReduceTool -= RemoveHUD;
        }
    }

    void Update()
    {
        if (currentShowHudType != showHudType)
        {
            switch (showHudType)
            {
                case ShowHudType.全局开:
                    IsShowAllHUD(true);
                    break;
                case ShowHudType.射线检测:
                    IsShowAllHUD(false);
                    break;
                case ShowHudType.全局关:
                    IsShowAllHUD(false);
                    break;
            }
            currentShowHudType = showHudType;
        }
        if (showHudType == ShowHudType.射线检测)
        {
            rayDt.Update(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, ~(1 << -1), true);
        }
    }

    /// <summary>
    /// 根据Tool的HUDType适配prefab
    /// </summary>
    /// <param name="tool"></param>
    /// <returns></returns>
    private GameObject GetPrefab(Tool tool)
    {
        for (int i = 0; i < hudPrefabs.Length; i++)
        {
            if (hudPrefabs[i].GetComponent<HUD>().type == tool.hudType)
            {
                return hudPrefabs[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 添加指定Tool的HUD
    /// </summary>
    /// <param name="tool"></param>
    public void AddHUD(Tool tool)
    {
        if (tool == null)
        {
            Debug.Log("Tools不存在！");
            return;
        }
        GameObject prefab = GetPrefab(tool);
        if (prefab == null)
            return;

        GameObject go = Instantiate(prefab, hudParent) as GameObject;

        go.transform.localScale = Vector3.one;
        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);

        go.GetComponent<HUD>().InitHud(tool, this);

        go.transform.name = tool.ToolName.ToString();

    }

    /// <summary>
    /// 删除指定tool的HUD
    /// </summary>
    /// <param name="tool"></param>
    public void RemoveHUD(Tool tool)
    {
        if (hudParent != null)
        {
            for (int i = 0; i < hudParent.childCount; i++)
            {
                if (hudParent.GetChild(i).GetComponent<HUD>().GetToolInfo().GetInstanceID() == tool.GetInstanceID())
                {
                    DestroyImmediate(hudParent.GetChild(i).gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 设置指定HUD是否显示
    /// </summary>
    /// <param name="hud"></param>
    /// <param name="isShow"></param>
    public void IsShowHUD(Tool tool, bool isShow)
    {
        for (int i = 0; i < hudParent.childCount; i++)
        {
            if (hudParent.GetChild(i).GetComponent<HUD>().GetToolInfo().GetInstanceID() == tool.GetInstanceID())
            {
                hudParent.GetChild(i).GetComponent<HUD>().IsShowHud(isShow);
            }
        }
    }

    /// <summary>
    /// 设置所有标签的隐藏显示
    /// </summary>
    /// <param name="Show"></param>
    public void IsShowAllHUD(bool Show)
    {
        for (int i = 0; i < hudParent.childCount; i++)
        {
            hudParent.GetChild(i).GetComponent<HUD>().IsShowHud(Show);
        }
    }

    /// <summary>
    /// 开关面板
    /// </summary>
    /// <param name="display"></param>
    public override void SetPanelDisplay(bool display)
    {
        isDisplay = display;
        this.myRectTransfrom.gameObject.SetActive(display);
    }
}
