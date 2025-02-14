using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class FadePanel
{

    public Image ScreenPanel;
    private float _red;
    private float _green;
    private float _blue;
    private float _alpha;

    public float Red { get { return _red; } }
    public float Green { get { return _green; } }
    public float Blue { get { return _blue; } }
    public float Alpha { get { return _alpha; } }

    public void ChangeAlpha(float alpha)
    {
        _alpha = Mathf.Clamp01(alpha);
        ScreenPanel.color = new Color(_red, _green, _blue, _alpha);
    }

    public void ChangeColor(float red , float green, float blue)
    {
        _red = Mathf.Clamp01(red);
        _green = Mathf.Clamp01(green);
        _blue = Mathf.Clamp01(blue);
        ScreenPanel.color = new Color(_red, _green, _blue, _alpha);
    }
}
