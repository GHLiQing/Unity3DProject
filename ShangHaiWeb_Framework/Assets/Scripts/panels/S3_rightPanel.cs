using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3_rightPanel : Panel
{
    public override void SetPanelDisplay(bool display)
    {
        // throw new System.NotImplementedException();
    }
    Transform kouqiang;
    protected override void FindObj()
    {
        //base.FindObj();
        kouqiang = transform.Find("RightBG/lilun");
    }
    protected override void RegistEvent()
    {
        EventTriggerListener.Get(kouqiang.gameObject).onClick = (obj) => 
        {
            DataManager.s03 = "口腔科";
           switch (DataManager.s01)
            {
                case "理论学习":
                    LoadingManager_lightmap.Instance.LoadScene("s05");
                    break;
                case "实践操作":
                    LoadingManager_lightmap.Instance.LoadScene("s04");
                    break;
            }

           
        };
    }


}
