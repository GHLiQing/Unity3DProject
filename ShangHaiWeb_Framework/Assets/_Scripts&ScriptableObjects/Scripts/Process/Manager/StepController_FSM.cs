using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Step的状态机类
/// 用于控制Step之间的切换、运行
/// 一个fsm控制所有step进行逻辑上的判断
/// </summary>
[System.Serializable]
public class StepController_FSM
{
    #region Field
    /// <summary>
    /// 步骤列表
    /// </summary>
    [SerializeField]
    private List<Step> allSteps; //聚合关系 

    [SerializeField]
    //当前的运行的Step
    private Step currentStep;

    public Step CurrentStep
    {
        get { return currentStep; }
    }

    //保持的Step，即空Step，用于判断并保持当前状态
    public Step remainStep;

    [SerializeField]
    //当前FSM是否运转
    private bool isRunning = false;
    /// <summary>
    /// 只读，是否Running
    /// </summary>
    public bool IsRunning
    {
        get { return isRunning; }
    }

    [SerializeField]
    private bool isStart = false;
    #endregion

    #region Init
    /// <summary>
    /// 初始化
    /// </summary>
    public StepController_FSM(List<Step> allSteps, Step currentStep, Step remainStep, bool isRunning)
    {
        this.allSteps = allSteps;
        this.currentStep = currentStep;
        this.remainStep = remainStep;
        this.isRunning = isRunning;
    }
	#endregion

	/// <summary>
	/// Update当前的Step依赖关系 ProcessManager依赖于fsm
	/// </summary>
	/// <param name="process"></param>
	public void UpdateCurrentStep(ProcessManager process)
    {
        if (!isStart && isRunning)
        {
			//开启状态机
            Start();
            return;
        }
        else if (!isRunning && isStart)
        {
            isStart = false;
        }
        //运行当前Step的OnUpdate 和CheckTransitions
        if (currentStep != null && isRunning)
        {
			//执行当前状态update
            currentStep.UpdateStep(process);
        }
    }
    /// <summary>
    /// 切换CurrentStep
    /// </summary>
    /// <param name="process"></param>
    /// <param name="nextState"></param>
    public void ChangeStep(ProcessManager process, Step nextState)
    {
        //此处加上判断，当前要切换的step不是RemainStep才进行切换
        if (nextState != remainStep)
        {
            Step lastStep = currentStep;

            //执行上一个Step的OnExit
            lastStep.OnExit(process, nextState);

            if (lastStep != null && lastStep.myTask != null && lastStep.finishTaskOnExit)
            {
                lastStep.myTask.TaskState = TaskState.已完成;
            }
            //切换
            currentStep = nextState;

            //从上一个Step拷贝信息
            currentStep.CopyFromLastStep(process, lastStep);

            lastStep = null;
        }
    }

    /// <summary>
    /// 切换Step，增加一个参数，此方法可用于切换小步骤
    /// </summary>
    /// <param name="process"></param>
    /// <param name="nextState"></param>
    /// <param name="stepProcessIndex"></param>
    public void ChangeStep(ProcessManager process, Step nextState, int stepProcessIndex)
    {
        //此处加上判断，当前要切换的step不是RemainStep才进行切换
        if (nextState != remainStep)
        {
            Step lastStep = currentStep;

            //执行上一个Step的OnExit
            lastStep.OnExit(process, nextState);

            if (lastStep != null && lastStep.myTask != null && lastStep.finishTaskOnExit)
            {
                lastStep.myTask.TaskState = TaskState.已完成;
            }
            //切换
            currentStep = nextState;

            //设置
            currentStep.stepProcessIndex = stepProcessIndex;

            //从上一个Step拷贝信息
            currentStep.CopyFromLastStep(process, lastStep);

            lastStep = null;
        }
    }

    /// <summary>
    /// 开启状态机
    /// </summary>
    public void Start()
    {
        if (currentStep == null)
        {
            Debug.Log("CurrentStep为空");
            return;
        }

        if (!currentStep.IsClone) currentStep = currentStep.Clone();

        isRunning = true;

        isStart = true;

        Debug.Log("状态机已启动");
    }

    /// <summary>
    /// 关闭状态机
    /// </summary>
    public void Stop()
    {
        isRunning = false;

        Debug.Log("状态机已停止，重新开始将重新初始化");
    }

    /// <summary>
    /// 从List中获取Step，可用于手动切换步骤
    /// </summary>
    /// <param name="stepName"></param>
    /// <returns></returns>
    public Step GetStep(string stepName)
    {
        for (int i = 0; i < allSteps.Count; i++)
        {
            if (allSteps[i].stepName == stepName)
                return allSteps[i].Clone();
        }
        Debug.Log("未找到Step:" + stepName);
        return null;
    }

    /// <summary>
    /// 添加Step到列表中
    /// </summary>
    /// <param name="step"></param>
    public void AddStep(Step step)
    {
        if (!allSteps.Contains(step))
        {
            allSteps.Add(step);
        }
        else
        {
            Debug.Log("已添加Step:" + step.stepName);
        }
    }

    public void AddSteps(Step[] step)
    {
        for (int i = 0; i < step.Length; i++)
        {
            AddStep(step[i]);
        }
    }

    /// <summary>
    /// 从列表中移除Step
    /// </summary>
    /// <param name="stepName"></param>
    public void RemoveStep(string stepName)
    {
        Step step = GetStep(stepName);
        if (step != null)
        {
            allSteps.Remove(step);
        }
    }

    public void RemoveAllStep()
    {
        if (allSteps != null)
        {
            allSteps.Clear();
        }
    }
}
