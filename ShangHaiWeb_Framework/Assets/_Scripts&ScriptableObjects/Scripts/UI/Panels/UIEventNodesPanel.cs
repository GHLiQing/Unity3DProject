using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIEventNodesPanel : Panel
{
    public UIEventNode[] nodePrefabs;//node的预制体列表
    public UIEventNodeLine[] nodelinePrefabs;//nodeline的预制体列表
    public UIEventNodeMutiLines[] mutiNodelinesPrefabs;//mutiNodeLine的预制体列表
    /// <summary>
    /// node的池，Creat之后自动存入，dispose自动清除
    /// </summary>
    private Dictionary<string, UIEventNode> singleNodeDic = new Dictionary<string, UIEventNode>();
    /// <summary>
    /// node池的只读属性，用来获取其中元素
    /// </summary>
    public Dictionary<string, UIEventNode> SingleNodeDic
    {
        get
        {
            return singleNodeDic;
        }
    }
    /// <summary>
    /// nodeline的池，creat之后自动存入，dispose自动清除
    /// </summary>
    private Dictionary<string, UIEventNodeLine> nodeLineDic = new Dictionary<string, UIEventNodeLine>();
    /// <summary>
    /// nodeline池的只读属性
    /// </summary>
    public Dictionary<string, UIEventNodeLine> NodeLineDic
    {
        get
        {
            return nodeLineDic;
        }
    }
    /// <summary>
    /// mutiNodeline的池，creat之后自动存入，dispose自动清除
    /// </summary>
    private Dictionary<string, UIEventNodeMutiLines> mutiLinesDic = new Dictionary<string, UIEventNodeMutiLines>();
    /// <summary>
    /// mutiNodeline池的只读属性
    /// </summary>
    public Dictionary<string, UIEventNodeMutiLines> MutiLinesDic
    {
        get
        {
            return mutiLinesDic;
        }
    }
    /// <summary>
    /// Creat一个node组件
    /// </summary>
    /// <param name="nodeName"></param>
    /// <param name="position"></param>
    /// <param name="scale"></param>
    /// <param name="nodePrefab"></param>
    /// <param name="onEnterAction"></param>
    /// <param name="onExitAction"></param>
    /// <param name="onClickAction"></param>
    /// <param name="OnMouseStayAction"></param>
    /// <param name="OnMouseDownAction"></param>
    /// <returns></returns>
    public UIEventNode CreatSingleNode(string nodeName, UIEventNode nodePrefab,
        EventTriggerListener.VoidDelegate onEnterAction = null, EventTriggerListener.VoidDelegate onExitAction = null,
        EventTriggerListener.VoidDelegate onClickAction = null, EventTriggerListener.VoidDelegate OnMouseStayAction = null,
        EventTriggerListener.VoidDelegate OnMouseDownAction = null)
    {
        UIEventNode node = Instantiate(nodePrefab, transform, false);

        node.gameObject.name = nodeName;

        if (onEnterAction != null)
            node.AddEnterAciton(onEnterAction);

        if (onExitAction != null)
            node.AddExitAciton(onExitAction);

        if (onClickAction != null)
            node.AddClickAciton(onClickAction);

        if (OnMouseStayAction != null)
            node.AddMouseOverAciton(OnMouseStayAction);

        if (OnMouseDownAction != null)
            node.AddMouseDownAciton(OnMouseDownAction);

        singleNodeDic.Add(nodeName, node);

        return node;
    }

    /// <summary>
    /// Creat一个直线的连线
    /// </summary>
    /// <param name="nodelineName"></param>
    /// <param name="nodelinePosition"></param>
    /// <param name="nodelinePrefab"></param>
    /// <param name="progressEvent"></param>
    /// <returns></returns>
    public UIEventNodeLine CreatUIEventLine(string nodelineName, Vector3 localPosition, UIEventNodeLine nodelinePrefab, UnityAction<float> progressEvent)
    {
        UIEventNodeLine nodeline = Instantiate(nodelinePrefab, transform);
        nodeline.transform.localPosition = localPosition;
        nodeline.AddProgressEvent(progressEvent);
        nodeline.gameObject.name = nodelineName;
        nodeLineDic.Add(nodelineName, nodeline);
        return nodeline;
    }
    public UIEventNodeLine CreatUIEventLine(string nodelineName, UIEventNodeLine nodelinePrefab, UnityAction<float> progressEvent)
    {
        UIEventNodeLine nodeline = Instantiate(nodelinePrefab, transform, false);
        nodeline.AddProgressEvent(progressEvent);
        nodeline.gameObject.name = nodelineName;
        nodeLineDic.Add(nodelineName, nodeline);
        return nodeline;
    }

    /// <summary>
    /// Creat一个曲线（多条直线）的连线
    /// </summary>
    /// <param name="mutiNodelineName"></param>
    /// <param name="mutiLinesPosition"></param>
    /// <param name="mutiLinePrefab"></param>
    /// <param name="progressEvent"></param>
    /// <returns></returns>
    public UIEventNodeMutiLines CreatMutiLines(string mutiNodelineName, Vector3 localPosition, UIEventNodeMutiLines mutiLinePrefab, UnityAction<float> progressEvent)
    {
        UIEventNodeMutiLines mutiLine = Instantiate(mutiLinePrefab, transform);
        mutiLine.transform.localPosition = localPosition;
        mutiLine.AddProgressEvent(progressEvent);
        mutiLine.gameObject.name = mutiNodelineName;
        mutiLinesDic.Add(mutiNodelineName, mutiLine);
        return mutiLine;
    }
    public UIEventNodeMutiLines CreatMutiLines(string mutiNodelineName, UIEventNodeMutiLines mutiLinePrefab, UnityAction<float> progressEvent)
    {
        UIEventNodeMutiLines mutiLine = Instantiate(mutiLinePrefab, transform, false);
        mutiLine.AddProgressEvent(progressEvent);
        mutiLine.gameObject.name = mutiNodelineName;
        mutiLinesDic.Add(mutiNodelineName, mutiLine);
        return mutiLine;
    }
    /// <summary>
    ///  Creat一个曲线（多条直线）的连线
    /// </summary>
    /// <param name="mutilineName"></param>
    /// <param name="nodePositions"></param>
    /// <param name="nodePrefab"></param>
    /// <param name="progressEvent"></param>
    /// <returns></returns>
    public UIEventNodeMutiLines CreatMutiLines(string mutilineName, Vector3[] nodePositions, UIEventNode nodePrefab, UnityAction<float> progressEvent)
    {
        if (nodePositions == null || nodePositions.Length <= 2)
        {
            Debug.Log("节点数量不足");
            return null;
        }

        UIEventNode[] nodes = new UIEventNode[nodePositions.Length];

        for (int i = 0; i < nodePositions.Length; i++)
        {
            UIEventNode node = Instantiate(nodePrefab, transform);
            node.SetTransformInfo(nodePositions[i], node.transform.eulerAngles, node.transform.localScale);
            node.name = mutilineName + "_node_" + i.ToString();
            nodes[i] = node;
        }

        UIEventNodeMutiLines mutiNodelines = (Instantiate(new GameObject(mutilineName), transform)).AddComponent<UIEventNodeMutiLines>();

        mutiNodelines.gameObject.AddComponent<RectTransform>();

        mutiNodelines.gameObject.name = mutilineName;

        mutiNodelines.initOnAwake = false;

        mutiNodelines.debugProgress = true;

        mutiNodelines.Init(nodes, progressEvent);

        mutiLinesDic.Add(mutilineName, mutiNodelines);

        return mutiNodelines;
    }
    public UIEventNodeMutiLines CreatMutiLines(string mutilineName, Transform[] nodePositionTargets, UIEventNode nodePrefab, UnityAction<float> progressEvent)
    {
        if (nodePositionTargets == null || nodePositionTargets.Length <= 2)
        {
            Debug.Log("节点数量不足");
            return null;
        }

        UIEventNode[] nodes = new UIEventNode[nodePositionTargets.Length];

        for (int i = 0; i < nodePositionTargets.Length; i++)
        {
            UIEventNode node = Instantiate(nodePrefab, transform);
            node.SetTransformInfo(nodePositionTargets[i].position, nodePositionTargets[i].eulerAngles, nodePositionTargets[i].localScale);
            node.name = mutilineName + "_node_" + i.ToString();
            nodes[i] = node;
        }

        UIEventNodeMutiLines mutiNodelines = (Instantiate(new GameObject(mutilineName), transform)).AddComponent<UIEventNodeMutiLines>();

        mutiNodelines.gameObject.AddComponent<RectTransform>();

        mutiNodelines.gameObject.name = mutilineName;

        mutiNodelines.initOnAwake = false;

        mutiNodelines.debugProgress = true;

        mutiNodelines.Init(nodes, progressEvent);

        mutiLinesDic.Add(mutilineName, mutiNodelines);

        return mutiNodelines;
    }
    /// <summary>
    /// 销毁并从池中清除该node
    /// </summary>
    /// <param name="nodeName"></param>
    public void DisposeSingleNode(string nodeName)
    {
        if (!singleNodeDic.ContainsKey(nodeName))
            return;
        UIEventNode node = singleNodeDic[nodeName];
        node.Destroy();
        singleNodeDic.Remove(nodeName);
    }
    /// <summary>
    /// 销毁并清除池中所有node
    /// </summary>
    public void DisposeAllSingleNodes()
    {
        foreach (var key in singleNodeDic.Keys)
        {
            singleNodeDic[key].Destroy();
        }
        singleNodeDic.Clear();
    }
    /// <summary>
    /// 销毁并从池中清除该nodeline
    /// </summary>
    /// <param name="nodelineName"></param>
    public void DisposeUIEventLine(string nodelineName)
    {
        if (!nodeLineDic.ContainsKey(nodelineName))
            return;
        UIEventNodeLine nodeline = nodeLineDic[nodelineName];
        nodeline.Destory();
        nodeLineDic.Remove(nodelineName);
    }
    /// <summary>
    /// 销毁并清除池中所有nodeline
    /// </summary>
    public void DisposeAllUIEventLines()
    {
        foreach (var key in nodeLineDic.Keys)
        {
            nodeLineDic[key].Destory();
        }
        nodeLineDic.Clear();
    }
    /// <summary>
    /// 销毁并从池中清除该mutiNodeline
    /// </summary>
    /// <param name="mutiNodeLineName"></param>
    public void DisposeMutilines(string mutiNodeLineName)
    {
        if (!mutiLinesDic.ContainsKey(mutiNodeLineName))
            return;
        UIEventNodeMutiLines mutiNodeline = mutiLinesDic[mutiNodeLineName];
        mutiNodeline.Destroy();
        mutiLinesDic.Remove(mutiNodeLineName);
    }
    /// <summary>
    /// 销毁并清除池中所有mutiNodelines
    /// </summary>
    public void DisposeAllMutilines()
    {
        foreach (var key in mutiLinesDic.Keys)
        {
            mutiLinesDic[key].Destroy();
        }
        mutiLinesDic.Clear();
    }
    public override void SetPanelDisplay(bool display)
    {
        this.isDisplay = display;
        this.gameObject.SetActive(display);
    }
}
