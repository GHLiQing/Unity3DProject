using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.PostProcessing;
using UnityEngine.UI;

public static class MyUtilities
{
    #region Animation
    private static Dictionary<Animation, bool> bool_Animation = new Dictionary<Animation, bool>();
    public static void DetectionAnimation(this Animation ani, string animationName, UnityAction OnStart, UnityAction OnUpdate, UnityAction OnEnd)
    {
        if (!bool_Animation.ContainsKey(ani))
        {
            bool_Animation.Add(ani, false);
        }
        if (!bool_Animation[ani])
        {
            if (ani.IsPlaying(animationName))
            {
                bool_Animation[ani] = true;
                if(OnStart!=null)
                OnStart();
            }
        }
        else
        {
            if (ani.IsPlaying(animationName))
            {
                if(OnUpdate!=null)
                OnUpdate();
            }
            else
            {
                if(OnEnd!=null)
                OnEnd();
                bool_Animation.Remove(ani);
            }
        }
    }

    public static IEnumerator IEDetectionAnimation(this Animation ani, string animationName, UnityAction OnStart, UnityAction OnUpdate, UnityAction OnEnd)
    {
        while (!ani.IsPlaying(animationName))
        {
            yield return null;
        }
        OnStart();
        while (ani.IsPlaying(animationName))
        {
            OnUpdate();
            yield return null;
        }
        OnEnd();
    }
    #endregion
    #region Animator
    private static Dictionary<Animator, bool> bool_Animator = new Dictionary<Animator, bool>();
    public static void DetectionAnimator(this Animator ani, int layerIndex, string stateName, UnityAction OnStart, UnityAction OnUpdate, UnityAction OnEnd)
    {
        if (!bool_Animator.ContainsKey(ani))
        {
            bool_Animator.Add(ani, false);
        }

        if (!bool_Animator[ani])
        {
            if (ani.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName))
            {
                bool_Animator[ani] = true;
                OnStart();
            }
        }
        else
        {
            if (ani.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName))
            {
                OnUpdate();
            }
            else
            {
                bool_Animator[ani] = false;
                OnEnd();
            }
        }
    }
    public static IEnumerator IEDetectionAnimator(this Animator ani, int layerIndex, string stateName, UnityAction OnStart, UnityAction OnUpdate, UnityAction OnEnd)
    {
        while (!ani.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName))
        {
            yield return null;
        }
        OnStart();
        while (ani.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName))
        {
            OnUpdate();
            yield return null;
        }
        OnEnd();
    }
    public static void DetectionAnimator_nor(this Animator ani, int layerIndex, string stateName, UnityAction OnStart, UnityAction OnUpdate, UnityAction OnEnd)
    {
        if (!bool_Animator.ContainsKey(ani))
        {
            bool_Animator.Add(ani, false);
        }
        if (!bool_Animator[ani])
        {
            if (ani.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName))
            {
                bool_Animator[ani] = true;
                OnStart();
            }
        }
        else
        {
            if (ani.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime<0.95f)
            {
                OnUpdate();
            }
            else
            {
                bool_Animator[ani] = false;
                OnEnd();
            }
        }
    }
    #endregion
    #region GameObject/Transform
    //  public static Sequence mySequence;
    // public static bool isMoving = false;
    public static void DoMoveAndRotation(this GameObject target, Vector3 position, Vector3 rotation, float duration, Ease ease, UnityAction OnUpdate, UnityAction OnEnd, Space spaceWorld)
    {
        if (target.GetComponent<WowMainCamera>())
        {
            if (target.GetComponent<WowMainCamera>().enabled)
            {
                // print("存在WowMainCamera.cs脚本");
                Debug.LogError("存在WowMainCamera.cs脚本");
                return;
            }
        }
        //  isMoving = true;
        //  mySequence.Kill();
        Sequence mySequence = DOTween.Sequence();
        Tweener t1;
        Tweener t2;

        switch (spaceWorld)
        {
            case Space.Self:
                t1 = target.transform.DOLocalMove(position, duration).SetEase(ease);
                t2 = target.transform.DOLocalRotate(rotation, duration).SetEase(ease);
                break;
            default:
                t1 = target.transform.DOMove(position, duration).SetEase(ease);
                t2 = target.transform.DORotate(rotation, duration).SetEase(ease);
                break;
        }

        mySequence = DOTween.Sequence();
        mySequence.Append(t1)
            .Insert(0, t2)
             .AppendCallback(() =>
             {
                 //    isMoving = false;
                 OnEnd();
             }).OnUpdate(() =>
             {
                 OnUpdate();
             });
    }

    public static Sequence SharkSequence;

    public static void Shake(this GameObject target, float time, Vector3 strengthV3, bool isCamera, UnityAction OnUpdate) 
    {
        SharkSequence.Kill();
        Tweener t1;
        if(isCamera)
            t1 = target.transform.DOShakeRotation(time, strengthV3, 0, 0);
        else
            t1 = target.transform.DOShakePosition(time, strengthV3, 0, 0);
        SharkSequence = DOTween.Sequence();
        SharkSequence.Append(t1);
    }
    public static void Swing(this GameObject target, float time, Vector3 strengthV3, UnityAction OnUpdate) 
    {
        if (SharkSequence == null || !SharkSequence.IsActive())
        {
            SharkSequence.Kill();
            Tweener t1 = target.transform.DOShakeRotation(time, strengthV3, 0, 0);
            SharkSequence = DOTween.Sequence();
            SharkSequence.Append(t1);
        }
    }
    public static void WowSetControl(this GameObject target, bool isControl, bool isDrag, bool isZoom) //改变WowMainCamera的CanBeControl和canBeZoom变量
    {
        WowMainCamera wow = target.GetComponent<WowMainCamera>();
        if (wow == null)
        {
            Debug.LogError("wow为null");
            return;
        }
        wow.allowDrag = isDrag;
        wow.allowControl = isControl;
        wow.allowZoom = isZoom;
    }
    public static void WowDoMove_X_Y_Distance(this GameObject target, Vector3 position, float x, float y, float distance, float duration, Ease ease, UnityAction OnUpdate, UnityAction OnEnd, Space spaceWorld)
    {
        //  isMoving = true;
        //  mySequence.Kill();
        WowMainCamera wow = target.GetComponent<WowMainCamera>();
        if (wow == null)
        {
            Debug.LogError("wow为null");
            return;
        }

        Tweener t1, t2, t3, t4;

        wow.x %= 360;
        wow.y %= 360;
        float deviation = Mathf.Abs(wow.x - x);
        if (deviation > 180)
        {
            if (x > 0)
            {
                if (wow.x > 180)
                {
                    x += 360;
                }
                else
                {
                    x -= 360;
                }
            }
            else
            {
                if (wow.x < -180)
                {
                    wow.x += 360;
                }
                else
                {
                    x += 360;
                }

            }
        }

        switch (spaceWorld)
        {
            case Space.Self:
                t1 = wow.target.transform.DOLocalMove(position, duration).SetEase(ease);
                break;
            default:
                t1 = wow.target.transform.DOMove(position, duration).SetEase(ease);
                break;
        }

        t2 = DOTween.To(() => wow.x, a => wow.x = a, x, duration).SetEase(ease);
        t4 = DOTween.To(() => wow.y, b => wow.y = b, y, duration).SetEase(ease);
        t3 = DOTween.To(() => wow.desiredDistance, c => wow.desiredDistance = c, distance, duration).SetEase(Ease.Linear);
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(t1)
            .Insert(0, t2)
            .Insert(0, t3)
            .Insert(0, t4)
            .AppendCallback(() =>
            {
                //    isMoving = false;
                OnEnd();
            }).OnUpdate(() =>
            {
                OnUpdate();
            });
    }

    public static void WowDoMove(this GameObject target, Vector3 position, float duration, Ease ease, UnityAction OnUpdate, UnityAction OnEnd, Space spaceWorld)
    {
        //  isMoving = true;
        //  mySequence.Kill();
        WowMainCamera wow = target.GetComponent<WowMainCamera>();
        if (wow == null)
        {
            Debug.LogError("wow为null");
            return;
        }

        Tweener t1;
        switch (spaceWorld)
        {
            case Space.Self:
                t1 = wow.target.transform.DOLocalMove(position, duration).SetEase(ease);
                break;
            default:
                t1 = wow.target.transform.DOMove(position, duration).SetEase(ease);
                break;
        }


        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(t1)

            .AppendCallback(() =>
            {
                //    isMoving = false;
                OnEnd();
            }).OnUpdate(() =>
            {
                OnUpdate();
            });
    }
    public static void WowDoXY(this GameObject target, float x, float y, float duration, Ease ease, UnityAction OnUpdate, UnityAction OnEnd)
    {
        //  isMoving = true;
        //  mySequence.Kill();
        WowMainCamera wow = target.GetComponent<WowMainCamera>();
        if (wow == null)
        {
            Debug.LogError("wow为null");
            return;
        }


        wow.x %= 360;
        wow.y %= 360;
        float deviation = Mathf.Abs(wow.x - x);
        if (deviation > 180)
        {
            if (x > 0)
            {
                if (wow.x > 180)
                {
                    x += 360;
                }
                else
                {
                    x -= 360;
                }
            }
            else
            {
                if (wow.x < -180)
                {
                    wow.x += 360;
                }
                else
                {
                    x += 360;
                }

            }
        }
        Tweener t2 = DOTween.To(() => wow.x, a => wow.x = a, x, duration).SetEase(ease);
        Tweener t4 = DOTween.To(() => wow.y, b => wow.y = b, y, duration).SetEase(ease);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(t2)


            .Insert(0, t4)
            .AppendCallback(() =>
            {
                //    isMoving = false;
                OnEnd();
            }).OnUpdate(() =>
            {
                OnUpdate();
            });
    }

    public static void WowDoMove_X_Y(this GameObject target, Vector3 position, float x, float y, float duration, Ease ease, UnityAction OnUpdate, UnityAction OnEnd, Space worldSpace)
    {
        //  isMoving = true;
        //  mySequence.Kill();
        WowMainCamera wow = target.GetComponent<WowMainCamera>();
        if (wow == null)
        {
            Debug.LogError("wow为null");
            return;
        }

        Tweener t1;
        switch (worldSpace)
        {
            case Space.Self:
                t1 = wow.target.transform.DOLocalMove(position, duration).SetEase(ease);
                break;
            default:
                t1 = wow.target.transform.DOMove(position, duration).SetEase(ease);
                break;
        }


        wow.x %= 360;
        wow.y %= 360;
        float deviation = Mathf.Abs(wow.x - x);
        if (deviation > 180)
        {
            if (x > 0)
            {
                if (wow.x > 180)
                {
                    x += 360;
                }
                else
                {
                    x -= 360;
                }
            }
            else
            {
                if (wow.x < -180)
                {
                    wow.x += 360;
                }
                else
                {
                    x += 360;
                }

            }
        }
        Tweener t2 = DOTween.To(() => wow.x, a => wow.x = a, x, duration).SetEase(ease);
        Tweener t4 = DOTween.To(() => wow.y, b => wow.y = b, y, duration).SetEase(ease);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(t1)
            .Insert(0, t2)

            .Insert(0, t4)
            .AppendCallback(() =>
            {
                //    isMoving = false;
                OnEnd();
            }).OnUpdate(() =>
            {
                OnUpdate();
            });
    }

    public static void WowDoDistance(this GameObject target, float distance, float duration, Ease ease, UnityAction OnUpdate, UnityAction OnEnd)
    {
        //  isMoving = true;
        //  mySequence.Kill();
        WowMainCamera wow = target.GetComponent<WowMainCamera>();
        if (wow == null)
        {
            Debug.LogError("wow为null");
            return;
        }




        Tweener t3 = DOTween.To(() => wow.desiredDistance, c => wow.desiredDistance = c, distance, duration).SetEase(Ease.Linear);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(t3)

            .AppendCallback(() =>
            {
                //    isMoving = false;
                OnEnd();
            }).OnUpdate(() =>
            {
                OnUpdate();
            });
    }
    #endregion
    #region Graphic
    #region  Text
    //   public static Sequence sequence_text;
    public static Dictionary<Text, Sequence> text_Sequence = new Dictionary<Text, Sequence>();
    public static void FadeShowText(this Text text, float showtime, string text_str, Color text_color)
    {

        Sequence sequence_text = null;

        Debug.Log(text_Sequence.ContainsKey(text) + " " + text_Sequence.Count + " " + text.name);



        if (text_Sequence.ContainsKey(text))
        {

            sequence_text = text_Sequence[text];

            sequence_text.Kill(true);
            sequence_text = DOTween.Sequence();

            text_Sequence.Add(text, sequence_text);

        }
        else
        {
            sequence_text = DOTween.Sequence();
            text_Sequence.Add(text, sequence_text);

        }

        text.text = text_str;

        text.color = new Color(text_color.r, text_color.g, text_color.b, 0);

        Tweener t1 = text.DOFade(1, 1);
        Tweener t2 = text.DOFade(0, 1);
        
        sequence_text.Append(t1)
              .AppendInterval(showtime)
            .Append(t2)
            .OnComplete(() =>
            {
                text_Sequence.Remove(text);
                Debug.Log("remove");
            });
    }
    #endregion
    public static Dictionary<Graphic, Sequence> graphic_Sequence = new Dictionary<Graphic, Sequence>();
    public static void FadeInOut(this Graphic graphic, float time_stay, UnityAction OnFadeIn, UnityAction OnEnd)
    {
        Tweener t1 = graphic.DOFade(1, 0.8f).OnComplete(() =>
        {
            OnFadeIn();
        });
        Tweener t2 = graphic.DOFade(0, 0.8f).SetDelay(time_stay).OnComplete(() =>
        {
            graphic_Sequence.Remove(graphic);
            OnEnd();
        });
        Sequence sequence = null;
        if (graphic_Sequence.ContainsKey(graphic))
        {
            sequence = graphic_Sequence[graphic];
            sequence.Kill(true);
        }
        //else
        //{
            sequence = DOTween.Sequence();
            graphic_Sequence.Add(graphic, sequence);
        //}
        sequence.Append(t1)
                .AppendInterval(time_stay)
          .Append(t2)
        .AppendCallback(() =>
        {
            graphic_Sequence.Remove(graphic);
            Debug.Log("remove");
        });
    }
    #endregion
    #region  Camera
    public static void DoCameraMoveRotationAndView(this Camera ca, Vector3 position, Vector3 rotation, float view, float duration, Ease ease, UnityAction OnUpdate, UnityAction OnEnd)
    {
        if (ca.GetComponent<WowMainCamera>())
        {
            if (ca.GetComponent<WowMainCamera>().enabled)
            {
                // print("存在WowMainCamera.cs脚本");
                Debug.LogError("存在WowMainCamera.cs脚本");
                return;
            }
        }
        //  isMoving = true;
        //  mySequence.Kill();
        Sequence mySequence = DOTween.Sequence();

        Tweener t1 = ca.transform.DOMove(position, duration).SetEase(ease);
        Tweener t2 = ca.transform.DORotate(rotation, duration).SetEase(ease);
        Tweener t3 = ca.DOFieldOfView(view, duration).SetEase(ease);
        mySequence = DOTween.Sequence();
        mySequence.Append(t1)
            .Insert(0, t2)
            .Insert(0, t3)
             .AppendCallback(() =>
             {
                 //isMoving = false;
                 OnEnd();
             }).OnUpdate(() =>
             {
                 OnUpdate();
             });
    }
    #endregion
    #region MonoBehaviour
    public static void DelayToDo(float time, UnityAction method)
    {
        float a = 0;
        DOTween.To(() => { return a; }, (v) => { a = v; }, 10, time).OnComplete(() =>
        {
            method();
        });
    }
    #endregion


}

