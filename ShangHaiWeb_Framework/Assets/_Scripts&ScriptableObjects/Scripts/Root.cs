using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class Root : MonoBehaviour
{
    #region 此场景的Step数据
    public List<Step> allSteps;

    public Step currentStep;

    public Step remainStep;

    public bool isRunning = true;
	#endregion
	#region 动态加载的UI面板
	
    #endregion
    // 添加各个模块脚本
    void Awake()
    {
	
		UIManager.Instance.Init();

        ProcessManager.Instance.Init();

        LoadAssetManager.Instance.Init();

		//AudioVideoManager.Instance.Init();

		// EvalutionManager.Instance.Init();
		
    }

}
