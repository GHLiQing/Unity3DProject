using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class QuestionOptionItem : MonoBehaviour
{
    public string ItemName = "";
    public int index;
    //public Tool tool;

    //背包中单个按钮的点击事件
    public UnityAction<int, bool> QuestionItemClick;
    
    private void Start()
    {
        GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
        {

            if (QuestionItemClick != null)
            {
                QuestionItemClick(index, isOn);
            }
            else
            {
                Debug.Log("未注册事件");
            }
        });
    }

   
}
