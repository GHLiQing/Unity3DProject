using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "Custom/Step/Step_5")]
public class Step_5 : Step
{
    public override void CopyFromLastStep(ProcessManager process, Step lastStep)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnExit(ProcessManager process, Step nextStep = null)
    {
        EventPool<UIData>.GetInstance.UnSubscribeEvent((int)EventID.MovePlayer, MovePlayer);
    }
    public Transform player;
    public Transform root;
    WowMainCamera wow;
    CharacterMovement cm;
    Camera mainCamera;

    Top top_ui;
    S5_rightPanel s5_rPanel;

  
    public override void OnStart(ProcessManager process)
    {
      
        Debug.Log("step_5");
        root = GameObject.Find("Root").transform;
        player = root.Find("PLAYER").transform;
        wow = player.Find("Main Camera").GetComponent<WowMainCamera>();
        mainCamera = player.Find("Main Camera").GetComponent<Camera>();
        cm = player.Find("Capsule").GetComponent<CharacterMovement>();
        SetPlayerPos(new Vector3(1.096527f, 1.085f, 8.681348f),new Vector3(-52.6f, 3.960001f,0));

        top_ui = UIManager.Instance.GetPanel("Top") as Top;
        top_ui.Init();
        s5_rPanel = UIManager.Instance.GetPanel("RightPanel") as S5_rightPanel;
        s5_rPanel.Init();

        

        EventPool<UIData>.GetInstance.SubscribeEvent((int)EventID.MovePlayer, MovePlayer);
    }

    public override void OnUpdate(ProcessManager process)
    {
       
    }
    void SetPlayerPos(Vector3 pos, Vector3 angle)
    {
        player.Find("Capsule").transform.localPosition = pos;
        wow.x = angle.x;
        wow.y = angle.y;
    }
    //===========================订阅事件======================
    void MovePlayer(object sender,BaseEvent be)
    {
        MovePlayerData mpd = be.obj as MovePlayerData;
        wow.gameObject.WowDoMove_X_Y(mpd.pos,mpd.angle.x,mpd.angle.y,mpd.t,Ease.Linear,()=> { },()=> 
        {
            //  Application.ExternalCall("UnityToJs", "理论学习/口腔科/根管治疗");
            DataManager.s05 = "根管治疗";
            Application.ExternalCall("UnityToJs", DataManager.GetStr(StrType._s05));
        },Space.Self);
    }
}
