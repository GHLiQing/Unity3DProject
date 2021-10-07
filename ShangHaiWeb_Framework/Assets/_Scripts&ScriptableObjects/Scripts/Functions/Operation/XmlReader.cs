using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class XmlReader
{
    private XmlElement root;//根节点

    private XmlDocument xmlDoc;

    public XmlReader()
    {
    }


    private string GetPath(string path)
    {
#if UNITY_WEBGL || UNITY_WEBPLAYER

#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        path = "file://" + path;
#endif
        return path;
    }

    private void LoadXml_resourse(string resourcePath)
    {
        if (xmlDoc == null)
        {
            TextAsset text = Resources.Load<TextAsset>(resourcePath);
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(text.text);
            root = xmlDoc.DocumentElement;
        }
    }


    #region 对外接口


    #region WWW
    public IEnumerator LoadXml_WWW(string wwwPath)
    {
        if (xmlDoc == null)
        {
            wwwPath = GetPath(wwwPath);

            Debug.Log(wwwPath);

            WWW www = new WWW(wwwPath);

            yield return www;

            if (www.error == null)
            {
                xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(www.text);

                root = xmlDoc.DocumentElement;
            }
            else
            {
                Debug.Log(www.error);
            };
        }
    }

    public XmlElement GetRootNode()
    {
        return root;
    }
    public XmlNode GetSingleNode(XmlNode parent, string nodeName)
    {
        if (root == null)
        {
            Debug.LogError("root节点为空");

            return null;
        }
        for (int i = 0; i < parent.ChildNodes.Count; i++)
        {
            if (parent.ChildNodes[i].Name == nodeName)
            {
                return parent.ChildNodes[i];
            }
        }
        return null;

    }

    public XmlNode SearchSingleNode(string nodeName)
    {
        if (root == null)
        {
            Debug.LogError("root节点为空");

            return null;
        }

        return SearchNode(root, nodeName);
    }
    #endregion
    private XmlNode SearchNode(XmlNode node, string nodeName)
    {

        if (node.Name == nodeName)
        {
            return node;
        }

        for (int i = 0; i < node.ChildNodes.Count; i++)
        {
            var n = SearchNode(node.ChildNodes[i], nodeName);
            if (n != null)
                return n;
        }
        return null;
    }

    #region Resource
    /// <summary>
    /// resources加载 获取根节点
    /// </summary>
    /// <returns></returns>
    public XmlElement LoadXmlRoot_Resource(string resourcePath)
    {
        LoadXml_resourse(resourcePath);

        return root;
    }
    #endregion


    #endregion
}
