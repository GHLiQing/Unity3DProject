using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
public class AssetBundleTools
{
    private static string _dirName = "";

    [MenuItem("Itools/清除选中文件夹中文件的所有标签")]
    static void ResetSelectFolderFileBundleName()
    {
        UnityEngine.Object[] selObj = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Unfiltered);
        foreach (UnityEngine.Object item in selObj)
        {
            string objPath = AssetDatabase.GetAssetPath(item);
            DirectoryInfo dirInfo = new DirectoryInfo(objPath);
            if (dirInfo == null)
            {
                Debug.LogError("******请检查，是否选中了非文件夹对象******");
                return;
            }
            _dirName = null;

            string filePath = dirInfo.FullName.Replace('\\', '/');
            filePath = filePath.Replace(Application.dataPath, "Assets");
            AssetImporter ai = AssetImporter.GetAtPath(filePath);
            ai.assetBundleName = _dirName;

            SetAssetBundleName(dirInfo);
        }
        AssetDatabase.Refresh();
        Debug.Log("******批量清除AssetBundle名称成功******");

    }
    static void SetAssetBundleName(DirectoryInfo dirInfo)
    {
        FileSystemInfo[] files = dirInfo.GetFileSystemInfos();
        foreach (FileSystemInfo file in files)
        {
            if (file is FileInfo && file.Extension != ".meta" && file.Extension != ".txt")
            {
                string filePath = file.FullName.Replace('\\', '/');
                filePath = filePath.Replace(Application.dataPath, "Assets");
                AssetImporter ai = AssetImporter.GetAtPath(filePath);
                ai.assetBundleName = _dirName;
            }
            else if (file is DirectoryInfo)
            {
                string filePath = file.FullName.Replace('\\', '/');
                filePath = filePath.Replace(Application.dataPath, "Assets");
                AssetImporter ai = AssetImporter.GetAtPath(filePath);
                ai.assetBundleName = _dirName;
                SetAssetBundleName(file as DirectoryInfo);
            }
        }
    }
    [MenuItem("Itools/Build AssetBundles From Directory of Files")]
    public static void ExportAssetBundles()
    {
        //streamingassetpath / windows
        string outPath = IPathTools.GetAssetBundlePath();
        Debug.Log("out path == " + outPath);
        if (!Directory.Exists(outPath))
        {
            Directory.CreateDirectory(outPath);
        }
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
        {
            BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        }
        else
        {
            BuildPipeline.BuildAssetBundles(outPath, 0, EditorUserBuildSettings.activeBuildTarget);
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Itools/MarkAssetBundle")]
    public static void MarkAssetBundle()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();

        string path = Application.dataPath + "/Art_AssetBundle/Scenes/";
      //  Debug.Log(path);
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
        for (int i = 0; i < fileInfo.Length; i++)
        {
            FileSystemInfo tmpInfo = fileInfo[i];
            if (tmpInfo is DirectoryInfo)
            {
                string tmpPath = Path.Combine(path, tmpInfo.Name);
                CreatOverView(tmpPath);
            }
        }
        string outPath = IPathTools.GetAssetBundlePath();
        string outPathResources = IPathTools.GetAssetResourcePath();
        CopyRecord(path, outPath);
        CopyRecord(path, outPathResources);
        AssetDatabase.Refresh();
    }
    [MenuItem("Itools/MarkAssetBundle_selected")]
    public static void MarkAssetBundle_selected()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();

     
        UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        //   Debug.LogError(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/" + AssetDatabase.GetAssetPath(arr[0]));


        string path = Application.dataPath + "/Art_AssetBundle/Scenes/";
     
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
        for (int i = 0; i < fileInfo.Length; i++)
        {
            FileSystemInfo tmpInfo = fileInfo[i];
            if (tmpInfo is DirectoryInfo)
            {
                string tmpPath = Path.Combine(path, tmpInfo.Name);
                CreatOverView(tmpPath);
            }
        }
        string outPath = IPathTools.GetAssetBundlePath();
        string outPathResources = IPathTools.GetAssetResourcePath();
        CopyRecord(path, outPath);
        CopyRecord(path, outPathResources);
        AssetDatabase.Refresh();
    }
    [MenuItem("Itools/ChangeAllName")]
    public static void ChangeName()
    {
        int MaxValue = 0;
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (!(o is Object))
                continue;
            string path = AssetDatabase.GetAssetPath(o);
            string[] strs = path.Split(new char[] { '/' });
            if (MaxValue < strs.Length)
            {
                MaxValue = strs.Length;
            }
            if (strs.Length >= MaxValue)
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(o), strs[MaxValue - 2] + o.name);
            }
        }
    }

    [MenuItem("Itools/ChangeRepeatName")]
    public static void ChangeRepeatName()
    {
        int MaxValue = 0;
        Dictionary<string, Object> objects = new Dictionary<string, Object>();
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (!(o is Object))
                continue;
            if (!objects.ContainsKey(o.name))
            {
                objects.Add(o.name, o);
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(o);
                string[] strs = path.Split(new char[] { '/' });
                if (MaxValue < strs.Length)
                {
                    MaxValue = strs.Length;
                }
                if (strs.Length >= MaxValue)
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(o), strs[MaxValue - 2] + o.name);
                }
            }
        }
    }
    [MenuItem("Itools/DeleteName")]
    public static void DeleteName()
    {
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (!(o is Object))
                continue;
            string[] str = o.name.Split(new char[] { '_' });
            o.name = str[1];
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(o), o.name);
        }
    }
    public static void CopyRecord(string surcePath, string disPath)
    {
        DirectoryInfo dir = new DirectoryInfo(surcePath);
        if (!dir.Exists)
        {
            Debug.Log("is not exit");
            return;
        }
        if (!Directory.Exists(disPath))
            Directory.CreateDirectory(disPath);

        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i] as FileInfo;
            if (file != null && file.Extension == ".txt")
            {
                string sourceFile = surcePath + file.Name;
                string disFile = disPath + "/" + file.Name;
                Debug.Log("sourceFile == " + sourceFile);
                Debug.Log("disFile == " + disFile);
                File.Copy(sourceFile, disFile, true);
            }
        }
    }

    public static void CreatOverView(string path)
    {
        string textFileName = "Record.txt";
        string tmpPath = path + textFileName;
     //   Debug.Log(tmpPath);
        FileStream fs = new FileStream(tmpPath, FileMode.OpenOrCreate);
        StreamWriter bw = new StreamWriter(fs);
        //储存对应关系
        Dictionary<string, string> readDict = new Dictionary<string, string>();

        ChangeHead(path, readDict);
        bw.WriteLine(readDict.Count);
        foreach (KeyValuePair<string, string> keyValue in readDict)
        {
            bw.Write(keyValue.Key);
            bw.Write(" ");
            bw.Write(keyValue.Value);
            bw.Write("\n");
        }
        bw.Close();
        fs.Close();
    }
    public static void ChangeHead(string fullPath, Dictionary<string, string> theWriter)
    {
        int tmpCount = fullPath.IndexOf("Assets");
        int tmpLenth = fullPath.Length;
        string replacePath = fullPath.Substring(tmpCount, tmpLenth - tmpCount);

        DirectoryInfo dir = new DirectoryInfo(fullPath);
        if (dir != null)
        {
            ListFiles(dir, replacePath, theWriter);
        }
        else
        {
            Debug.Log("this path is not exit");
        }
    }
    public static void ListFiles(FileSystemInfo info, string replacePath, Dictionary<string, string> theWriter)
    {
        if (!info.Exists)
        {
            Debug.Log("is not exit");
            return;
        }
        DirectoryInfo dir = info as DirectoryInfo;
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i] as FileInfo;
            if (file != null)
            {
                ChangeMark(file, replacePath, theWriter);
            }
            else
            {
                ListFiles(files[i], replacePath, theWriter);
            }
        }
    }

    public static void ChangeMark(FileInfo tmpFile, string replacePath, Dictionary<string, string> theWriter)
    {

        if (tmpFile.Extension == ".meta")
        {
            return;
        }
        string markStr = GetBundlePath(tmpFile, replacePath);
        ChangeAssetMark(tmpFile, markStr, theWriter);
    }
    public static void ChangeAssetMark(FileInfo tmpFile, string markStr, Dictionary<string, string> theWriter)
    {
        string fullPath = tmpFile.FullName;
        string assetPath = fullPath.Substring(fullPath.IndexOf("Assets"));
        AssetImporter importer = AssetImporter.GetAtPath(assetPath);

        importer.assetBundleName = markStr;
        if (tmpFile.Extension == ".unity")
        {
            importer.assetBundleVariant = "u3d";
        }
        else
        {
            importer.assetBundleVariant = "ld";
        }
        string modleName = "";
        string[] subMark = markStr.Split("/".ToCharArray());
        if (subMark.Length > 1)
        {
            modleName = subMark[1];
        }
        else
        {
            modleName = markStr;
        }
        string modlePath = markStr.ToLower() + "." + importer.assetBundleVariant;
        if (!theWriter.ContainsKey(modleName))
        {
            Debug.Log(modleName + " " + modlePath);
            theWriter.Add(modleName, modlePath);
        }
    }
    public static string GetBundlePath(FileInfo file, string replacePath)
    {
        string tmpPath = file.FullName.Replace("\\", "/");
        int assetCount = tmpPath.IndexOf(replacePath);
        assetCount += replacePath.Length + 1;
        int nameCount = tmpPath.LastIndexOf(file.Name);
        int tmpCount = replacePath.LastIndexOf("/");
        string sceneHead = replacePath.Substring(tmpCount + 1, replacePath.Length - tmpCount - 1);
        int tmpLenth = nameCount - assetCount;
        if (tmpLenth > 0)
        {
            string subString = tmpPath.Substring(assetCount, tmpPath.Length - assetCount);
            string[] result = subString.Split("/".ToCharArray());
            return sceneHead + "/" + result[0];
        }
        else
        {
            return sceneHead;
        }
    }
}
