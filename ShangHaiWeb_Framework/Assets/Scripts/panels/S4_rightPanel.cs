using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S4_rightPanel : Panel
{
    public override void SetPanelDisplay(bool display)
    {
        //throw new System.NotImplementedException();
    }
    Transform lilun;
    protected override void FindObj()
    {
        lilun = transform.Find("RightBG/lilun");
    }
    protected override void RegistEvent()
    {
        EventTriggerListener.Get(lilun.gameObject).onClick = (obj) => 
        {
            DataManager.s04 = "根管治疗";
            Application.ExternalCall("UnityToJs",DataManager.GetStr(StrType._s04) );
        };
    }
}
