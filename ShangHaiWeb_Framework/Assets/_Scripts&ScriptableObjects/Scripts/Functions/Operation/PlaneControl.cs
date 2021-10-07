using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum CameraMoveType
{
    camera, wowcamera
};
/// <summary>
///平面内操控物体  by 秦汉 2018.1.22
/// 
/// </summary>
public class PlaneControl
{

    #region 内部变量
    #region Laser
    private Texture[] tietu_laser;//射线的贴图
    private LineRenderer lRenderer;//射线的线渲染器
    private GameObject laser;//射线起始点预制体
    private GameObject laserDot;//射线射到桌子上的点面片
    Transform laserTarget;
    #endregion
    #region  IK相关变量
    private bool isInIK = false;

    private HandType hand;  //左右手

    LinkAnimatorIK linkAnimatorIK;


    #endregion

    private Transform controlObj;//控制的物体

    private Transform plane;

    private Transform pivot;//鼠标的位置

    private bool isIncontrol;

    private List<Tweener> tweenerList = new List<Tweener>();//平滑移动的dotween

    private bool isLaser;//是否有射线

    private bool isFollowing;//是否跟随物体视角  //

    private CameraMoveType cameraFollowType;

    private Transform player;//wowtarget

    private WowMainCamera wow;

    private bool isLimitPos;//是否限制位置

    private Vector2 x_Range, y_Range, z_Range;//限制的范围

    private float playerMoveSpeed = 0.1f;//设置移动速度

    private Vector3 playerOffset = Vector3.zero;//摄像机 和物体的相对偏移

    private List<Transform> hitObj;//被触发的物体

    private Camera mainCamera;

    private RayDetect rdt;//射线类
    #endregion
    #region 内置方法

    private void ResetTweenList()
    {
        for (int i = 0; i < tweenerList.Count; i++)
        {
            if (tweenerList[i].IsActive())
            {
                tweenerList[i].Kill();
            }
        }
        tweenerList.Clear();
    }

    private Transform FindColliderObj(Transform obj)
    {
        obj.GetComponentInChildren<Collider>();
        if (obj.GetComponent<Collider>() != null)
        {
            return obj;
        }
        else
        {
            for (int i = 0; i < obj.childCount; i++)
            {
                if (FindColliderObj(obj.GetChild(i)) != null) return FindColliderObj(obj.GetChild(i));
            }
        }
        return null;
    }

