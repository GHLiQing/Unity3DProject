using UnityEngine;
/// <summary>
/// 过渡子类
/// 流程类项目常用，当Step的stepProcessIndex大于target时，返回True，跳转至对应Transition的TrueStep
/// 此类为可配置文件，在Project面板下创建.asset文件后修改Target到你需要的数值。在拖给对应Step的Transition即可
/// </summary>
[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Decisions/Timmer")]
public class TimmerDecision : Decision
{
    public float targetTime;
    public override bool Decide(ProcessManager process)
    {
        return TimeDecision(process);
    }
    private bool TimeDecision(ProcessManager process)
    {
        return process.stepController.CurrentStep.stepTimeElapsed >= targetTime;
    }
}
                            