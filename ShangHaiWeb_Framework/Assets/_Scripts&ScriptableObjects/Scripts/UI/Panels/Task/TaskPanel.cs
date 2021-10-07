using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 任务面板
/// </summary>
public class TaskPanel : Panel
{
    public Transform taskContent;
    public Transform finishTaskContent;//已完成任务项的目录
    public Transform doingTaskContent;//正在进行的任务项的目录

    private Animator anim;

    public GameObject finishItemPrefab;//已完成任务项的组件预制体 
    public GameObject doingItemPrefab;//正在进行的任务项的组件预制体

    private Button CloseBtn;

    protected override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();

        transform.Find("Close").GetComponent<Button>().onClick.AddListener(() =>
        {
            SetPanelDisplay(false);
        });

        UpdateFinishTask();
    }

    #region FinishTask
    public void UpdateFinishTask()//更新已完成任务项
    {
        ClearFinishTask();
        MyUtilities.DelayToDo(0.05f, () =>
        {
            AddAllFinishTask();
        });
    }
    private void ClearFinishTask()//清除已完成任务项
    {
        for (int i = finishTaskContent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(finishTaskContent.GetChild(i).gameObject);
        }
    }
    private void AddAllFinishTask()//添加已完成任务项
    {
        for (int i = 0; i < EvalutionManager.Instance.finishedTasksList.Count; i++)
        {
            AddOneFinishTask(EvalutionManager.Instance.finishedTasksList[i]);
        }
    }
    void AddOneFinishTask(Task task) //
    {
        GameObject go = Instantiate(finishItemPrefab) as GameObject;
        go.transform.SetParent(finishTaskContent);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
        go.transform.GetComponent<FinishTaskItem>().SetInfo(task.name.Split('(')[0]);
    }
    #endregion
    #region  DoingTask
    public void UpdateDoingTask()//更新正在进行的任务项
    {
        RemoveAllDoingTask();
        AddAllDoingTask();
        UpdateDoingContentLayout();
    }
    private void AddAllDoingTask()
    {
        for (int i = 0; i < EvalutionManager.Instance.doingTasksList.Count; i++)
        {
            AddOneDoingTask(EvalutionManager.Instance.doingTasksList[i]);
        }
    }
    private void RemoveAllDoingTask()
    {
        for (int i = doingTaskContent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(doingTaskContent.GetChild(i).gameObject);
        }
    }
    private void AddOneDoingTask(Task task)
    {
        GameObject go = Instantiate(doingItemPrefab) as GameObject;
        go.transform.SetParent(doingTaskContent);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
        go.transform.GetComponent<DoingTaskItem>().SetInfo(task.name.Split('(')[0], task.taskIntroduce);

        //UpdateAllContentLayout();
    }
    private void RemoveDoingTask()
    {
        for (int i = 0; i < doingTaskContent.childCount; i++)
        {
            DestroyImmediate(doingTaskContent.GetChild(i).gameObject);
        }
    }
    private void RemoveDoingTask(Task task)
    {
        for (int i = 0; i < doingTaskContent.childCount; i++)
        {
            if (doingTaskContent.GetChild(i).GetComponent<DoingTaskItem>().name == task.name.Split('(')[0])
                DestroyImmediate(doingTaskContent.GetChild(i).gameObject);
        }
    }
    #endregion
    public void UpdateAll()
    {
        UpdateDoingTask();
        UpdateFinishTask();
    }
    void UpdateDoingContentLayout()
    {
        doingTaskContent.transform.GetComponent<VerticalLayoutGroup>().enabled = false;
        MyUtilities.DelayToDo(0.05f, () =>
        {
            if (doingTaskContent != null)
                doingTaskContent.transform.GetComponent<VerticalLayoutGroup>().enabled = true;
        });
    }
    public override void SetPanelDisplay(bool display)
    {
        //throw new System.NotImplementedException();


        anim.SetBool("isOpen", display);

    }
}
