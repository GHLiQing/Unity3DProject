using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class ExportToXml
{
    [MenuItem("Export/ExportTest")]
    static void ExportTest()
    {
        //获取当前场景完整路径
        string scenePath = EditorApplication.currentScene;
        Debug.Log("scenePath:"+ scenePath);
        Debug.Log("LastIndexOf:" + scenePath.LastIndexOf("/"));
    }

    [MenuItem("Export/ExportScene----将当前场景导出为Xml")]
    static void ExportGameObjects()
    {
        //获取当前场景完整路径
        string scenePath = EditorApplication.currentScene;
        //获取当前场景名称
        string sceneName = scenePath.Substring(scenePath.LastIndexOf("/") + 1, scenePath.Length - scenePath.LastIndexOf("/") - 1);
        sceneName = sceneName.Substring(0, sceneName.LastIndexOf("."));
        //获取保存路径
        string savePath = EditorUtility.SaveFilePanel("输出场景内物体", "", sceneName, "xml");
        //创建Xml文件
        XmlDocument xmlDoc = new XmlDocument();
        //创建根节点
        XmlElement scene = xmlDoc.CreateElement("Scene");
        scene.SetAttribute("Name", sceneName);
        scene.SetAttribute("Asset", scenePath);
        xmlDoc.AppendChild(scene);
        //遍历场景中的所有物体
        foreach (GameObject go in Object.FindObjectsOfType(typeof(GameObject)))
        {
            //仅导出场景中的父物体
            if (go.transform.parent == null)
            {
                //创建每个物体
                XmlElement gameObject = xmlDoc.CreateElement("GameObject");
                gameObject.SetAttribute("Name", go.name);
                gameObject.SetAttribute("Asset", "Prefabs/" + go.name + ".prefab");
                //创建Transform
                XmlElement transform = xmlDoc.CreateElement("Transform");
                transform.SetAttribute("x", go.transform.position.x.ToString());
                transform.SetAttribute("y", go.transform.position.y.ToString());
                transform.SetAttribute("z", go.transform.position.z.ToString());
                gameObject.AppendChild(transform);
                //创建Rotation
                XmlElement rotation = xmlDoc.CreateElement("Rotation");
                rotation.SetAttribute("x", go.transform.eulerAngles.x.ToString());
                rotation.SetAttribute("y", go.transform.eulerAngles.y.ToString());
                rotation.SetAttribute("z", go.transform.eulerAngles.z.ToString());
                gameObject.AppendChild(rotation);
                //创建Scale
                XmlElement scale = xmlDoc.CreateElement("Scale");
                scale.SetAttribute("x", go.transform.localScale.x.ToString());
                scale.SetAttribute("y", go.transform.localScale.y.ToString());
                scale.SetAttribute("z", go.transform.localScale.z.ToString());
                gameObject.AppendChild(scale);
                //添加物体到根节点
                scene.AppendChild(gameObject);
            }
        }

        xmlDoc.Save(savePath);
    }

}