    /// <summary>
    /// 摄像机跟随控制物体视角
    /// </summary>
    /// <param name="isfollowing"></param>
    private void FollowObj(bool isfollowing)
    {
        if (isfollowing)
        {
            if (cameraFollowType == CameraMoveType.wowcamera)
            {
                #region 摄像机控制Wow
                Rect limitRect = new Rect(Screen.width * 0.3f, Screen.height * 0.3f, Screen.width - Screen.width * 0.3f, Screen.height - Screen.height * 0.3f);//边界，鼠标超过边界即向鼠标方向移动

                float x = 0, z = 0;

                if (Input.mousePosition.x < limitRect.x || Input.mousePosition.x > limitRect.width)
                {
                    x = (Input.mousePosition.x < limitRect.x) ? Input.mousePosition.x - limitRect.x : Input.mousePosition.x - limitRect.width;
                }

                if (Input.mousePosition.y < limitRect.y || Input.mousePosition.y > limitRect.height)
                {
                    z = (Input.mousePosition.y < limitRect.y) ? Input.mousePosition.y - limitRect.y : Input.mousePosition.y - limitRect.height;
                }

                Vector3 targetDirection = new Vector3(x, 0, z);

                float y = wow.transform.rotation.eulerAngles.y;

                targetDirection = Quaternion.Euler(0, y, 0) * targetDirection * 0.03f;

                player.transform.Translate(targetDirection * Time.deltaTime * playerMoveSpeed, Space.World);

                player.transform.position = new Vector3(Mathf.Clamp(player.transform.position.x, (x_Range.x + playerOffset.x), (x_Range.y + playerOffset.x)), player.transform.position.y, Mathf.Clamp(player.transform.position.z, (z_Range.x + playerOffset.z), (z_Range.y + playerOffset.z)));

                wow.allowControl = false;

                wow.allowZoom = true;
                #endregion
            }
            else
            {
                #region 摄像机控制
                Rect limitRect = new Rect(Screen.width * 0.3f, Screen.height * 0.3f, Screen.width - Screen.width * 0.3f, Screen.height - Screen.height * 0.3f);//边界，鼠标超过边界即向鼠标方向移动

                float x = 0, z = 0;

                if (Input.mousePosition.x < limitRect.x || Input.mousePosition.x > limitRect.width)
                {
                    x = (Input.mousePosition.x < limitRect.x) ? Input.mousePosition.x - limitRect.x : Input.mousePosition.x - limitRect.width;
                }

                if (Input.mousePosition.y < limitRect.y || Input.mousePosition.y > limitRect.height)
                {
                    z = (Input.mousePosition.y < limitRect.y) ? Input.mousePosition.y - limitRect.y : Input.mousePosition.y - limitRect.height;
                }

                Vector3 targetDirection = new Vector3(x, 0, z);

                float y = mainCamera.transform.rotation.eulerAngles.y;

                targetDirection = Quaternion.Euler(0, y, 0) * targetDirection * 0.03f;

                mainCamera.transform.Translate(targetDirection * Time.deltaTime * playerMoveSpeed, Space.World);

                mainCamera.transform.position = new Vector3(Mathf.Clamp(mainCamera.transform.position.x, (x_Range.x + playerOffset.x), (x_Range.y + playerOffset.x)), mainCamera.transform.position.y, Mathf.Clamp(mainCamera.transform.position.z, (z_Range.x + playerOffset.z), (z_Range.y + playerOffset.z)));
                #endregion
            }
        }
    }


    //======================================================



