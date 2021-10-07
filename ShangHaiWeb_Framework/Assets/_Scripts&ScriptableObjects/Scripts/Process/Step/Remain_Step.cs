using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 空Step，用于保持当前Step
/// </summary>
[CreateAssetMenu(menuName = "Custom/Step/Remain")]
public class Remain_Step : Step
{
    public override void CopyFromLastStep(ProcessManager process, Step lastStep)
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit(ProcessManager process, Step nextStep = null)
    {
        throw new System.NotImplementedException();
    }

    public override void OnStart(ProcessManager process)
    {
        throw new System.NotImplementedException();
    }


    public override void OnUpdate(ProcessManager process)
    {
        throw new System.NotImplementedException();
    }
}
