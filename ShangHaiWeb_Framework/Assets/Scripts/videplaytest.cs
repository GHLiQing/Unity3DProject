using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class videplaytest : MonoBehaviour {

   public VideoComponent_URL vc_u;
    public InputField input_field;
	void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void PlayVideo()
    {
        vc_u.OnPlayVideo(input_field.text);
    }
}
