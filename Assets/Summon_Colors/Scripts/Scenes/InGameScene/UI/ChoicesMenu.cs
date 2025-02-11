using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ChoicesMenu
{
    public Image[] Fonts;
    public Image[] Cursors;

    public void ChoiceCursor(int index)
    {
        foreach (var c in Cursors)
        {
            c.enabled = false;
        }
        if (index < 0 || index >= Cursors.Length) { return; }
        Cursors[index].enabled = true;
    }

    public void View(bool flag, int index = 0)
    {
        foreach (var f in Fonts)
        {
            f.enabled = flag;
        }
        if(!flag) { index = -1; }
        ChoiceCursor(index);
    }
}
