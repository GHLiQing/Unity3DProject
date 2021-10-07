using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 所有Process的处理中心，提供暂停、开始process的功能 (步骤进度过程)
/// </summary>
public class ProcessManager : Singleton<ProcessManager>
{
    public StepController_FSM stepController;//Fsm对象 状态机  关联关系 

    /// <summary>
    /// 所有逻辑的Update
    /// </summary>
    private void Update()
    {
		//Debug.Log("ProcessManager Update");
        if (stepController != null)
        {
            stepController.UpdateCurrentStep(this);
            EventPool<UIData>.GetInstance.Update();
			EventPool<StrData>.GetInstance.Update();
			EventPool<TestData>.GetInstance.Update();
        }
    }

    /// <summary>
    /// 初始化ProcessMananger，每个场景都会挂Root，所以每次切换场景都换执行一次Init()  
    /// 可以在此处做根据不同场景的初始化
	/// 在Root 中 awake执行一次
    /// </summary>
    public override void Init()
    {
        base.Init();
        Root root = GameObject.Find("Root").GetComponent<Root>();
        stepController = new StepController_FSM(root.allSteps, root.currentStep, root.remainStep, root.isRunning);
    }
    public void StopProcess()
    {
        if (stepController != null && stepController.CurrentStep != null)
        {
            stepController.CurrentStep.OnExit(this);
            Debug.Log("clear");
            stepController = null;
        }
    }

}
