using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Custom/Step/Step_4")]
public class Step_4 : Step
{
    public override void CopyFromLastStep(ProcessManager process, Step lastStep)
    {
       // throw new System.NotImplementedException();
    }

    public override void OnExit(ProcessManager process, Step nextStep = null)
    {
       // throw new System.NotImplementedException();
    }
    public Transform player;
    public Transform root;
    WowMainCamera wow;
    CharacterMovement cm;

    Top top_ui;
    S4_rightPanel s4_right_Panel;
    public override void OnStart(ProcessManager process)
    {
        // throw new System.NotImplementedException();
        Debug.Log("Step_4");
        root = GameObject.Find("Root").transform;
        player = root.Find("PLAYER").transform;
        wow = player.Find("Main Camera").GetComponent<WowMainCamera>();
        cm = player.Find("Capsule").GetComponent<CharacterMovement>();
        SetPlayerPos(new Vector3(0.1739707f, 1.085f, 8.702379f),new Vector3(-261.75f, 3.960001f,0));

        top_ui = UIManager.Instance.GetPanel("Top") as Top;
        top_ui.Init();
        s4_right_Panel  = UIManager.Instance.GetPanel("RightPanel") as S4_rightPanel;
        s4_right_Panel.Init();

    }

    public override void OnUpdate(ProcessManager process)
    {
       // throw new System.NotImplementedException();
    }
    void SetPlayerPos(Vector3 pos, Vector3 angle)
    {
        player.Find("Capsule").transform.localPosition = pos;
        wow.x = angle.x;
        wow.y = angle.y;
    }
}
