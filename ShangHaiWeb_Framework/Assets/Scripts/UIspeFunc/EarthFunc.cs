using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EarthFunc : MonoBehaviour
{

    Transform _1, _2, _3, earth;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnEnable()
    {
        if (_1 == null)
        {
            _1 = transform.Find("1");
            _2 = transform.Find("2");
            _3 = transform.Find("3");
            earth= transform.Find("earth");
        }

        _1.localScale = Vector3.one*2;
        _2.localScale = Vector3.one*2;
        _3.localScale = Vector3.one*2;
        earth.localScale = Vector3.zero;

        _3.DOScale(1, 0.2f).SetId("UIani");
        _2.DOScale(1, 0.4f).SetId("UIani");
        _1.DOScale(1, 0.6f).OnComplete(()=> 
        {
            earth.DOScale(1, 0.5f).SetId("UIani");
        }).SetId("UIani");
      
    }
    private void OnDisable()
    {
    
    }
}
