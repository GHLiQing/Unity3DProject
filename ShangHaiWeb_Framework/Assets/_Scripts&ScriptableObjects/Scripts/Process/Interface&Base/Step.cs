using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Step的抽象基类，继承于ScriptableObject
/// 每个Step最后都体现为一个.asset数据包，便于外部加载和步骤之间的切换
/// 和Animator的原理类似，OnUpdate为执行当前的动画，CheckTransitions为检测过渡的条件
/// </summary>
[Serializable]
public abstract class Step : ScriptableObject
{
    /// <summary>
    /// 当前对象的Update,做两件事，1：执行当前的Update保持逻辑运转；2：检查当前的过渡条件，看当前是否满足切换步骤的条件
    /// </summary>
    /// <param name="process"></param>
    public void UpdateStep(ProcessManager process)
    {
        //如果不是拷贝体，不能运行
        if (!isClone)
        {
            Debug.Log("当前使用的是源文件，请拷贝实例");
            return;
        }
        //OnStart都在此处运行
        if (!isStart)
        {
            isStart = true;

            if (myTask != null)
            {
                //检索正在进行的任务列表，如果表中有任务名称跟本步骤的任务名称相同，则拷贝该任务的数据给自己
                Task task = EvalutionManager.Instance.GetDoingTask(myTask);

                if (task == null)//如果正在执行的表中没有本步骤的任务，则将本步骤的任务设为正在进行中
                {
                    myTask.TaskState = TaskState.进行中;
                }
                else
                {
                    myTask = task;
                }
            }

            OnStart(process);

            return;
        }

        //计算此步骤的用时
        stepTimeElapsed += Time.deltaTime;

        OnUpdate(process);

        //检测过渡条件
        CheckTransitions(process);
    }

    /// <summary>
    /// 检查当前的过渡条件，看当前是否满足切换步骤的条件
    /// </summary>
    /// <param name="process"></param>
    private void CheckTransitions(ProcessManager process)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceeded = transitions[i].decision.Decide(process);

            if (decisionSucceeded)
            {
                process.stepController.ChangeStep(process, transitions[i].trueStep.Clone());
            }
            else
            {
                process.stepController.ChangeStep(process, transitions[i].falseStep.Clone());
            }
        }
    }

    #region  Field
    [SerializeField]
    /// <summary>
    /// 此步骤是否拷贝，此变量用来保障每个步骤都用拷贝出来的实例，不用源文件
    /// </summary>
    private bool isClone = false;

    public bool IsClone
    {
        get { return isClone; }
    }

    [SerializeField]
    /// <summary>
    /// 是否开始运行
    /// </summary>
    private bool isStart = false;

    /// <summary>
    /// 步骤名称，可用于检索
    /// </summary>
    public string stepName = "";

    /// <summary>
    /// 过渡，和Animator的原理相同，每个Step都可以有多个过渡对象，每个过渡对象包含一个过渡条件和对应过渡检测返回True或False后跳转的两个Step
    /// RemainStep代表保持当前状态不跳转
    /// </summary>
    public Transition[] transitions;

    /// <summary>
    /// 当前步骤的资源列表
    /// 可以将当前步骤用到的所有资源拖入其中，方便一起打成assetbundle包加载
    /// 比如实现跳步骤功能，把对应的预制体放进来，利用该预制体还原场景
    /// </summary>
    public UnityEngine.Object[] myPrefabResource;

    /// <summary>
    /// 当前步骤运行的时间,用于计时
    /// </summary>
    public float stepTimeElapsed = 0f;

    /// <summary>
    /// 用于逻辑控制
    /// </summary>
    public int stepProcessIndex = 0;

    [SerializeField]
    /// <summary>
    /// 用于逻辑控制
    /// </summary>
    protected int index = 0;

    /// <summary>
    /// 是否在Step结束时把此Step的task状态设置为已完成
    /// </summary>
    public bool finishTaskOnExit = true;

    /// <summary>
    /// 当前步骤的任务 ScriptableObject，以配置文件形式存在
    /// </summary>
    public Task myTask;
    #endregion

    #region Abstract Function
    /// <summary>
    /// 切换步骤时最先运行，可以从上一个步骤拷贝一些需要的数据
    /// </summary>
    /// <param name="process"></param>
    /// <param name="lastStep"></param>
    public abstract void CopyFromLastStep(ProcessManager process, Step lastStep);
    /// <summary>
    /// 切入当前step时执行一次
    /// </summary>
    /// <param name="process"></param>
    /// <param name="lastStep"></param>
    public abstract void OnStart(ProcessManager process);
    /// <summary>
    /// 处于当前step时一直执行
    /// </summary>
    /// <param name="process"></param>
    public abstract void OnUpdate(ProcessManager process);
    /// <summary>
    /// 离开本Step时执行一次
    /// </summary>
    /// <param name="process"></param>
    /// <param name="nextStep"></param>
    public abstract void OnExit(ProcessManager process, Step nextStep = null);
    #endregion

    #region Virtual

    /// <summary>
    /// 为了运行时不影响源.asset文件，每次跳转到该步骤时都拷贝一份新的实例
    /// </summary>
    /// <returns></returns>
    public virtual Step Clone()
    {
        if (this.GetType() == typeof(Remain_Step))
        {
            return this;
        }

        isClone = true;

        Step target = Instantiate(this);

        if (target.myTask != null)
        {
            target.myTask = target.myTask.Clone();
        }
        else
        {
            Debug.Log(target.name + "的Task为空");
        }

        isClone = false;

        return target;
    }
    #endregion
}
/// <summary>
/// 过渡类，每个Step可以包含多个过渡 
/// </summary>
[System.Serializable]
public class Transition
{
    public Decision decision;//过渡条件 ScriptableObject 以.asset可配置文件形式存在
    public Step trueStep;//decision判断为True时跳转的状态
    public Step falseStep;//decision判断为False时跳转的状态
}

/// <summary>
/// Decision抽象类，可根据不同条件可以拓展
/// </summary>
[Serializable]
public abstract class Decision : ScriptableObject
{
    public abstract bool Decide(ProcessManager process);
}



