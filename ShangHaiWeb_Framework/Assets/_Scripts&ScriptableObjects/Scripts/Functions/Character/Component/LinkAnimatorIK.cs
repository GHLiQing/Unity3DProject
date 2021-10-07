using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum HandType
{
    LeftHand,
    RightHand
}
[RequireComponent(typeof(Animator))]
public class LinkAnimatorIK : MonoBehaviour
{

    public bool drawControlBall;//是否在scene视图中绘制出IK跟随的目标物体

    #region 内置变量、方法
    private Animator ani;

    private GameObject ik_prefab;

    private GameObject _IKTarget_RightHand;//右手Ik目标

    private GameObject _IKTarget_LeftHand;//左手Ik目标

    private GameObject _IKTarget_Head;//头部Ik目标

    private GameObject _IKTarget_RightLeg;//右腿Ik目标

    private GameObject _IKTarget_LeftLeg;//左腿Ik目标

    private void CreateIK()
    {
        if (isRightHandIK_pos && !_IKTarget_RightHand)
        {
            _IKTarget_RightHand = new GameObject("IKTarget_RightHand");
            _IKTarget_RightHand.transform.SetParent(ik_prefab.transform);
        }
        else if (!isRightHandIK_pos && _IKTarget_RightHand)
        {
            Destroy(_IKTarget_RightHand);
        }

        if (isLeftHandIK_pos && !_IKTarget_LeftHand)
        {
            _IKTarget_LeftHand = new GameObject("IKTarget_LeftHand");
            _IKTarget_LeftHand.transform.SetParent(ik_prefab.transform);
        }
        else if (!isLeftHandIK_pos && _IKTarget_LeftHand)
        {
            Destroy(_IKTarget_LeftHand);
        }

        if (isRightLegIK_pos && !_IKTarget_RightLeg)
        {
            _IKTarget_RightLeg = new GameObject("IKTarget_RightLeg");
            _IKTarget_RightLeg.transform.SetParent(ik_prefab.transform);
        }
        else if (!isRightLegIK_pos && _IKTarget_RightLeg)
        {
            Destroy(_IKTarget_RightLeg);
        }

        if (isLeftLegIK_pos && !_IKTarget_LeftLeg)
        {
            _IKTarget_LeftLeg = new GameObject("IKTarget_LeftLeg");
            _IKTarget_LeftLeg.transform.SetParent(ik_prefab.transform);
        }
        else if (!isLeftLegIK_pos && _IKTarget_LeftLeg)
        {
            Destroy(_IKTarget_LeftLeg);
        }
        if (isHeadIK_pos && !_IKTarget_Head)
        {
            _IKTarget_Head = new GameObject("IKTarget_Head");
            _IKTarget_Head.transform.SetParent(ik_prefab.transform);
        }
    }
    #endregion

    #region 对外变量 接口 
    #region 各个部位IK开关和权重值
    //右手
    public bool isRightHandIK_pos = false;//右手位置
    public bool isRightHandIK_rotate = false;//右手旋转
    public float rightHandWeight_pos;//右手位置比重
    public float rightHandWeight_rotate;//右手旋转比重

    //左手
    public bool isLeftHandIK_pos = false;//左手位置
    public bool isLeftHandIK_rotate = false;//左手旋转
    public float leftHandWeight_pos;//左手位置比重
    public float leftHandWeight_rotate;//左手旋转比重

    //头部
    public bool isHeadIK_pos = false;//头部位置
    public float headWeight_pos;//头部位置比重

    //右腿
    public bool isRightLegIK_pos = false;//右腿位置
    public bool isRightLegIK_rotate = false;//右腿旋转
    public float rightLegWeight_pos;//右腿位置比重
    public float rightLegWeight_rotate;//右腿旋转比重

