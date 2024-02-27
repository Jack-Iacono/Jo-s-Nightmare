using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class MyFunctions
{
    public static float Distance(Vector3 orig, Vector3 comp)
    {
        Vector3 difference = new Vector3
            (
                comp.x - orig.x,
                comp.y - orig.y,
                comp.z - orig.z
            );

        return Mathf.Sqrt
            (
            difference.x * difference.x +
            difference.y * difference.y +
            difference.z * difference.z
            );
    }
    public static bool LayermaskContains(LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | 1 << layer);
    }
    public static string ConvertToTime(float time)
    {
        string finalString = "";

        int min = Mathf.FloorToInt(time / 60);

        if (min == 0)
            finalString += "00:";
        else if (min < 10)
            finalString += "0" + min + ":";
        else
            finalString += min + ":";

        int sec = Mathf.FloorToInt(time % 60);

        if (sec == 0)
            finalString += "00";
        else if (sec < 10)
            finalString += "0" + sec;
        else
            finalString += sec;

        return finalString;
    }
}
