using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTaskManager : Singleton<SingleTaskManager>
{
    [SerializeField]
    private List<SingleTask> singleTaskList = new List<SingleTask>();//执行的任务列表
	/// <summary>
	/// 添加到任务集合中
	/// </summary>
	/// <param name="s_task"></param>
    public void RegistTask(SingleTask s_task)
    {
        singleTaskList.Add(s_task);
    }
	/// <summary>
	/// 移除任务
	/// </summary>
	/// <param name="s_task"></param>
    public void ReMoveTask( SingleTask s_task)
    {
        singleTaskList.Remove(s_task);
    }

	/// <summary>
	/// 持续更新任务
	/// </summary>
    public void Update()
    {

        for (int i = 0; i < singleTaskList.Count; i++)
            singleTaskList[i].UpdateTask();

    
    }
    public SingleTask GetTask(string id)
    {
        SingleTask sk = singleTaskList.Find((p) => { return p.TaskID == id; });
        if (sk == null)
        {
            Debug.LogError("任务id:" + id + "未找到");
        }
        return sk;
    }
    //bool AllDependencedTasksComplete(SingleTask st)
    //{
    //    string[] d = st.DependencedTaskId;
    //    for (int i = 0; i < d.Length; i++)
    //    {
    //        SingleTask d_task = GetTask(d[i]);
    //        if (d_task.T_state == SingleTaskState.Complete)
    //            continue;
    //        else
    //            return false;
    //    }
    //    return true;
    //}
	/// <summary>
	///重置任务
	/// </summary>
	public void ResetAllTask()
	{
		for (int i = 0; i < singleTaskList.Count; i++)
		{
			singleTaskList[i].Reset();
		}
	}

}


