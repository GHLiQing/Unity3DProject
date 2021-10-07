using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseEvent : EventArgs
{
    public readonly object obj;
    public int eventId
    {
        get;
        set;
    }
    public BaseEvent(int id, object obj)
    {
        eventId = id;
        this.obj = obj;
    }
    public BaseEvent(int id)
    {
        eventId = id;
        this.obj = null;
    }

}
public class StrData:BaseEvent
{
    public StrData(int id, object obj) : base(id, obj)
    {
        
    }
    public StrData(int id) : base(id)
    {
      
    }
}
public class UIData : BaseEvent
{
    public UIData(int id, object obj) : base(id, obj)
    {

    }
    public UIData(int id) : base(id)
    {

    }
}
public class TestData : BaseEvent
{
	public TestData(int id, object obj) : base(id, obj)
	{
	}
	public TestData(int id) : base(id)
	{

	}

}
public class MovePlayerData
{
    public Vector3 pos;
    public Vector2 angle;
    public float t;
    public MovePlayerData(Vector3 pos,Vector2 angle,float t)
    {
        this.pos = pos;
        this.angle = angle;
        this.t = t;
    }
}

public enum EventID
{
  OnLoadScene,//场景加载完毕  0
  SkipAni,//略过动画  1
  MovePlayer,//移动角色  2
  Test,//测试
}