public static class Mathf_Frame
{
    /// <summary>
    /// 计算直线和面的交点
    /// </summary>
    /// <param name="planeVector">平面的法向量</param>
    /// <param name="planePoint">平面上一点</param>
    /// <param name="lineVector">直线的方向向量</param>
    /// <param name="linePoint">直线上一点</param>
    /// <returns></returns>
    public static Vector3 CalPlaneLineIntersectPoint(Vector3 planeVector, Vector3 planePoint, Vector3 lineVector, Vector3 linePoint)
    {

        Vector3 returnResult = Vector3.zero;
        float vp1, vp2, vp3, n1, n2, n3, v1, v2, v3, m1, m2, m3, t, vpt;
        vp1 = planeVector.x;
        vp2 = planeVector.y;
        vp3 = planeVector.z;
        n1 = planePoint.x;
        n2 = planePoint.y;
        n3 = planePoint.z;
        v1 = lineVector.x;
        v2 = lineVector.y;
        v3 = lineVector.z;
        m1 = linePoint.x;
        m2 = linePoint.y;
        m3 = linePoint.z;
        vpt = v1 * vp1 + v2 * vp2 + v3 * vp3;
        //首先判断直线是否与平面平行  
        if (vpt == 0)
        {
            //  returnResult = null;       
            Debug.Log("无交点");
        }
        else
        {
            t = ((n1 - m1) * vp1 + (n2 - m2) * vp2 + (n3 - m3) * vp3) / vpt;
            returnResult.x = m1 + v1 * t;
            returnResult.y = m2 + v2 * t;
            returnResult.z = m3 + v3 * t;
        }
        return returnResult;
    }

}



