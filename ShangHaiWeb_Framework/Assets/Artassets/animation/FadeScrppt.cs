using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeScrppt : MonoBehaviour
{

    Image img;

    Tweener t;
    void OnEnable()
    {
        if (img == null)
        {
            img = GetComponent<Image>();
        }
        t= img.DOFade(0.3f,0.5f).SetLoops(-1,LoopType.Yoyo);
    }
    void OnDisable()
    {
        t.Kill(true);
    }
}
