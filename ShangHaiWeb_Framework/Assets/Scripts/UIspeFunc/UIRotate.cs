using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIRotate : MonoBehaviour {

    Tweener t;

    private Vector3 dir;

    [SerializeField]
    private float speed=10;
    [SerializeField]
    private RotateDir rd;
	void Start ()
    {
        switch (rd)
        {
            case RotateDir.x:
                dir = transform.right;
                break;
            case RotateDir.y:
                dir = transform.up;
                break;
            case RotateDir.z:
                dir = transform.forward;
                break;
        }


    }
    private void OnEnable()
    {
       // t = transform.DOLocalRotate();
    }
    private void OnDisable()
    {
        
    }
    // Update is called once per frame
    void Update ()
    {
        transform.Rotate(dir,Time.deltaTime*speed,Space.World);
	}

    public enum RotateDir
    {
        x,
        y,
        z,
        
    }
}
