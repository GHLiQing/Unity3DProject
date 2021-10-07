using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Custom/Step/Step_6")]
public class Step_6 : Step
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
    Camera mainCamera;

    Top top_ui;

    RayDetect ray;
    public override void OnStart(ProcessManager process)
    {
        Debug.Log("step_3");
        root = GameObject.Find("Root").transform;

        player = root.Find("PLAYER").transform;

        wow = player.Find("Main Camera").GetComponent<WowMainCamera>();
        cm = player.Find("Capsule").GetComponent<CharacterMovement>();
        mainCamera = player.Find("Main Camera").GetComponent<Camera>();
        SetPlayerPos(new Vector3(5.491395f, 1.085f, -0.010875f), new Vector3(-102.75f, 4.920001f, 0));

        top_ui = UIManager.Instance.GetPanel("Top") as Top;
        top_ui.Init();

        ray = new RayDetect();
        ray.AddRayEvent(RayEventType.DoubleClick, (tra) =>
        {
            Application.ExternalCall("UnityToJs", "图书资料");
        });
    }

    public override void OnUpdate(ProcessManager process)
    {
        ray.Update(mainCamera.ScreenPointToRay(Input.mousePosition), 5, (1 << 10), false);
    }

    void SetPlayerPos(Vector3 pos, Vector3 angle)
    {
        player.Find("Capsule").transform.localPosition = pos;
        wow.x = angle.x;
        wow.y = angle.y;
    }
}
