using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ProgressImage : MonoBehaviour
{


    public Texture[] allTexture;

    public Image img;
    int length;
    void Start()
    {
        img = GetComponent<Image>();
        allTexture = Resources.LoadAll<Texture>("切图-箭头动画");
        length = allTexture.Length;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ShowImage(float  progress)
    {
        int index = (int)(length * progress + 0.5f)-1;

        Texture2D t2d = allTexture[index] as Texture2D;
        img.sprite = Sprite.Create(t2d, new Rect(0,0, t2d.width, t2d.height),Vector2.zero) ;
    }
}
