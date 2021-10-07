using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*每个步骤会有很多细节的UI，这个类就是用来做单个步骤相关UI的拓展
 * 可以对应一个步骤创建一个新的UI逻辑的类并继承于此类，DetailsPanel中新建一个对应步骤的面板，步骤中用到的所有UI都放在这个面板下面
 * 新建的类挂在面板上，如DetailsPanel/Step1 ，保证该面板为激活状态，激活状态才能执行Awake将自己注册到UImanager中
 * 在自己的子类中通过UImanager的GetView填入面板名称可得到对应的对象，以此来跟UI进行交互
 * 所以 除项目负责人或指定人员外，其他人不允许擅自修改UImanager类，拓展对应步骤的UI只需要按照上面说的做就行。
 */


public abstract class Panel : MonoBehaviour
{

    public bool registeMyselfOnAwake = true;
    //将自己注册到UImanager的AllViewer中
    protected void RegisteMyself(string name)
    {
        if (UIManager.Instance != null)
        {
			Debug.Log("注册自己的面板在字典中");
            UIManager.Instance.RegisetPanel(name, this);
        }
        else
        {
            Debug.Log("UIManger为空");
        }
    }

    //UI面板，一般此脚本挂在空的父物体上，下一级才是UI面板
    protected RectTransform myRectTransfrom;

    protected bool isDisplay = true;
    public bool IsDisplay
    {
        get { return isDisplay; }
    }
	

    protected virtual void Awake()
    {
        if (registeMyselfOnAwake)
        {
            RegisteMyself(gameObject.name);
        }
        if (this.transform.childCount > 0)
        {
            myRectTransfrom = this.transform.GetChild(0).GetComponent<RectTransform>();
        }
    }
    /// <summary>
    /// 设置UI面板的锚点和位置（一般脚本挂在空的父物体上，修改的是下一级的UI）
    /// </summary>
    /// <param name="anchor">锚点x,y均为0-1，比如（1,1）为右上角，(1,0)为右下角，（0.5,0.5）为中间，（0,0）为左下角，(0,1)为左上角</param>
    /// <param name="position">相对于锚点的位置</param>
    public virtual void SetPanelPosition(Vector2 anchor, Vector2 position)
    {
        myRectTransfrom.anchorMax = anchor;
        myRectTransfrom.anchorMin = anchor;
        myRectTransfrom.anchoredPosition = position;
    }


    /// <summary>
    /// 开关面板的抽象方法
    /// </summary>
    /// <param name="display"></param>
    public abstract void SetPanelDisplay(bool display);

    protected virtual void OnDestroy()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnRegisetPanel(gameObject.name);
        }
        else
        {
            Debug.Log("UIManger为空");
        }
    }

    public virtual void Init()
    {
        FindObj();
        RegistEvent();
    }
    protected virtual void FindObj()
    { }
    protected virtual void RegistEvent()
    { }
	

}
