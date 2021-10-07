using UnityEngine;
using System;
/// <summary>
/// 过渡子类
/// 流程类项目常用，当Step的stepProcessIndex大于target时，返回True，跳转至对应Transition的TrueStep
/// 此类为可配置文件，在Project面板下创建.asset文件后修改Target到你需要的数值。在拖给对应Step的Transition即可
/// </summary>
[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Decisions/RayDetect")]
public class RayDetectDecision : Decision
{
    /// <summary>
    /// 检测间隔帧数
    /// </summary>
    public int intervalFrames = 30;

    private int index = 0;
    public override bool Decide(ProcessManager process)
    {
        return ClickDecision(process);
    }

    private bool ClickDecision(ProcessManager process)
    {
        //index++;
        //if (index % intervalFrames == 0)
        //{
        //  //  Example3_Step step = process.stepController.CurrentStep as Example3_Step;
        //    return step.isClick;
        //}
        return false;
    }
}
