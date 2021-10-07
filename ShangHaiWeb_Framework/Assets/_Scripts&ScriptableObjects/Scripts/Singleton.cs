
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 单例模板，调用对应单例自动生成gameobject并添加对应组件
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' 已销毁，请不要再调用此单例");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    if (FindObjectsOfType(typeof(T)).Length > 0)
                    {
                        Debug.LogError("[Singleton] 当前场景中包含多个此类实例，请检查并确保只保留1个：" + typeof(T));
                        return null;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();

                        _instance = singleton.AddComponent<T>();

                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);
                    }
                }

                return _instance;
            }
        }
    }

    public virtual void Init()
    {

    }
    private static bool applicationIsQuitting = false;

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
