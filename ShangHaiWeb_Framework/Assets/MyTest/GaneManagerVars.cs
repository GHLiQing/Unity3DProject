using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(menuName ="Creat/Vars")]
public class GaneManagerVars :  ScriptableObject{

	public static GaneManagerVars Creat()
	{
		return Resources.Load<GaneManagerVars>("GaneManagerVars");
	}
	public GameObject go;
}
