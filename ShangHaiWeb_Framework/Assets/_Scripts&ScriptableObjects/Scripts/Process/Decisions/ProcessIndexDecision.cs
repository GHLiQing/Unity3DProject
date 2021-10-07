using UnityEngine;
/// <summary>
/// 过渡子类
/// 流程类项目常用，当Step的stepProcessIndex大于target时，返回True，跳转至对应Transition的TrueStep
/// 此类为可配置文件，在Project面板下创建.asset文件后修改Target到你需要的数值。在拖给对应Step的Transition即可
/// </summary>
[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Decisions/ProcessIndex")]
public class ProcessIndexDecision : Decision
{
    public int target = 10;
    public override bool Decide(ProcessManager process)
    {
        return StepDecision(process);
    }
    private bool StepDecision(ProcessManager process)
    {
        return process.stepController.CurrentStep.stepProcessIndex >= target;
    }
}
