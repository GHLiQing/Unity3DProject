using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Top : Panel
{
    public override void SetPanelDisplay(bool display)
    {
      
    }
    Transform home, back;
    protected override void FindObj()
    {
        home = transform.Find("BG/home");
        back = transform.Find("BG/back");
    }
    protected override void RegistEvent()
    {
        EventTriggerListener.Get(home.gameObject).onClick = (obj) => 
        {
            LoadingManager_lightmap.Instance.LoadScene("s01");
        };
        EventTriggerListener.Get(back.gameObject).onClick = (obj) => 
        {
            LoadingManager_lightmap.Instance.LoadScene("s03");
        };
    }
}
