using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterData/SummonedBlack", fileName = "SummonedBlackData")]
public class SummonedBlackData : SummonedData
{
    public int[] RankCosts;
    public int[] RankMaxHp;
    public int[] RankAttack;
    public int[] RankVitality;
    public int[] RankBreak;
    public int[] RankAppearnce;
    public float[] RankAgility;
    public float[] RankCoolTime;

    public void Initialize(int rank)
    {
        if(rank < 0 || rank >= RankCosts.Length) { return; }
        MaxHp = RankMaxHp[rank];
        Attack = RankAttack[rank];
        Vitality = RankVitality[rank];
        Break = RankBreak[rank];
        Appearance = RankAppearnce[rank];
        Agility = RankAgility[rank];
        CoolTime = RankCoolTime[rank];
    }
}
