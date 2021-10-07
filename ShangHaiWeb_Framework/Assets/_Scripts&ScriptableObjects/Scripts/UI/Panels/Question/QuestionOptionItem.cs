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

    //�����е�����ť�ĵ���¼�
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
                Debug.Log("δע���¼�");
            }
        });
    }

   
}
