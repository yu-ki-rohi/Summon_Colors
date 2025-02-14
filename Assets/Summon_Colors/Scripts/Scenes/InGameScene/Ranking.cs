using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Ranking
{
    string[] ranking = new string[6] { "first", "second", "third", "forth", "fifth", "sixth" };
    int[] rankingValue = new int[6] { 0, 0, 0, 0, 0, 0};

    public int[] RankingValue { get { return rankingValue; } }

    // ランキング呼び出し
    public void GetRanking()
    {
        // ランキング呼び出し
        for (int i = 0; i < ranking.Length; i++) 
        {
            rankingValue[i] = PlayerPrefs.GetInt(ranking[i]);
        }
    }

    // ランキング書き込み
    public void SetRanking(int _value)
    {
        // 書き込み用
        for(int i = 0;i < ranking.Length;i++)
        {
            // 取得した値とRankingの値を比較して入れ替え
            if (_value > rankingValue[i])
            {
                var change = rankingValue[i];
                rankingValue[i] = _value;
                _value = change;
            }
        }
        
        // 入れ替えた値を保存
        for(int i = 0; i < ranking.Length; i++)
        {
            PlayerPrefs.SetInt(ranking[i], rankingValue[i]);
        }
    }

    public int GetScore(int i)
    {
        return rankingValue[i];
    }
}
