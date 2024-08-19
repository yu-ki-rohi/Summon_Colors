using System;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// 保持している色の量
/// </summary>
[Serializable]
public class ColorElements
{
    public enum ColorType
    {
        Blue,
        Red,
        Yellow,
        Orange,
        Green,
        Violet,
        All
    }

    public int Blue = 0;
    public int Red = 0;
    public int Yellow = 0;

    private int _blue = 0;
    private int _red = 0;
    private int _yellow = 0;

    /// <summary>
    /// ColorTypeの項目数を取得
    /// </summary>
    /// <returns>ColorTypeの項目数</returns>
    static public int GetColorTypeNum()
    {
        return Enum.GetNames(typeof(ColorType)).Length;
    }

    /// <summary>
    /// 初期化メソッド
    /// </summary>
    public void Initialize()
    {
        _blue = Blue;
        _red = Red;
        _yellow = Yellow;
    }

    public int GetColorMaxSum()
    {
        return Blue + Red + Yellow;
    }

    public int GetColorSum()
    {
        return _blue + _red + _yellow;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public float GetRemaining(ColorType color)
    {
        switch (color)
        {
            case ColorType.Blue:
                return GetRemainingColor(_blue, Blue);
            case ColorType.Red:
                return GetRemainingColor(_red, Red);
            case ColorType.Yellow:
                return GetRemainingColor(_yellow, Yellow);
            case ColorType.Orange:
                return GetRemainingColor(_red + _yellow, Red + Yellow);
            case ColorType.Green:
                return GetRemainingColor(_blue + _yellow, Blue + Yellow);
            case ColorType.Violet:
                return GetRemainingColor(_red + _blue, Red + Blue);
            default:
                return GetRemainingColor(_blue + _red + _yellow, GetColorMaxSum());
        }
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
            case ColorType.Blue:
                AddColor(ref _blue, Blue, value);
                break;
            case ColorType.Red:
                AddColor(ref _red, Red, value);
                break;
            case ColorType.Yellow:
                AddColor(ref _yellow, Yellow, value);
                break;
            case ColorType.Orange:
                AddColor(ref _red, Red, value);
                AddColor(ref _yellow, Yellow, value);
                break;
            case ColorType.Green:
                AddColor(ref _blue, Blue, value);
                AddColor(ref _yellow, Yellow, value);
                break;
            case ColorType.Violet:
                AddColor(ref _blue, Blue, value);
                AddColor(ref _red, Red, value);
                break;
            default:
                AddColor(ref _blue, Blue, value);
                AddColor(ref _red, Red, value);
                AddColor(ref _yellow, Yellow, value);
                break;
        }
    }

    /// <summary>
    /// 色を減少させるメソッド
    /// </summary>
    /// <param name="color">減少させる色</param>
    /// <param name="value">減少させる量</param>
    /// <param name="exhaust">足りない際使い切るか</param>
    /// <returns>どれだけ減らしたか</returns>
    public int Reduce(ColorType color, int value, bool exhaust = true)
    {
        switch (color)
        {
            case ColorType.Blue:
                if(!exhaust &&
                    _blue < value)
                {
                    return 0;
                }
                return ReduceColor(ref _blue, value);
            case ColorType.Red:
                if (!exhaust &&
                    _red < value)
                {
                    return 0;
                }
                return ReduceColor(ref _red, value);
            case ColorType.Yellow:
                if (!exhaust &&
                    _yellow < value)
                {
                    return 0;
                }
                return ReduceColor(ref _yellow, value);
            case ColorType.Orange:
                if (!exhaust &&
                    (_red < value ||
                    _yellow < value))
                {
                    return 0;
                }
                return ReduceColor(ref _red, value) + ReduceColor(ref _yellow, value);
            case ColorType.Green:
                if (!exhaust &&
                   (_blue < value ||
                    _yellow < value))
                {
                    return 0;
                }
                return ReduceColor(ref _blue, value) + ReduceColor(ref _yellow, value);
            case ColorType.Violet:
                if (!exhaust &&
                    (_red < value ||
                    _blue < value))
                {
                    return 0;
                }
                return ReduceColor(ref _blue, value) + ReduceColor(ref _red, value);
            default:
                if (!exhaust &&
                    (_blue < value ||
                    _red < value ||
                    _yellow < value))
                {
                    return 0;
                }
                return ReduceColor(ref _blue, value) + ReduceColor(ref _red, value) + ReduceColor(ref _yellow, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="current"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private float GetRemainingColor(int current, int max)
    {
        if (max == 0)
        {
            return 0.0f;
        }

        return  ((float)current / max);
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
