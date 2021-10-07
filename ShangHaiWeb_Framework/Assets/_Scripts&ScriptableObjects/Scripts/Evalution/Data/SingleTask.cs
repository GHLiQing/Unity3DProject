using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 单独的任务 
/// </summary>
public class SingleTask
{
    private string taskName;//任务名称
    private string taskDescription;//任务描述
    private string taskId;//任务id 唯一标识
    private SingleTaskState t_state;//任务状态
                                    //    private string[] dependencedTaskId;//前置任务
                                    //  private List<SingleTask> singleTaskList;//所有任务列表
    private List<SingleTask> dependenceTaskList = new List<SingleTask>();//前置任务列表 

    private bool isCompleted = false;

    public UnityAction<SingleTask> OnComplete;

    public UnityAction<SingleTask> OnUpdate;

    public UnityAction<SingleTask> OnStart;


    public string TaskName { get { return taskName; } }
    public string TaskID { get { return taskId; } }

    //public string[] DependencedTaskId { get => dependencedTaskId; }
    public SingleTaskState T_state { get { return t_state; } set { t_state = value; } }//通过设置此状态来控制任务的初始化与结束

    public bool IsCompleted//判断任务是否完成的依据
    {
        get
        {
            return isCompleted;
        }
    }
    public SingleTask(string id, string name, UnityAction<SingleTask> OnStart = null, UnityAction<SingleTask> OnUpdate = null, UnityAction<SingleTask> Onend = null)
    {
        taskId = id;
        taskName = name;
        this.OnStart = OnStart;
        this.OnUpdate = OnUpdate;
        OnComplete = Onend;
        t_state = SingleTaskState.CanStart;
    }
    #region public function
    public void UpdateTask()
    {
        //   Debug.Log("state:"+t_state+"  "+ AllDependencedTasksComplete());
        if (t_state == SingleTaskState.NotStart)
        {
            return;
        }
        if (!AllDependencedTasksComplete())
        {
            return;
        }
        else if (t_state == SingleTaskState.CanStart)
        {
            if (OnStart != null)
                OnStart(this);
            InitTask();
            t_state = SingleTaskState.Running;
        }
        else if (t_state == SingleTaskState.Running)
        {
            if (OnUpdate != null)
                OnUpdate(this);
            OnUpdateTask();
        }

        if (t_state == SingleTaskState.Complete && !isCompleted)//保证完成后立即执行complete
        {
            if (OnComplete != null)
                OnComplete(this);
            OnCompleteTask();
            isCompleted = true;
            Debug.Log("taskid:" + taskId + " complete");
        }
    }
    /// <summary>
    /// 初始化时执行
    /// </summary>
    public virtual void InitTask()
    {

    }
    /// <summary>
    /// 每帧执行
    /// </summary>
    public virtual void OnUpdateTask()
    {

    }
    /// <summary>
    /// 完成时执行
    /// </summary>
    public virtual void OnCompleteTask()
    {

    }
	/// <summary>
	/// 追加任务到集合中 在执行当前任务的时候 判断前面任务是否完成 才能进行接下来的任务操作
	/// </summary>
	/// <param name="tasks"></param>
    public void SetDependencedTask(SingleTask[] tasks)
    {
        dependenceTaskList.AddRange(tasks);
    }
    public void SetDependencedTask(SingleTask tasks)
    {
        if (!dependenceTaskList.Contains(tasks))
            dependenceTaskList.Add(tasks);
        else
            Debug.LogError("already contain task:" + tasks.TaskID);
    }

    #endregion

    /// <summary>
    /// 检测所有前置任务是否完成
    /// </summary>
    /// <returns></returns>
    bool AllDependencedTasksComplete()
    {
        for (int i = 0; i < dependenceTaskList.Count; i++)
        {
            if (dependenceTaskList[i].t_state == SingleTaskState.Complete)
                continue;
            else
                return false;
        }
        return true;
    }
	/// <summary>
	/// 重置任务
	/// </summary>
	public void Reset()
	{

	}
}
public enum SingleTaskState
{
    NotStart,//不可运行
    CanStart,
    Running,
    Complete,
}