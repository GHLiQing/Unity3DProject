using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class Knapsack : Panel
{
    //单个道具按钮的组件预制体
    public KnapsackItem itemPrefab;

    //所有道具按钮
    public List<KnapsackItem> itemList;

    //按钮根目录
    public Transform itemParent;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        itemList = new List<KnapsackItem>();

        for (int i = 0; i < itemParent.childCount; i++)
        {
            itemList.Add(itemParent.GetChild(i).GetComponent<KnapsackItem>());
        }
    }

    /// <summary>
    /// 开关面板
    /// </summary>
    /// <param name="display"></param>
    public override void SetPanelDisplay(bool display)
    {
        isDisplay = display;
        //自定义方式开关相关UI
        myRectTransfrom.gameObject.SetActive(display);
    }

    /// <summary>
    /// 设置背包按钮的点击事件
    /// </summary>
    /// <param name="OnStepToolItemClick"></param>
    public void SetKnapsackItemClickEvent(UnityAction<string, Tool> OnStepToolItemClick)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].OnKnapsackItemClick = OnStepToolItemClick;
        }
    }

    /// <summary>
    /// 重新遍历Item的父物体，将所有Item加入到表中
    /// </summary>
    public void AddItem()
    {
        itemList = new List<KnapsackItem>();
        for (int i = 0; i < itemParent.childCount; i++)
        {
            itemList.Add(itemParent.GetChild(i).GetComponent<KnapsackItem>());
        }
    }
    /// <summary>
    /// 加入一个item到背包中
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(KnapsackItem item)
    {
        if (!itemList.Contains(item))
        {
            
            item.transform.SetParent(itemParent);
            itemList.Add(item);
        }
    }

    /// <summary>
    /// 删除单个道具按钮
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(KnapsackItem item)
    {
        if (itemList.Contains(item))
        {
            itemList.Remove(item);
            Destroy(item);
        }
    }
    /// <summary>
    /// 删除所有道具按钮
    /// </summary>
    public void RemoveAllItem()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            Destroy(itemList[i]);
        }
        itemList = new List<KnapsackItem>();
    }
}
