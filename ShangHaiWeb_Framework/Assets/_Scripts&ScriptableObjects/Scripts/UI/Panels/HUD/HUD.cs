using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public enum HUDType
{
    Null,
    Round,
    Item
}
public class HUD : MonoBehaviour
{
    [SerializeField]
    private Tool tool;//对应道具

    public Vector2 offset;//偏移量

    public HUDType type;//当前组件的hud类型

    public bool isShow = false;//是否显示

    bool isInView = false;

    public Text hudText;//hud的text

    private Animator anim;

    private RectTransform myRect;

    private HUDPanel parentPanel;
    void Start()
    {
        anim = transform.GetComponent<Animator>();
        myRect = transform.GetComponent<RectTransform>();
    }

    void Update()
    {
        float angle = Vector3.Angle(Camera.main.transform.forward, tool.transform.position - Camera.main.transform.position);

        isInView = angle < 55;

        if (isShow)
        {
            Debug.Log(angle);

            if (isInView)
            {
                Vector2 screenPos = Camera.main.WorldToScreenPoint(tool.gameObject.transform.position);

                myRect.anchoredPosition = new Vector2(screenPos.x - Screen.width / 2, screenPos.y - Screen.height / 2) + offset;
            }

            anim.SetBool("isShow", isInView);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="tool"></param>
    public void InitHud(Tool tool, HUDPanel sender)
    {
        this.tool = tool;
        this.parentPanel = sender;
        this.offset = tool.hudOffset;
        hudText.text = tool.ToolName.ToString();
        tool.OnToolNameChanged = (value) => { hudText.text = value; };
        tool.OnToolActiveChanged = (value) =>
        {
            if (value)
            {
                if (parentPanel.showHudType == ShowHudType.全局开)
                    IsShowHud(value);
            }
            else
            {
                IsShowHud(value);
            }
        };
    }

    /// <summary>
    /// 获取hud对应的tool
    /// </summary>
    /// <returns></returns>
    public Tool GetToolInfo()
    {
        return tool;
    }

    /// <summary>
    /// 设置是否显示
    /// </summary>
    /// <param name="isShow"></param>
    public void IsShowHud(bool isShow)
    {
        this.isShow = isShow;
        anim.SetBool("isShow", isShow);
    }
}
