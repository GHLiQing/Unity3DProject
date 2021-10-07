using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BtnFunc : MonoBehaviour
{

    Image img;
    Image nei;
    Image wai;

    // List<Tweener> list_tween=new List<Tweener>();
    void Start()
    {
      


    }
    private void OnEnable()
    {
        if (img == null)
        {
            img = GetComponent<Image>();
            nei = transform.Find("nei").GetComponent<Image>();
            wai = transform.Find("wai").GetComponent<Image>();

            img.type = Image.Type.Filled;
            nei.type = Image.Type.Filled;
            wai.type = Image.Type.Filled;

            if (transform.name.Equals("lilun") || transform.name.Equals("jiaoxue") || transform.name.Equals("xinxi"))
            {
                //  img.fillMethod
                img.fillOrigin = (int)Image.Origin360.Top;
                nei.fillOrigin = (int)Image.Origin360.Top;
                wai.fillOrigin = (int)Image.Origin360.Top;
            }
        }

        img.fillAmount = 0;
        nei.fillAmount = 0;
        wai.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnShow(UnityAction end = null)
    {


        ShowFill(img, end);
        ShowFill(nei, end);
        ShowFill(wai, end);
    }
    public void OnSelected()
    {

    }

    void ShowFill(Image img_, UnityAction end = null)
    {
        img_.fillAmount = 0;
        Tweener t = DOTween.To(() => { return img_.fillAmount; }, (v) => { img_.fillAmount = v; }, 1, 1).OnComplete(() =>
          {
              if (end != null)
                  end();
          }).SetId("UIani");
        //  list_tween.Add(t);
    }
    private void OnDisable()
    {
        //for (int i = list_tween.Count - 1; i >= 0; i--)
        //{
        //  //  DOTween.Kill(list_tween[i],false);
        //  //  list_tween.Remove(list_tween[i]);
        //}
   
        //  list_tween.Clear();
        img.fillAmount = 0;
        nei.fillAmount = 0;
        wai.fillAmount = 0;
    }

}
