using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class UIEventNode : MonoBehaviour
{
    public bool isShow = true;              // 是否显示

    public bool interactalbe = true;

    public Vector3 scale = Vector3.one;                // 缩放大小

    public EventTriggerListener.VoidDelegate onEnterAction;
    public EventTriggerListener.VoidDelegate onExitAction;
    public EventTriggerListener.VoidDelegate onClickAction;
    public EventTriggerListener.VoidDelegate OnMouseStayAction;
    public EventTriggerListener.VoidDelegate OnMouseDownAction;
    private bool isMouseOver = false;
    void Awake()
    {
        EventTriggerListener.Get(gameObject).onEnter += (target) =>
        {
            if (!interactalbe) return;
            if (onEnterAction != null)
            {
                onEnterAction(target);
            }
            isMouseOver = true;
        };

        EventTriggerListener.Get(gameObject).onExit += (target) =>
        {
            if (!interactalbe) return;

            if (onExitAction != null)
            {
                onExitAction(target);
            }
            isMouseOver = false;
        };

        EventTriggerListener.Get(gameObject).onClick += (target) =>
        {
            if (!interactalbe) return;

            if (onClickAction != null)
            {
                onClickAction(target);
            }
        };
        EventTriggerListener.Get(gameObject).onDown += (target) =>
        {
            if (!interactalbe) return;

            if (OnMouseDownAction != null)
            {
                OnMouseDownAction(target);
            }
        };
        //scale = this.transform.localScale;
    }
    /// <summary>
    /// 设置鼠标移入事件
    /// </summary>
    /// <param name="onEnterAction"></param>
    public void AddEnterAciton(EventTriggerListener.VoidDelegate onEnterAction)
    {
        this.onEnterAction += onEnterAction;
    }
    /// <summary>
    /// 清除鼠标移入事件
    /// </summary>
    public void ResetEnterAciton()
    {
        this.onEnterAction = null;
    }
    /// <summary>
    /// 设置鼠标移出事件
    /// </summary>
    /// <param name="onExitAction"></param>
    public void AddExitAciton(EventTriggerListener.VoidDelegate onExitAction)
    {
        this.onExitAction += onExitAction;
    }
    /// <summary>
    /// 清除鼠标移出事件
    /// </summary>
    public void ResetExitAciton()
    {
        this.onExitAction = null;
    }
    /// <summary>
    /// 设置鼠标点击事件
    /// </summary>
    /// <param name="onClickAction"></param>
    public void AddClickAciton(EventTriggerListener.VoidDelegate onClickAction)
    {
        this.onClickAction += onClickAction;
    }
    /// <summary>
    /// 清空点击事件
    /// </summary>
    public void ResetClickAciton()
    {
        this.onClickAction = null;
    }
    /// <summary>
    /// 设置鼠标停留事件
    /// </summary>
    /// <param name="OnMouseStayAction"></param>
    public void AddMouseOverAciton(EventTriggerListener.VoidDelegate OnMouseStayAction)
    {
        this.OnMouseStayAction += OnMouseStayAction;
    }
    /// <summary>
    /// 清空鼠标停留事件
    /// </summary>
    public void ResetMouseOverAciton()
    {
        this.OnMouseStayAction = null;
    }
    public void AddMouseDownAciton(EventTriggerListener.VoidDelegate OnMouseDownAction)
    {
        this.OnMouseDownAction += OnMouseDownAction;
    }
    public void ResetMouseDownAciton()
    {
        this.OnMouseDownAction = null;
    }
    void Update()
    {
        if (!interactalbe) return;

        if (isMouseOver && OnMouseStayAction != null)
        {
            OnMouseStayAction(this.gameObject);
        }
        if (gameObject.activeSelf != isShow)
        {
            SetDisplay(isShow);
        }
    }
    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="pos"></param>
    public void SetTransformInfo(Vector3 position, Vector3 eulerAngles, Vector3 scale, Space spaceSelf = Space.World)
    {
        if (spaceSelf == Space.World)
        {
            transform.eulerAngles = eulerAngles;
            transform.position = position;
            transform.localScale = scale;
        }
        else
        {
            transform.localEulerAngles = eulerAngles;
            transform.localPosition = position;
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// 设置开关
    /// </summary>
    /// <param name="isShow"></param>
    public void SetDisplay(bool isShow)
    {
        if (!isShow)
            transform.localScale = Vector3.zero;
        else
            transform.localScale = scale;
        this.isShow = isShow;
    }
    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
