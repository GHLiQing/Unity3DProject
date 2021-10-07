using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishTaskItem : MonoBehaviour {
    private Text nameText;
    private Text stateText;


    public void SetInfo(string name)
    {
        nameText = transform.Find("Name").GetComponent<Text>();
        stateText = transform.Find("Text").GetComponent<Text>();
        nameText.text = name;
        stateText.text = "已完成";
    }


}