    //左腿
    public bool isLeftLegIK_pos = false;//左腿位置
    public bool isLeftLegIK_rotate = false;//左腿旋转
    public float leftLegWeight_pos;//左腿位置比重
    public float leftLegWeight_rotate;//左腿旋转比重
    #endregion
    #region 对外接口
    public GameObject GetIKTarget_RightHand()
    {
        return _IKTarget_RightHand;
    }
    public GameObject GetIKTarget_LeftHand()
    {
        return _IKTarget_LeftHand;
    }
    public GameObject GetIKTarget_Head()
    {
        return _IKTarget_Head;
    }
    #endregion
    #endregion

    void Start()
    {
        ik_prefab = GameObject.Find("IK_Prefab");

        if (ik_prefab == null)
        {
            ik_prefab = new GameObject("IK_Prefab");
        }

        ani = GetComponent<Animator>();

        CreateIK();
    }

    void OnAnimatorIK(int layerIndex)
    {
        CreateIK();

        #region  右手
        if (isRightHandIK_pos)
        {
            ani.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight_pos);
            ani.SetIKPosition(AvatarIKGoal.RightHand, _IKTarget_RightHand.transform.position);
        }
        if (isRightHandIK_rotate && _IKTarget_RightHand)
        {

            ani.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight_rotate);
            ani.SetIKRotation(AvatarIKGoal.RightHand, _IKTarget_RightHand.transform.rotation);
        }
        #endregion
        #region 左手
        if (isLeftHandIK_pos)
        {

            ani.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight_pos);
            ani.SetIKPosition(AvatarIKGoal.LeftHand, _IKTarget_LeftHand.transform.position);
        }
        if (isLeftHandIK_rotate && _IKTarget_LeftHand)
        {
            ani.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight_rotate);
            ani.SetIKRotation(AvatarIKGoal.LeftHand, _IKTarget_LeftHand.transform.rotation);
        }
        #endregion
        #region 头部
        if (isHeadIK_pos)
        {
            ani.SetLookAtWeight(headWeight_pos);
            ani.SetLookAtPosition(_IKTarget_Head.transform.position);
        }


        #endregion
        #region 左腿
        if (isLeftLegIK_pos)
        {
            ani.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftLegWeight_pos);
            ani.SetIKPosition(AvatarIKGoal.LeftFoot, _IKTarget_LeftLeg.transform.position);
        }
        if (isLeftLegIK_rotate && _IKTarget_LeftLeg)
        {
            ani.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftLegWeight_rotate);
            ani.SetIKRotation(AvatarIKGoal.LeftFoot, _IKTarget_LeftLeg.transform.rotation);
        }
        #endregion
        #region 右腿
        if (isRightLegIK_pos)
        {
            ani.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightLegWeight_pos);
            ani.SetIKPosition(AvatarIKGoal.RightFoot, _IKTarget_RightLeg.transform.position);
        }
        if (isRightLegIK_rotate && _IKTarget_RightLeg)
        {
            ani.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightLegWeight_rotate);
            ani.SetIKRotation(AvatarIKGoal.RightFoot, _IKTarget_RightLeg.transform.rotation);
        }
        #endregion

    }

    void OnDrawGizmos()
    {
        if (!drawControlBall)
            return;
        if (isRightHandIK_pos && _IKTarget_RightHand)
        {
            Gizmos.color = Color.blue;//为随后绘制的gizmos设置颜色。
            Gizmos.DrawWireSphere(_IKTarget_RightHand.transform.position, .25f / 4);//使用center和radius参数，绘制一个线框球体。
        }
        if (isLeftHandIK_pos && _IKTarget_LeftHand)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_IKTarget_LeftHand.transform.position, .25f / 4);
        }

        if (isLeftLegIK_pos && _IKTarget_LeftLeg)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_IKTarget_LeftLeg.transform.position, .25f / 4);
        }

        if (isRightLegIK_pos && _IKTarget_RightLeg)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_IKTarget_RightLeg.transform.position, .25f / 4);
        }

    }

    void Update()
    {
      
    }



}
