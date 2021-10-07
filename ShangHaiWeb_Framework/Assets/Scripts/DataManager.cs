using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DataManager
{
    public static string s01, s03, s04, s05, s06;

    public static string GetStr(StrType type)
    {
        string str = s01;
        switch (type)
        {
            case StrType._s04:
                str = str + "/" + s03 + "/" + s04;
                break;
            case StrType._s05:
                str = str + "/" + s03 + "/" + s05;
                break;
            case StrType._s06:
                str = str + "/"  + s06;
                break;
        }
        Debug.Log("DataManager str:"+ str);
        return str;
    }

}
public enum StrType
{
    _s04,
    _s05,
    _s06,
}
