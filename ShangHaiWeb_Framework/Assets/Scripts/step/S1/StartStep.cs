using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 当前temp为.asset文件 当前步骤执行的逻辑
/// </summary>
[CreateAssetMenu(menuName = "Custom/Step/StartStep")]

public class StartStep : Step
{
    public Transform player;
    // public Transform camera;
    public Transform root;
    Animation ani_camera;
    WowMainCamera wow;
    Camera mainCamera;

    CharacterMovement cm;
    RightPanel rPanel;

    PostEffectManager pem;
    public override void CopyFromLastStep(ProcessManager process, Step lastStep)
    {

    }

    public override void OnExit(ProcessManager process, Step nextStep = null)
    {
		 EventPool<UIData>.GetInstance.UnSubscribeEvent((int)EventID.SkipAni, SkipAni);
		 EventPool<TestData>.GetInstance.UnSubscribeEvent((int)EventID.Test, Test);
	}

    public override void OnStart(ProcessManager process)
    {
		Debug.Log("OnStart");
        //    ClassData cd = new ClassData();
        //   cd.name = "123";
        //     AllDatas ad = new AllDatas();
        //   ad.allList = new ClassData[1];
        //   ad.allList[0] = cd;
        //    ad.allList.Add(cd);
        // string sss = JsonUtility.ToJson(ad);
        //    Debug.Log(sss);


        //   ad = JsonUtility.FromJson<AllDatas>(sss);

        //   Debug.Log(ad.allList[0].name);

        root = GameObject.Find("Root").transform;

        player = root.Find("PLAYER").transform;
        ani_camera = player.Find("Main Camera").GetComponent<Animation>();
        mainCamera= player.Find("Main Camera").GetComponent<Camera>();
        wow = player.Find("Main Camera").GetComponent<WowMainCamera>();
        cm = player.Find("Capsule").GetComponent<CharacterMovement>();

        SetPlayerPos(new Vector3(0.1739706f, 1.085f, 8.702379f), new Vector3(-179.5f, 3.960001f, 0));
        rPanel = UIManager.Instance.GetPanel("RightPanel") as RightPanel;
        rPanel.Init();
        pem = mainCamera.GetComponent<PostEffectManager>();
       // pem.SetBlack_FadeOut(mainCamera, 0.5f,()=> { });

        if (Loading.once)
        {
            Loading.once = !Loading.once;
            PlayAni(true);
            CanControl(false);
            rPanel.SetRightPanel(false);
        }
        else
        {

            PlayAni(false);
            CanControl(false);
            rPanel.SetRightPanel(true);
        }
        EventPool<UIData>.GetInstance.SubscribeEvent((int)EventID.SkipAni, SkipAni);
		EventPool<TestData>.GetInstance.SubscribeEvent((int)EventID.Test, Test);
	}

	public override void OnUpdate(ProcessManager process)
    {
        if (ani_camera != null)
        {
            ani_camera.DetectionAnimation("camera01", null, null, () =>
               {
                   PlayAni(false);
                   rPanel.SetRightPanel(true);
               });
        }
    }
    void PlayAni(bool a)
    {
        ani_camera.enabled = a;
        //wow.enabled = !a;
        //cm.enabled = !a;
    }

    //=================================
    void SetPlayerPos(Vector3 pos, Vector3 angle)
    {
        player.Find("Capsule").transform.localPosition = pos;
        wow.x = angle.x;
        wow.y = angle.y;
    }
    //===================订阅事件======================//
    void SkipAni(object sender, BaseEvent be)
    {
        //pem.SetBlack_FadeIn(mainCamera, 0.5f, () => 
        //{
        //    PlayAni(false);  
        //    rPanel.SetRightPanel(true);
        //    mainCamera.transform.localPosition = new Vector3(0.1739706f, 1.785f, 8.702379f);
        //    mainCamera.transform.localEulerAngles = new Vector3(3.96f, 180.5f,0);
        //    pem.SetBlack_FadeOut(mainCamera, 0.5f, () => 
        //    {

        //    });
        //});    
    }
	/// <summary>
	/// 测试订阅  
	/// </summary>
	/// <param name="o"></param>
	/// <param name="baseEvent"></param>
	 void Test(object sender, BaseEvent baseEvent)
	{
		GameObject go=   baseEvent.obj as GameObject;
		Debug.Log("共："+go.name);
		Debug.Log("测试订阅   Over");

	}
	public void CanControl(bool a)
    {
        wow.enabled = a;
        cm.enabled = a;
    }

}



