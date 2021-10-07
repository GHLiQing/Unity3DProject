using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoingTaskItem : MonoBehaviour {
    private Transform top;
    private Text nameText;
    private Text stateText;
    private Text introduce;

	
    public void SetInfo(string name, string introduce)
    {
        transform.Find("Top").Find("Name").GetComponent<Text>().text = name;
        transform.Find("Top").Find("Text").GetComponent<Text>().text = "进行中";
        transform.Find("Introduce").GetComponent<Text>().text = introduce;
    }

    public void ChangeToFinish()
    {
        transform.Find("Top").Find("Text").GetComponent<Text>().text = "已完成";
        DestroyImmediate(transform.Find("Introduce").gameObject);
    }
}
