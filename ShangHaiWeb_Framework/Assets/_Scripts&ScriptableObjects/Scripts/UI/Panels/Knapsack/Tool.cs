using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// tool道具类，挂在物体上，手动填入名称，也可以动态更改，用于显示物品标签或背包系统
/// </summary>
public class Tool : MonoBehaviour
{
    public bool registeMyselfOnAwake = true;

    /// <summary>
    /// 注意toolname尽量不要相同
    /// </summary>
    public HUDType hudType; //当前组件的Hud类型

    public Vector2 hudOffset;//hud显示的偏移量

    [SerializeField]
    private string toolName; //道具名称

    public string ToolName
    {
        get
        {
            return toolName;
        }
        set
        {
            if (toolName != value)
            {
                if (OnToolNameChanged != null)
                    OnToolNameChanged(value);
                toolName = value;
            }
        }
    }

    public UnityAction<string> OnToolNameChanged;//用于名称改变时改变对应的hud text
    public UnityAction<bool> OnToolActiveChanged;//用于名称改变时改变对应的hud text

    public GameObject ToolObj//本身gameobject
    {
        get { return this.gameObject; }
    }

    public bool isActiveOnAwake = true;

    private void Awake()
    {
        if (registeMyselfOnAwake)
        {
            //if (UIManager.Instance != null)
            //    UIManager.Instance.RegistTool(this);
        }

        if (!isActiveOnAwake)
        {
            gameObject.SetActive(isActiveOnAwake);
        }
    }

    private void OnDestroy()
    {
        OnToolActiveChanged = null;
        OnToolNameChanged = null;

        //if (UIManager.Instance != null)
        //    UIManager.Instance.UnRegistTool(this);
    }

    private void OnDisable()
    {
        if (OnToolActiveChanged != null)
        {
            OnToolActiveChanged(false);
        }
    }
    private void OnEnable()
    {
        if (OnToolActiveChanged != null)
        {
            OnToolActiveChanged(true);
        }
    }
    private void OnApplicationQuit()
    {
        OnToolActiveChanged = null;
        OnToolNameChanged = null;
    }
}
