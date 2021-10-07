using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class RightPanel : Panel
{
    Transform rightbg, skip, bottom, rtbtn;
    Transform lilun, shijian, jiaoxue, xinxi, tushu, xuesheng;
    Texture[] allTexture;
    int texLength, currentIndex;

    Image earth;

    WowMainCamera wow;

    public Transform player;
    CharacterMovement cm;

    protected override void FindObj()
    {
        rightbg = transform.Find("RightBG");
        skip = transform.Find("skip");
        lilun = rightbg.Find("lilun");
        shijian = rightbg.Find("shijian");
        jiaoxue = rightbg.Find("jiaoxue");
        xinxi = rightbg.Find("xinxi");
        tushu = rightbg.Find("tushu");
        xuesheng = rightbg.Find("xuesheng");
        bottom = transform.Find("bottom");
        rtbtn = transform.Find("RT");
        earth = rightbg.Find("earth/earth").GetComponent<Image>();

        allTexture = Resources.LoadAll<Texture>("旋转的地球");
        texLength = allTexture.Length;
        currentIndex = 0;
        //   Debug.Log(texLength);
        player = GameObject.Find("PLAYER").transform;
        wow = player.Find("Main Camera").GetComponent<WowMainCamera>();
        cm = player.Find("Capsule").GetComponent<CharacterMovement>();
    }

    protected override void RegistEvent()
    {
        EventTriggerListener.Get(skip.gameObject).onClick = (obj) =>
        {
			EventPool<UIData>.GetInstance.PostEvent(this, new UIData((int)EventID.SkipAni));
			//抛出消息，testData可以使用一个参数或者两个参数的构造方法 进行参数的传递
			//EventPool<TestData>.GetInstance.PostEvent(this, new TestData((int)EventID.Test,gameObject));
		};
        EventTriggerListener.Get(lilun.gameObject).onClick = (obj) =>
        {
			//加载场景，这里用了类是异步加载
            LoadingManager_lightmap.Instance.LoadScene("s03");
            DataManager.s01 = "理论学习";
        };
        EventTriggerListener.Get(shijian.gameObject).onClick = (obj) =>
        {
            LoadingManager_lightmap.Instance.LoadScene("s03");
            DataManager.s01 = "实践操作";
        };
        EventTriggerListener.Get(jiaoxue.gameObject).onClick = (obj) =>
        {
            Application.ExternalCall("UnityToJs", "教学管理");
        };
        EventTriggerListener.Get(xinxi.gameObject).onClick = (obj) =>
        {
			Application.ExternalCall("UnityToJs", "信息发布");
        };
        EventTriggerListener.Get(tushu.gameObject).onClick = (obj) =>
        {
            LoadingManager_lightmap.Instance.LoadScene("s06");
        };
        EventTriggerListener.Get(xuesheng.gameObject).onClick = (obj) =>
        {
            Application.ExternalCall("UnityToJs", "学生园地");

        };
        for (int i = 0; i < rightbg.childCount; i++)
        {
            string name = rightbg.GetChild(i).name;
            if (name.Equals("earth") || name.Equals("BG"))
                continue;
            EventTriggerListener.Get(rightbg.GetChild(i).gameObject).onEnter = (obj) =>
            {
                obj.transform.Find("nei").GetComponent<Image>().color = new Color(236 / 255f, 223 / 255f, 132 / 255f);
                obj.transform.Find("wai").GetComponent<Image>().color = new Color(236 / 255f, 223 / 255f, 132 / 255f);
            };
            EventTriggerListener.Get(rightbg.GetChild(i).gameObject).onExit = (obj) =>
            {
                obj.transform.Find("nei").GetComponent<Image>().color = Color.white;
                obj.transform.Find("wai").GetComponent<Image>().color = Color.white;
            };
        }
        EventTriggerListener.Get(rtbtn.gameObject).onClick = (obj) =>
        {
            if (rightbg.gameObject.activeInHierarchy)
            {
                //  rightbg.gameObject.SetActive(false);
                ShowRightBG(false);
                rtbtn.Find("shang").DOScale(0, 0.5f);
                CanControl(true);
                DOTween.Kill("UIani");
            }
            else
            {
                //  rightbg.gameObject.SetActive(true);
                ShowRightBG(true);
                rtbtn.Find("shang").DOScale(1.5f, 0.3f).OnComplete(() =>
                {
                    rtbtn.Find("shang").DOScale(1, 0.5f);
                });
                CanControl(false);
            }
        };


    }

	

	public override void SetPanelDisplay(bool display)
    {
		
    }

    public void SetRightPanel(bool a)
    {
        if (a)
        {
            skip.gameObject.SetActive(false);
            //    rightbg.gameObject.SetActive(true);
            ShowRightBG(true);
            bottom.gameObject.SetActive(true);
            rtbtn.gameObject.SetActive(true);
        }
        else
        {
            skip.gameObject.SetActive(true);
            ShowRightBG(false);
            bottom.gameObject.SetActive(false);
            rtbtn.gameObject.SetActive(false);
        }
    }

    void ShowRightBG(bool a)
    {
        if (a)
        {
            rightbg.gameObject.SetActive(true);

            //for (int i = 0; i < rightbg.childCount; i++)
            //{
            //    BtnFunc bfc = rightbg.GetChild(i).GetComponent<BtnFunc>();
            //    if (bfc == null)
            //        continue;
            //    bfc.OnShow();
            //}

            ShowAni(rightbg.Find("shijian"));
            ShowAni(rightbg.Find("lilun"),()=> 
            {
                ShowAni(rightbg.Find("tushu"));
                ShowAni(rightbg.Find("jiaoxue"), () => 
                {
                    ShowAni(rightbg.Find("xuesheng"));
                    ShowAni(rightbg.Find("xinxi"));
                });
            });
        }
        else
        {
            rightbg.gameObject.SetActive(false);
        }
    }
    void ShowAni(Transform tra,UnityAction end=null)
    {
        BtnFunc bfc = tra.GetComponent<BtnFunc>();
        if (bfc == null)
            return;
        bfc.OnShow(end);
    }

    public void ShowImage(int index)
    {
        if (allTexture == null || allTexture.Length == 0)
            return;
        // Debug.Log(index);
        Texture2D t2d = allTexture[index] as Texture2D;
        earth.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
    }
    const int frame = 2;
    int currentframe;
    private void Update()
    {
        currentframe++;
        if (currentframe >= frame)
        {
            currentframe = 0;
            ChangeImage();
        }
    }
    void ChangeImage()
    {
        currentIndex++;
        if (currentIndex >= texLength)
        {
            currentIndex = 0;
        }
        ShowImage(currentIndex);
    }

    public void CanControl(bool a)
    {
        wow.enabled = a;
        cm.enabled = a;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        DOTween.Kill("UIani");
    }
	
}
