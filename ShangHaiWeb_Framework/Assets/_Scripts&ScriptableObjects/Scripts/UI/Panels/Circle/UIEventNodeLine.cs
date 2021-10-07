using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIEventNodeLine : MonoBehaviour
{


    public bool initOnAwake = false;

    [SerializeField]
    private bool interactalbe = true;

    public bool debugProgress = true;
    public bool Interactalbe
    {
        get { return interactalbe; }
        set
        {
            if (value == true)
            {
                OnDrawStart();
            }
            interactalbe = value;
        }
    }

    public UIEventNode startNode;             // 起始圈

    public UIEventNode endNode;               // 结束圈

    [SerializeField]
    private float progress = 0;                      // 画线进度

    public bool isInDrawing;

    [SerializeField]
    private bool isStartDrawLine = false;            // 是否开始画线
    public bool IsStartDrawLine
    {
        get { return isStartDrawLine; }
    }
    [SerializeField]
    private bool isDrawComplete = false;             // 是否结束画线
    public bool IsDrawComplete
    {
        get { return isDrawComplete; }
    }
    public List<GameObject> currentMouseOverNodesList;

    public float NodesDistance
    {
        get
        {
            float nodesDistance = Getdistance();

            return nodesDistance;
        }
    }
    private float Getdistance()
    {

        Vector3 startNodeScreenPosition = myCamera.WorldToScreenPoint(startNode.transform.position);

        Vector3 endNodeScreenPosition = myCamera.WorldToScreenPoint(endNode.transform.position);

        return Vector2.Distance(endNodeScreenPosition, startNodeScreenPosition);

    }

    private UnityAction<float> progressEvent;

    private Camera myCamera;

    void Start()
    {
        if (initOnAwake)
        {
            if (debugProgress)
            {
                AddProgressEvent((progress) => { Debug.Log(progress); });
            }
            Init();
        }
    }

    public void ResetLine()
    {
        isStartDrawLine = false;
        isDrawComplete = false;
        isInDrawing = false;
        progress = 0;
        currentMouseOverNodesList.Clear();
    }

    public void AddProgressEvent(UnityAction<float> progressEvent)
    {
        this.progressEvent += progressEvent;
    }


    public void Init(UIEventNode startNode, UIEventNode endNode, UnityAction<float> progressEvent)
    {
        this.myCamera = Camera.main;
        this.startNode = startNode;
        this.endNode = endNode;
        this.progressEvent += progressEvent;
        SetNodeEvent(startNode, endNode);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        if (startNode == null || endNode == null)
        {
            Debug.LogError("未添加EventNode");
            return;
        }

        myCamera = Camera.main;
        SetNodeEvent(startNode, endNode);
    }

    private void SetNodeEvent(UIEventNode startNode, UIEventNode endNode)
    {
        currentMouseOverNodesList = new List<GameObject>();

        startNode.AddEnterAciton((obj) =>
        {
            if (isDrawComplete)
                return;

            if (!currentMouseOverNodesList.Contains(obj) && isStartDrawLine)
            {
                currentMouseOverNodesList.Add(obj);
            }
            else
            {
                OnDrawDone(false);
            }
        });

        startNode.AddMouseDownAciton((obj) =>
        {
            if (isDrawComplete)
                return;

            if (currentMouseOverNodesList.Count == 0)
            {
                currentMouseOverNodesList.Add(obj);
            }
        });



        endNode.AddEnterAciton((obj) =>
        {
            if (isDrawComplete)
                return;
            if (!currentMouseOverNodesList.Contains(obj) && currentMouseOverNodesList.Count > 0 && isStartDrawLine)
            {
                currentMouseOverNodesList.Add(obj);
            }
        });
    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="isCorrect">是否正确</param>
    private void OnDrawDone(bool isCorrect)
    {
        isStartDrawLine = false;

        isInDrawing = false;
        if (isCorrect)
        {
            progress = 1;
            OnDrawUpdate(progress);
            isDrawComplete = true;
        }
        else
        {
            progress = 0;
            OnDrawUpdate(progress);
            currentMouseOverNodesList.Clear();
        }
    }
    /// <summary>
    /// 开始拖拽
    /// </summary>
    private void OnDrawStart()
    {
        isStartDrawLine = true;
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

        if (isDrawComplete || !interactalbe)
            return;


        if (Input.GetMouseButtonDown(0))
        {
            OnDrawStart();
        }


        if (Input.GetMouseButtonUp(0))
        {
            OnDrawDone(false);
        }


        if (isStartDrawLine)
        {
            progress = GetProgress();

            OnDrawUpdate(progress);

            if (currentMouseOverNodesList.Count == 2 && currentMouseOverNodesList[0].GetInstanceID() == startNode.gameObject.GetInstanceID())
            {
                OnDrawDone(true);
            }
        }
    }
    /// <summary>
    /// 计算鼠标三维位置在两个节点组成的向量上的投影长度，除以两个节点的距离，得出进度值
    /// </summary>
    /// <returns></returns>
    float GetProgress()
    {
        if (currentMouseOverNodesList.Count > 0 && currentMouseOverNodesList[0].GetInstanceID() == startNode.gameObject.GetInstanceID())
        {
            isInDrawing = true;


            float projectionLength = 0;



            Vector3 startNodeScreenPosition = myCamera.WorldToScreenPoint(startNode.transform.position);

            Vector3 endNodeScreenPosition = myCamera.WorldToScreenPoint(endNode.transform.position);

            projectionLength = Vector2.Dot((Input.mousePosition - startNodeScreenPosition), (endNodeScreenPosition - startNodeScreenPosition)) / Vector2.Distance(endNodeScreenPosition, startNodeScreenPosition);




            float progress = projectionLength / NodesDistance;

            return Mathf.Clamp(progress, 0, 0.99f);
        }
        else
        {
            return 0;
        }
    }
    public void Destory()
    {
        startNode.Destroy();
        endNode.Destroy();
        Destroy(gameObject);
    }
}
