using System;

/// <summary>
/// 保持している色の量
/// </summary>
[Serializable]
public class ColorElements
{
    public enum ColorType
    {
        Cyan,
        Magenta,
        Yellow
    }

    public int Cyan = 0;
    public int Magenta = 0;
    public int Yellow = 0;

    private int _cyan;
    private int _magenta;
    private int _yellow;

    /// <summary>
    /// 初期化メソッド
    /// </summary>
    public void Initialize()
    {
        _cyan = Cyan;
        _magenta = Magenta;
        _yellow = Yellow;
    }

    /// <summary>
    /// 色を増加させるメソッド
    /// </summary>
    /// <param name="color">増加させる色</param>
    /// <param name="value">増加させる量</param>
    public void Add(ColorType color, int value)
    {
        switch (color)
        {
            case ColorType.Cyan:
                AddColor(ref _cyan, Cyan, value);
                break;
            case ColorType.Magenta:
                AddColor(ref _magenta, Magenta, value);
                break;
            case ColorType.Yellow:
                AddColor(ref _yellow, Yellow, value);
                break;
        }
    }

    /// <summary>
    /// 色を減少させるメソッド
    /// </summary>
    /// <param name="color">減少させる色</param>
    /// <param name="value">減少させる量</param>
    /// <returns>どれだけ減らしたか</returns>
    public int Reduce(ColorType color,int value)
    {
        switch (color)
        {
            case ColorType.Cyan:
                return ReduceColor(ref _cyan, value);
            case ColorType.Magenta:
                return ReduceColor(ref _magenta, value);
            case ColorType.Yellow:
                return ReduceColor(ref _yellow, value);
            default:
                return 0;
        }
    }

    /// <summary>
    /// 色増加の内部メソッド、
    /// 上限より多くはさせない
    /// </summary>
    /// <param name="color">増加させる色</param>
    /// <param name="colorMax">上限</param>
    /// <param name="value">増加させる量</param>
    private void AddColor(ref int color, int colorMax, int value)
    {
        if(color + value > colorMax)
        {
            color = colorMax;
        }
        else
        {
            color += value;
        }
    }

    /// <summary>
    /// 色減少の内部メソッド、
    /// 0未満にはさせない
    /// </summary>
    /// <param name="color">減少させる色</param>
    /// <param name="value">減少させる量</param>
    /// <returns>どれだけ減らしたか</returns>
    private int ReduceColor(ref int color, int value)
    {
        if(color < value)
        {
            value = color;
        }
        color -= value;
        return value;
    }
}
