using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件池 单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventPool<T> where T : BaseEvent
{
    private static EventPool<T> event_manager;

    public static EventPool<T> GetInstance
    {
        get
        {
            if (event_manager == null)
            {
                event_manager = new EventPool<T>();
                //Debug.Log("new:"+typeof(T).Name);
            }
            return event_manager;
        }
    }
    private Dictionary<int, EventHandle<T>> evetList = new Dictionary<int, EventHandle<T>>();//注册的消息
    private Queue<Event> messageList = new Queue<Event>();// 抛出的消息 队列

    public void Update()
    {
        if (messageList.Count > 0)
        {
            Event date = messageList.Dequeue();
            // Debug.Log("get message:" + ((EventID)date.eh.eventId).ToString());？？？
            HandleEvent(date.sender, date.eh);
        }
    }
    public void Update_unbelievable()
    {
        if (messageList.Count > 10)
        {
            Debug.Log("clear");
            messageList.Clear();
        }
        else if (messageList.Count > 0)
        {
            Event date = messageList.Dequeue();
            // Debug.Log("get message:" + ((EventID)date.eh.eventId).ToString());？？？
            HandleEvent(date.sender, date.eh);
        }
    }
	/// <summary>
	/// 注册事件
	/// </summary>
	/// <param name="id"></param>
	/// <param name="eh"></param>
    public void SubscribeEvent(int id, EventHandle<T> eh)
    {
        if (evetList == null)
            evetList = new Dictionary<int, EventHandle<T>>();
        if (evetList.ContainsKey(id))
        {
            evetList[id] += eh;
        }
        else
        {
            evetList.Add(id, eh);
        }

    }
	/// <summary>
	/// 移除监听 取消订阅
	/// </summary>
	/// <param name="id"></param>
	/// <param name="eh"></param>
    public void UnSubscribeEvent(int id, EventHandle<T> eh)
    {
        if (evetList == null)
            return;
        if (evetList.ContainsKey(id))
        {
            evetList[id] -= eh;
            if (evetList[id] == null)
                evetList.Remove(id);
        }
    }
	/// <summary>
	/// 进行消息的抛出
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="id"></param>
	public void PostEvent(object sender, T id)
    {
        Event eventNode = new Event(sender, id);
		//锁住将要抛出的消息
        lock (messageList)
        {
            //  Debug.Log("get message:"+typeof(T).Name);
            //if (messageList.Count > 10)
            //{
            //    messageList.Clear();
            //    Debug.Log("clear");
            //}
            messageList.Enqueue(eventNode);
			
        }
    }

	/// <summary>
	/// 处理事件
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    private void HandleEvent(object sender, T e)
    {
        int eventId = e.eventId;
        EventHandle<T> handlers = null;
        if (evetList.TryGetValue(eventId, out handlers))
        {

            if (handlers != null)
            {
                handlers(sender, e);
            }
            //else
            //{
            //    Debug.Log("id:" + eventId + "null");
            //}
        }
        else
        {
            Debug.LogWarning("id:" + ((EventID)eventId).ToString() + "  null");
        }
    }

    public class Event
    {
        public readonly T eh;
        public readonly object sender;

        public Event(object sender, T id)
        {
            this.sender = sender;

            this.eh = id;

            //   this.eh.eventId = id;
        }
    }
}

public delegate void EventHandle<T>(object sender, T e) where T : BaseEvent;