    private float radius = 0.8f;//半径
    private Vector3 endPos;//
    private Vector3 raypointPos;
    private Transform currentHand;//当前控制的手
    private Transform currentUpArm;//圆球圆心（人物肩部）
    private Transform handPivotPoint;//手部中心点
    private void SetPos(LinkAnimatorIK linkAnimatorIK)
    {
        ///  Debug.Log(radius);
        Vector3 offset = handPivotPoint.transform.position - currentHand.transform.position;

        Vector3 tmpPos = ControlObj.transform.position - offset;

        Vector3 mousePos = ControlObj.transform.position;

        Ray ray = new Ray(mainCamera.transform.position, (ControlObj.transform.position - mainCamera.transform.position).normalized);

        float mousePos_center = Vector3.Distance(mousePos, currentUpArm.transform.position);  //鼠标点到圆心的距离(CA)

        float camPos_center = Vector3.Distance(mainCamera.transform.position, currentUpArm.transform.position);  //摄像机到圆心的距离(OC)

        if (mousePos_center <= radius)          //鼠标点在半径内或半径上
        {
            raypointPos = tmpPos;
        }
        else
        {
            Vector3 OA = mousePos - mainCamera.transform.position;      //OA向量
            Vector3 OC = currentUpArm.transform.position - mainCamera.transform.position;    //OC向量
            float OC2 = Vector3.Dot(ray.direction, OC);         //OC'
            float CC2 = Mathf.Sqrt(Mathf.Pow(camPos_center, 2) - Mathf.Pow(OC2, 2));       //CC'
            float C2P2 = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(CC2, 2));          //C'P2
            float OP2 = OC2 + C2P2;        //OP2
            if (camPos_center >= radius)
            {
                if (CC2 > radius)
                {
                    return;
                }
                Debug.Log(22);
                float P1C2 = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(CC2, 2));            //P1C'
                float OP1 = OC2 - P1C2;        //OP1
                float rayPoint = OP1 > OP2 ? OP1 : OP2;
                raypointPos = ray.GetPoint(rayPoint) - offset;
            }
            else
            {
                raypointPos = ray.GetPoint(OP2) - offset;
            }
        }


        endPos = raypointPos;

        //if (Vector3.Dot(linkAnimatorIK.transform.forward, handPivotPoint.transform.position - linkAnimatorIK.transform.position) > 0)
        //{

        //}

        if (hand == HandType.RightHand)
        {
            linkAnimatorIK.GetIKTarget_RightHand().transform.DOMove(endPos, 0.1f);
            linkAnimatorIK.isRightHandIK_pos = true;
            linkAnimatorIK.isRightHandIK_rotate = true;
        }
        else if (hand == HandType.LeftHand)
        {
            linkAnimatorIK.GetIKTarget_LeftHand().transform.DOMove(endPos, 0.1f);
            linkAnimatorIK.isLeftHandIK_pos = true;
            linkAnimatorIK.isLeftHandIK_rotate = true;
        }
    }
    #endregion
    #region 对外接口及属性
    public Transform ControlObj
    {
        get { return controlObj; }
    }

    public List<Transform> HitObj
    {
        get { return hitObj; }
    }
    public RayDetect Rdt
    {
        get { return rdt; }
    }
    #endregion
    //==============================================================================================new
    /// <summary>
    /// 只控制物体
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="controlPlane"></param>
    /// <param name="ctrlObj"></param>
    /// <param name="panelPos"></param>
    /// <param name="panelRotate"></param>
    /// <param name="pivot"></param>
    public PlaneControl(Camera camera, Transform controlPlane, Transform ctrlObj, Transform pivot = null)
    {
        mainCamera = camera;
        this.plane = controlPlane;
        tietu_laser = new Texture[4];
        tietu_laser[0] = Resources.Load("Laser/LaserTexture/LaserTexture") as Texture;
        tietu_laser[1] = Resources.Load("Laser/LaserTexture/LaserTexture2") as Texture;
        tietu_laser[2] = Resources.Load("Laser/LaserTexture/LaserDot") as Texture;
        tietu_laser[3] = Resources.Load("Laser/LaserTexture/LaserDot 1") as Texture;
        rdt = new RayDetect();


        isIncontrol = true;
        this.controlObj = ctrlObj;
        if (pivot != null)
        {
            this.pivot = pivot;
        }
        else
        {
            this.pivot = ctrlObj;
        }
    }
    /// <summary>
    /// 控制物体 向下射线
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="controlPlane"></param>
    public PlaneControl(Camera camera, Transform controlPlane, Transform ctrlObj, Transform pivot, Transform laserTarget, Transform[] hitTarget, UnityAction<Transform> clickEvent)
    {
        mainCamera = camera;
        this.plane = controlPlane;
        tietu_laser = new Texture[4];
        tietu_laser[0] = Resources.Load("Laser/LaserTexture/LaserTexture") as Texture;
        tietu_laser[1] = Resources.Load("Laser/LaserTexture/LaserTexture2") as Texture;
        tietu_laser[2] = Resources.Load("Laser/LaserTexture/LaserDot") as Texture;
        tietu_laser[3] = Resources.Load("Laser/LaserTexture/LaserDot 1") as Texture;
        rdt = new RayDetect();

        isIncontrol = true;
        this.controlObj = ctrlObj;
        this.pivot = pivot;
        //=====================================================================
        hitObj = new List<Transform>();

        for (int i = 0; i < hitTarget.Length; i++)
        {
            hitObj.Add(FindColliderObj(hitTarget[i]));
        }

        isLaser = true;
        this.laserTarget = laserTarget;

        laser = Object.Instantiate(Resources.Load("Laser/Laser")) as GameObject;
        laser.transform.SetParent(ctrlObj.transform);
        laser.transform.position = laserTarget.position;
        laser.transform.eulerAngles = laserTarget.eulerAngles;

        laser.name = laser.name.Replace("(Clone)", "");
        laserDot = laser.transform.GetChild(0).gameObject;
        lRenderer = laser.GetComponent<LineRenderer>();
        lRenderer.SetWidth(0.005f, 0.005f);

        rdt.AddRayEvent(RayEventType.Click, (obj) =>
        {
            clickEvent(obj);

            if (hitObj.Contains(obj))
            {
                hitObj.Remove(obj);
                laser.GetComponent<Renderer>().material.mainTexture = tietu_laser[0];
                laserDot.GetComponent<Renderer>().material.mainTexture = tietu_laser[2];

                Object.Destroy(laser);
                Object.Destroy(laserDot);

            }

        });
        rdt.AddRayEvent(RayEventType.Enter, (obj) =>
        {
            if (hitObj.Contains(obj))
            {
                laser.GetComponent<Renderer>().material.mainTexture = tietu_laser[1];
                laserDot.GetComponent<Renderer>().material.mainTexture = tietu_laser[3];
            }
            else
            {
                laser.GetComponent<Renderer>().material.mainTexture = tietu_laser[0];
                laserDot.GetComponent<Renderer>().material.mainTexture = tietu_laser[2];
            }
        });

    }
    /// <summary>
    /// IK 控制
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="ctrlObj">控制的物体</param>
    /// <param name="controlPlane">物体所在平面</param>
    /// <param name="handType">左右手</param>
    /// <param name="handObj">左右手物体 </param>
    /// <param name="circleCenter">圆心（左右肩部）</param>
    /// <param name="handPivotPoint"> 手部中心点（鼠标跟随的位置）</param>
    /// <param name="body">人物</param>
    /// <param name="radius">控制的半径</param>
    /// <param name="ray_EnterEvent">进入事件</param>
    /// <param name="ray_ExitEvent">推出事件</param>
    /// <param name="ray_ClickEvent">点击事件</param>
    /// <param name="ray_DoubleClickEvent">双击事件</param>
    public PlaneControl(Camera camera, Transform controlPlane, Transform ctrlObj, HandType handType, Transform handPivotPoint, Animator human, RayEventHandle<Transform> ray_EnterEvent = null, RayEventHandle<Transform> ray_ExitEvent = null, RayEventHandle<Transform> ray_ClickEvent = null, RayEventHandle<Transform> ray_DoubleClickEvent = null)
    {
        mainCamera = camera;
        this.controlObj = ctrlObj.transform;
        this.plane = controlPlane;
        this.hand = handType;

        isInIK = true;
        rdt = new RayDetect(ray_EnterEvent, ray_ExitEvent, ray_ClickEvent, ray_DoubleClickEvent);
        isIncontrol = true;
        pivot = ctrlObj.transform;

        linkAnimatorIK = human.GetComponent<LinkAnimatorIK>();

        if (linkAnimatorIK == null)
        {
            linkAnimatorIK = human.gameObject.AddComponent<LinkAnimatorIK>();
        }
        linkAnimatorIK.enabled = true;

        this.handPivotPoint = handPivotPoint;

        if (handType == HandType.RightHand)
        {
            currentHand = human.GetBoneTransform(HumanBodyBones.RightHand);
            linkAnimatorIK.isRightHandIK_pos = true;
            linkAnimatorIK.isRightHandIK_rotate = true;
            linkAnimatorIK.rightHandWeight_pos = 1;
            linkAnimatorIK.rightHandWeight_rotate = 1;
        }
        else if (handType == HandType.LeftHand)
        {
            currentHand = human.GetBoneTransform(HumanBodyBones.LeftHand);
            linkAnimatorIK.isLeftHandIK_pos = true;
            linkAnimatorIK.isLeftHandIK_rotate = true;
            linkAnimatorIK.leftHandWeight_pos = 1;
            linkAnimatorIK.leftHandWeight_rotate = 1;
        }

        currentUpArm = currentHand.parent.parent;

        this.radius = Vector3.Distance(currentHand.position, currentUpArm.position) * 1.2f;
    }
    public PlaneControl(Camera camera, Transform controlPlane, Transform ctrlObj, HandType handType, Transform handPivotPoint, Animator human, float radius, RayEventHandle<Transform> ray_EnterEvent = null, RayEventHandle<Transform> ray_ExitEvent = null, RayEventHandle<Transform> ray_ClickEvent = null, RayEventHandle<Transform> ray_DoubleClickEvent = null)
    {
        mainCamera = camera;
        this.controlObj = ctrlObj.transform;
        this.plane = controlPlane;
        this.hand = handType;

        isInIK = true;
        rdt = new RayDetect(ray_EnterEvent, ray_ExitEvent, ray_ClickEvent, ray_DoubleClickEvent);
        isIncontrol = true;
        pivot = ctrlObj.transform;
        this.radius = radius;
        linkAnimatorIK = human.GetComponent<LinkAnimatorIK>();

        if (linkAnimatorIK == null)
        {
            linkAnimatorIK = human.gameObject.AddComponent<LinkAnimatorIK>();
        }
        linkAnimatorIK.enabled = true;
        this.handPivotPoint = handPivotPoint;


        if (handType == HandType.RightHand)
        {
            currentHand = human.GetBoneTransform(HumanBodyBones.RightHand);
            linkAnimatorIK.isRightHandIK_pos = true;
            linkAnimatorIK.isRightHandIK_rotate = true;
            linkAnimatorIK.rightHandWeight_pos = 1;
            linkAnimatorIK.rightHandWeight_rotate = 1;
        }
        else if (handType == HandType.LeftHand)
        {
            currentHand = human.GetBoneTransform(HumanBodyBones.LeftHand);
            linkAnimatorIK.isLeftHandIK_pos = true;
            linkAnimatorIK.isLeftHandIK_rotate = true;
            linkAnimatorIK.leftHandWeight_pos = 1;
            linkAnimatorIK.leftHandWeight_rotate = 1;
        }

        currentUpArm = currentHand.parent.parent;


    }

    //======================================================================================================
    /// <summary>
    /// update 中调用
    /// </summary>
    public void Update()
    {
        if (isIncontrol && controlObj)
        {
            Vector3 planeVector = plane.transform.up;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            Vector3 offset = pivot.position - controlObj.position;

            Vector3 finalPos = Mathf_Frame.CalPlaneLineIntersectPoint(planeVector, plane.position, ray.direction, ray.origin) - offset;

            if (isLimitPos)
            {
                #region  限制位置
                if (x_Range != Vector2.zero)
                {
                    finalPos = new Vector3(Mathf.Clamp(finalPos.x, x_Range.x, x_Range.y), finalPos.y, finalPos.z);
                }
                if (z_Range != Vector2.zero)
                {
                    finalPos = new Vector3(finalPos.x, finalPos.y, Mathf.Clamp(finalPos.z, z_Range.x, z_Range.y));
                }
                if (y_Range != Vector2.zero)
                {
                    finalPos = new Vector3(finalPos.x, Mathf.Clamp(finalPos.y, y_Range.x, y_Range.y), finalPos.z);
                }
                #endregion
            }
            FollowObj(isFollowing);

            if (isInIK)
            {
                controlObj.transform.position = finalPos;

                SetPos(linkAnimatorIK);
            }
            else
            {
                Tweener t = null;
                t = controlObj.transform.DOMove(finalPos, 0.3f).OnComplete(() =>
                {
                    tweenerList.Remove(t);
                });
                tweenerList.Add(t);
            }
            if (isLaser)
            {
                if (laser == null)
                {
                    laser = Object.Instantiate(Resources.Load("Laser/Laser")) as GameObject;
                    laser.transform.SetParent(controlObj.transform);

                    laser.transform.position = laserTarget.position;
                    laser.transform.eulerAngles = laserTarget.eulerAngles;

                    laser.name = laser.name.Replace("(Clone)", "");
                    laserDot = laser.transform.GetChild(0).gameObject;
                    lRenderer = laser.GetComponent<LineRenderer>();
                    lRenderer.SetWidth(0.005f, 0.005f);
                }



                #region 线渲染器Effect
                laser.GetComponent<Renderer>().material.mainTextureOffset += new Vector2(Time.deltaTime * 1.0f * 0.5f, 0);
                laser.GetComponent<Renderer>().material.SetTextureOffset("_NoiseTex", new Vector2(-Time.time * 1.0f * 0.5f, 0.0f));
                float aniFactor = Mathf.PingPong(Time.time * 1.5f, 1.0f);
                aniFactor = Mathf.Max(0.5f, aniFactor) * 0.5f;
                lRenderer.SetWidth(aniFactor * 0.4f * 0.1f, aniFactor * 0.4f * 0.1f);
                #endregion
                Ray ray_l = new Ray(laser.transform.position, Vector3.down);

                RaycastHit ctrlObjHitInfo = new RaycastHit();

                ctrlObjHitInfo = rdt.GetHitInfo();

                rdt.Update(ray_l, Mathf.Infinity, ~(1 << -1), true);

                if (rdt.GetCurrentHitObj())
                {
                    #region 线渲染器功能
                    laser.SetActive(true);
                    lRenderer.SetPosition(0, laser.transform.position);
                    lRenderer.SetPosition(1, ctrlObjHitInfo.point);
                    if (laserDot)
                    {

                        laserDot.transform.position = ctrlObjHitInfo.point;
                        laserDot.transform.rotation = Quaternion.LookRotation(ctrlObjHitInfo.normal, laserDot.transform.up);//??
                        //   laserDot.transform.rotation = Quaternion.FromToRotation(ctrlObjHitInfo.normal, Vector3.up);

                        laserDot.transform.eulerAngles = new Vector3(0, laserDot.transform.eulerAngles.y, laserDot.transform.eulerAngles.z);

                    }
                    #endregion

                }
                //==========================================================================================
            }
            else
            {
                rdt.Update(ray, Mathf.Infinity, ~(1 << -1), true);
            }

        }
    }
    /// <summary>
    /// 设置摄像机 跟随物体移动 无wow脚本时使用
    /// </summary>
    public void SetFollowing(Camera camera, float speed = 0.2f)
    {
        playerMoveSpeed = speed;
        this.mainCamera = camera;
        this.isFollowing = true;
        cameraFollowType = CameraMoveType.camera;
    }

    /// <summary>
    ///  设置摄像机 跟随物体移动 wow
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="player"></param>
    public void SetWowFollowing(WowMainCamera wow, Transform player, float speed = 0.2f)
    {
        playerMoveSpeed = speed;
        this.mainCamera = wow.transform.GetComponent<Camera>();

        this.isFollowing = true;
        this.player = player;
        cameraFollowType = CameraMoveType.wowcamera;
        this.wow = wow;

    }

    /// <summary>
    /// 设置位置限制 设置操控物体时控制范围
    /// </summary>
    /// <param name="x_range"></param>
    /// <param name="z_range"></param>
    public void SetPosLimit(Vector2 x_range, Vector2 y_Range, Vector2 z_range, Vector3 playerOffset)
    {
        this.x_Range = x_range;
        this.z_Range = z_range;
        this.y_Range = y_Range;
        isLimitPos = true;
        this.playerOffset = playerOffset;
    }

    /// <summary>
    ///解除控制
    /// </summary>
    public void OutControl()
    {
        isIncontrol = false;
        controlObj = null;
        if (isLaser)
        {
            isLaser = false;
            Object.Destroy(laser.gameObject);
            hitObj.Clear();
        }
        ResetTweenList();
        rdt.DesRay();//
        isFollowing = false;
        isLimitPos = false;
        if (isInIK)
        {
            isInIK = false;
        }
    }


}
