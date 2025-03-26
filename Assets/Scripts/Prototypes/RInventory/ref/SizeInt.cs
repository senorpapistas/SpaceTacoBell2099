using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SizeInt
{
    public int width;
    public int height;

    public SizeInt(int w, int h)
    {
        width = w;
        height = h;
    }
}
