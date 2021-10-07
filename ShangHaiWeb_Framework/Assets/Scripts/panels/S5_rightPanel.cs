using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S5_rightPanel : Panel
{
    public override void SetPanelDisplay(bool display)
    {
       // throw new System.NotImplementedException();
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
            EventPool<UIData>.GetInstance.PostEvent(this, new UIData((int)EventID.MovePlayer,new MovePlayerData(new Vector3(-5.887249f, 1.085f, 9.369647f),new Vector2(-179.85f, -1.279712e-06f),0.5f)));
        };
    }
}
