using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABTest : MonoBehaviour {

	
	 void Start () {

		LoadAssetManager.Instance.LoadAssetBundle(LoadType.Single, "s01", "prefab", "Cube", (tempObj) =>
		{
			GameObject prefabs = Instantiate<GameObject>((GameObject)tempObj[0], gameObject.transform, false);
		}, (tempObj) => {
			Debug.Log("正在加载");
		});
	

	}
	
	
	void Update () {

		if (Input.GetMouseButtonDown(0))
		{
			Debug.Log("卸载");
			LoadAssetManager.Instance.ReleaseSingleObj("s01","prefab","Cube");
		}

		
	}
}
