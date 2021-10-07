using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 多点互动连线组件，用于手术类项目的操作互动
/// </summary>
public class UIEventNodeMutiLines : MonoBehaviour
{
    /// <summary>
    /// 是否自动初始化
    /// </summary>
    public bool initOnAwake = false;
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool interactalbe = true;
    /// <summary>
    /// 是否在控制台Debug进度
    /// </summary>
    public bool debugProgress = true;
    /// <summary>
    /// node节点数组
    /// </summary>
    public UIEventNode[] uiEventNodes;
    /// <summary>
    /// 总进度
    /// </summary>
    [SerializeField]
    private float allProgress = 0;
    /// <summary>
    /// 是否开始画线
    /// </summary>
    [SerializeField]
    private bool isStartDrawLines = false;            // 是否开始画线
    /// <summary>
    /// 是否完毕
    /// </summary>
    [SerializeField]
    private bool isDrawCompleteAll = false;             // 是否结束画线

    [SerializeField]
    private UIEventNodeLine[] uiNodeLines;

    /// <summary>
    /// 总距离
    /// </summary>
    private float totalDistance = 0;           //总距离
    /// <summary>
    /// progress事件
    /// </summary>
    private UnityAction<float> progressEvent;

    [SerializeField]
    private float[] progressGroup;

    private Camera myCamera;
    #region 对外接口
    public void Init(UIEventNode[] nodes, UnityAction<float> progressEvent)
    {
        Debug.Log(1);
        if (nodes.Length <= 2)
        {
            Debug.Log("节点数量小于二，不需要使用此组件");
            return;
        }
        if (this.uiEventNodes == null)
        {
            this.uiEventNodes = nodes;
        }
        this.progressEvent += progressEvent;

        uiNodeLines = new UIEventNodeLine[nodes.Length - 1];

        progressGroup = new float[uiNodeLines.Length];

        for (int i = 0; i < uiNodeLines.Length; i++)
        {
            int index = i;

            uiNodeLines[i] = CreatUINodeLines(nodes[i], nodes[i + 1], (value) =>
            {
                progressGroup[index] = value;
            });

            if (index != 0)
            {
                uiNodeLines[i].Interactalbe = false;
            }

            totalDistance += uiNodeLines[i].NodesDistance;
        }
    }

    public void AddProgressEvent(UnityAction<float> progressEvent)
    {

        this.progressEvent += progressEvent;


    }

    public void Destroy()
    {
        Debug.Log(uiEventNodes);
        for (int i = 0; i < uiEventNodes.Length; i++)
        {
            uiEventNodes[i].Destroy();
        }
        Destroy(gameObject);
    }
    #endregion
    void Start()
    {
        if (initOnAwake)
            Init(uiEventNodes, (progress) =>
            {
                if (debugProgress)
                    Debug.Log(progress);
            });
    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="isCorrect">是否正确</param>
    private void OnDrawDone(bool isCorrect)
    {
        isStartDrawLines = false;

        if (isCorrect)
        {
            allProgress = 1;
            OnDrawUpdate(allProgress);
            isDrawCompleteAll = true;
        }
        else
        {
            allProgress = 0;
            OnDrawUpdate(allProgress);
            for (int i = 0; i < uiNodeLines.Length; i++)
            {
                uiNodeLines[i].ResetLine();
                progressGroup[i] = 0;
                if (i != 0)
                {
                    uiNodeLines[i].Interactalbe = false;
                }
            }
        }
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    private void OnDrawStart()
    {
        isStartDrawLines = true;
    }

    /// <summary>
    /// 拖拽中
    /// </summary>
    /// <param name="progress"></param>
    private void OnDrawUpdate(float progress)
    {
        if (progressEvent != null)
            progressEvent(progress);
    }

    void Update()
    {
        if (isDrawCompleteAll || !interactalbe)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            OnDrawStart();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnDrawDone(false);
        }

        if (isStartDrawLines)
        {
            float total = 0;

            int index = 0;

            for (int i = 0; i < progressGroup.Length; i++)
            {
                total += progressGroup[i] * uiNodeLines[i].NodesDistance / totalDistance;

                if (progressGroup[i] == 1 && i < progressGroup.Length - 1 && !uiNodeLines[i + 1].Interactalbe)
                {
                    uiNodeLines[i + 1].Interactalbe = true;

                    uiNodeLines[i + 1].currentMouseOverNodesList.Add(uiNodeLines[i].endNode.gameObject);

                    return;
                }

                if (progressGroup[i] == 1)
                {
                    index++;
                }
            }


            if (index > 0 && index < progressGroup.Length && !uiNodeLines[index].IsStartDrawLine)
            {
                OnDrawDone(false);
                return;
            }

            if (progressGroup[progressGroup.Length - 1] == 1)
            {
                OnDrawDone(true);
            }
            else
            {
                allProgress = total;
                OnDrawUpdate(allProgress);
            }
        }

    }

    /// <summary>
    /// 生成子组件
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <param name="progressEvent"></param>
    /// <returns></returns>
    private UIEventNodeLine CreatUINodeLines(UIEventNode startNode, UIEventNode endNode, UnityAction<float> progressEvent)
    {
        UIEventNodeLine nodeLine = gameObject.AddComponent<UIEventNodeLine>();
        nodeLine.Init(startNode, endNode, progressEvent);
        return nodeLine;
    }


}
