using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Custom/Step/Step_3")]
public class Step_3 : Step
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
    S3_rightPanel s3_rpanel;
    public override void OnStart(ProcessManager process)
    {
        
        Debug.Log("step_3");
        root = GameObject.Find("Root").transform;

        player = root.Find("PLAYER").transform;
    
        wow = player.Find("Main Camera").GetComponent<WowMainCamera>();
        cm = player.Find("Capsule").GetComponent<CharacterMovement>();

        SetPlayerPos(new Vector3(-1.019549f, -0.4349999f, 9.076328f),new Vector3(163.25f, 8.039998f,0));

        top_ui = UIManager.Instance.GetPanel("Top") as Top;
        top_ui.Init();
        s3_rpanel  = UIManager.Instance.GetPanel("RightPanel") as S3_rightPanel;
        s3_rpanel.Init();
    }

    public override void OnUpdate(ProcessManager process)
    {
       // throw new System.NotImplementedException();
    }
   

    void SetPlayerPos(Vector3 pos, Vector3 angle)
    {
        player.Find("Capsule").transform.localPosition= pos;
        wow.x = angle.x;
        wow.y = angle.y;
    }
}
