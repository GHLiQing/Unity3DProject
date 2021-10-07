using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public delegate void RayEventHandle<T>(T obj);
/// <summary>
/// 射线监测 通用类 by 秦汉 2018.1.22
/// </summary>
/// 

public enum RayEventType { Enter, Exit, Click, DoubleClick };
public class RayDetect
{
    private event RayEventHandle<Transform> ray_EnterEvent, ray_ExitEvent, ray_ClickEvent, ray_DoubleClickEvent;

    [SerializeField]
    private GameObject currentHitObj;

    [SerializeField]
    private bool isUseRay = false;//是否使用射线

    private RaycastHit hit;


    #region 对外接口
    /// <summary>
    /// 获取当前射线检测到的物体
    /// </summary>
    /// <returns></returns>
    public GameObject GetCurrentHitObj()
    {
        return currentHitObj;
    }
    /// <summary>
    /// 获取RaycastHit
    /// </summary>
    /// <returns></returns>
    public RaycastHit GetHitInfo()
    {
        return hit;
    }
    /// <summary>
    /// 设置事件
    /// </summary>
    /// <param name="ray_EnterEvent"></param>
    /// <param name="ray_ExitEvent"></param>
    /// <param name="ray_ClickEvent"></param>
    /// <param name="ray_DoubleClickEvent"></param>
    public RayDetect(RayEventHandle<Transform> ray_EnterEvent = null, RayEventHandle<Transform> ray_ExitEvent = null, RayEventHandle<Transform> ray_ClickEvent = null, RayEventHandle<Transform> ray_DoubleClickEvent = null)
    {
        this.ray_EnterEvent += ray_EnterEvent;

        this.ray_ExitEvent += ray_ExitEvent;

        this.ray_ClickEvent += ray_ClickEvent;

        this.ray_DoubleClickEvent += ray_DoubleClickEvent;

        isUseRay = true;
    }
    /// <summary>
    /// 检测Update
    /// </summary>
    public void Update(Ray ray, float checkdis, LayerMask laymask, bool ischeckUI)
    {
        if (isUseRay)
        {
            MouseRayCheck(ray, checkdis, laymask, ischeckUI);
        }
    }
    /// <summary>
    /// 退出射线
    /// </summary>
    public void DesRay()
    {
        if (currentHitObj != null)
        {
            ExitEvent(currentHitObj.transform);
            currentHitObj = null;
        }
        isUseRay = false;
    }
    /// <summary>
    /// 为射线添加事件
    /// </summary>
    public void AddRayEvent(RayEventType rEventType, RayEventHandle<Transform> rEvent)
    {
        switch (rEventType)
        {
            case RayEventType.Enter:
                ray_EnterEvent += rEvent;
                break;
            case RayEventType.Exit:
                ray_ExitEvent += rEvent;
                break;
            case RayEventType.Click:
                ray_ClickEvent += rEvent;
                break;
            case RayEventType.DoubleClick:
                ray_DoubleClickEvent += rEvent;
                break;
        }
    }
    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="rEventType"></param>
    /// <param name="rEvent"></param>
    public void RemoveRayEvent(RayEventType rEventType, RayEventHandle<Transform> rEvent)
    {
        switch (rEventType)
        {
            case RayEventType.Enter:
                ray_EnterEvent -= rEvent;
                break;
            case RayEventType.Exit:
                ray_ExitEvent -= rEvent;
                break;
            case RayEventType.Click:
                ray_ClickEvent -= rEvent;
                break;
            case RayEventType.DoubleClick:
                ray_DoubleClickEvent -= rEvent;
                break;
        }
    }
    /// <summary>
    /// 移除所有事件
    /// </summary>
    public void RemoveAllRayEvent()
    {
        ray_EnterEvent = null;
        ray_ExitEvent = null;
        ray_ClickEvent = null;
        ray_DoubleClickEvent = null;
    }
    #endregion

    void MouseRayCheck(Ray ray, float checkdis, LayerMask laymask, bool ischeckUI)
    {
        if (ischeckUI)
        {
            if(EventSystem.current!=null)
            if (EventSystem.current.IsPointerOverGameObject())//点到UI上消失
            {
                if (currentHitObj != null)
                {
                    ExitEvent(currentHitObj.transform);
                    currentHitObj = null;
                }
                return;
            }
        }

        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //  RaycastHit hit;
        if (Physics.Raycast(ray, out hit, checkdis, laymask, QueryTriggerInteraction.UseGlobal))
        {

            if (currentHitObj == null)
            {
                //第一次射到物体
                currentHitObj = hit.transform.gameObject;
                EnterEvent(currentHitObj.transform);
            }
            else if (currentHitObj != hit.transform.gameObject)
            {

                ExitEvent(currentHitObj.transform);
                //离开一个物体射到另一个物体
                currentHitObj = hit.transform.gameObject;

                EnterEvent(currentHitObj.transform);
            }
            else if (currentHitObj == hit.transform.gameObject)
            {
                ClickObj(currentHitObj.transform);      
            }
        }
        else
        {
            //没射到物体
            if (currentHitObj != null)
            {
                ExitEvent(currentHitObj.transform);
            }
            currentHitObj = null;
        }
    }
    /// <summary>
    /// 鼠标进入物体事件
    /// </summary>
    /// <param name="tra"></param>
    void EnterEvent(Transform tra)
    {


        if (ray_EnterEvent != null)
            ray_EnterEvent(tra);
        //if (ray_EnterEvent_2 != null)
        //    ray_EnterEvent_2(tra, true);

    }
    /// <summary>
    /// 鼠标离开物体事件
    /// </summary>
    /// <param name="tra"></param>
    void ExitEvent(Transform tra)
    {

        if (ray_ExitEvent != null)
            ray_ExitEvent(tra);
        //if (ray_ExitEvent_2 != null)
        //    ray_ExitEvent_2(tra, false);
    }

    /// <summary>
    /// 鼠标点击物体事件
    /// </summary>
    /// <param name="tra"></param>
    void ClickObj(Transform tra)
    {
        DoubleClick(() =>
        {
            if (ray_DoubleClickEvent != null)
                ray_DoubleClickEvent(tra);
        });
        if (Input.GetMouseButtonDown(0))
        {

            if (ray_ClickEvent != null)
            {
                ray_ClickEvent(tra);             
            }

        }
    }

    float t1, t2;
    /// <summary>
    /// 双击
    /// </summary>
    void DoubleClick(UnityAction DoubleEvent)
    {
        if (Input.GetMouseButtonDown(0))
        {
            t2 = Time.time;
            if (t2 - t1 < 0.4)
            {
                DoubleEvent();
            }
            else
            {

            }
            t1 = t2;
        }
    }
}
