using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class KnapsackItem : MonoBehaviour
{
    public string ItemName = "";

    public Tool tool;

    //背包中单个按钮的点击事件
    public UnityAction<string, Tool> OnKnapsackItemClick;

    private void Start()
    { 
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (OnKnapsackItemClick != null)
            {
                OnKnapsackItemClick(ItemName, tool);
            }
            else
            {
                Debug.Log("未注册事件");
            }

        });
    }
}
