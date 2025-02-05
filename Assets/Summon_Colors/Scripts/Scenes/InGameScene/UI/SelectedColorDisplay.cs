using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SelectedColorDisplay
{
    public Sprite[] Colors;
    public Image Display;

    public void ChangeColor(ColorElements.ColorType colorType)
    {
        Display.sprite = Colors[(int)colorType];
    }
}
