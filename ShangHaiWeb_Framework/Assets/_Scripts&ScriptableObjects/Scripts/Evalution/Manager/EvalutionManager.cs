using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 任务控制器
/// </summary>
public class EvalutionManager : Singleton<EvalutionManager>
{
    [SerializeField]
    private List<Task> taskList;
    /// <summary>
    /// 总任务列表 源文件 只读
    /// </summary>
    public List<Task> TaskList
    {
        get
        {
            return taskList;
        }
    }
    /// <summary>
    /// 已完成的任务列表
    /// </summary>
    public List<Task> finishedTasksList = new List<Task>();

    /// <summary>
    /// 正在进行的任务列表
    /// </summary>
    public List<Task> doingTasksList = new List<Task>();
    /// <summary>
    /// 当返回首场景时，是否清空已完成和正在进行的任务列表
    /// </summary>
    public bool clearFinishedAndDoingTasksListWhenReturnFistScene = true;
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        base.Init();

        if (clearFinishedAndDoingTasksListWhenReturnFistScene)//如果为true，则回到首场景时清空两个表
        {
            if (SceneManager.GetActiveScene().buildIndex == 0 && SceneManager.sceneCount >= 1)
            {
                finishedTasksList = new List<Task>();
                doingTasksList = new List<Task>();
            }
        }

        if (taskList == null || taskList.Count == 0)//读取任务源文件
        {
            Task[] tasks = Resources.LoadAll<Task>("Tasks");

            taskList = new List<Task>(tasks);
        }
    }
    /// <summary>
    /// 获取任务的拷贝体
    /// </summary>
    /// <param name="taskName"></param>
    /// <returns></returns>
    public Task GetTask(string taskName)
    {
        for (int i = 0; i < taskList.Count; i++)
        {
            if (taskList[i].taskName == taskName)
            {
                return taskList[i].Clone();
            }
        }
        Debug.Log("未找到Task：" + taskName);
        return null;
    }
    /// <summary>
    /// 向已完成任务列表中添加项
    /// </summary>
    /// <param name="task"></param>
    public void AddFinishedTask(Task task)
    {
        RemoveDoingTask(task);

        for (int i = 0; i < finishedTasksList.Count; i++)
        {
            if (finishedTasksList[i].taskName == task.taskName)
            {
                finishedTasksList[i] = task;
                return;
            }
        }
        finishedTasksList.Add(task);

      //  UIManager.Instance.OnFinishedTaskListChange();
    }
    /// <summary>
    /// 向正在进行的任务列表中添加项
    /// </summary>
    /// <param name="task"></param>
    public void AddDoingTask(Task task)
    {
        for (int i = 0; i < doingTasksList.Count; i++)
        {
            if (doingTasksList[i].taskName == task.taskName)
            {
                doingTasksList[i] = task;
                return;
            }
        }
        doingTasksList.Add(task);

     //   UIManager.Instance.OnDoingTaskListChange();
    }
    /// <summary>
    /// 从正在进行的任务列表中获取任务
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public Task GetDoingTask(Task task)
    {
        for (int i = 0; i < doingTasksList.Count; i++)
        {
            if (doingTasksList[i].name == task.name)
            {
                return doingTasksList[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 移除正在进行的任务列表中的某项任务
    /// </summary>
    /// <param name="task"></param>
    private void RemoveDoingTask(Task task)
    {
        for (int i = 0; i < doingTasksList.Count; i++)
        {
            if (doingTasksList[i].taskName == task.taskName)
            {
                doingTasksList.Remove(doingTasksList[i]);
                return;
            }
        }
    }
    //现在所完成任务的总分
    public float CalculateScore()
    {
        float score = 0;
        for (int i = 0; i < finishedTasksList.Count; i++)
        {
            score += finishedTasksList[i].CurrentScore;
        }
        return score;
    }
    //完成了任务总量的百分之多少
    public float GetFinishedTaskPercent()
    {
        return (float)finishedTasksList.Count / taskList.Count;
    }
}
