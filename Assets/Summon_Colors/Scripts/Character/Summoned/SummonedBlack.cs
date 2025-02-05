using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedBlack : SummonedBase
{
    [SerializeField] private SummonedBlackData _data;

    public override int GetCosts(int rank)
    {
        if(rank < 0 || rank >= _data.RankCosts.Length) { return -1; }
        return _data.RankCosts[rank];
    }
    public void Initialize(int rank, int id, Transform standByPosition, Summon summon, Vector3 summonedPosition)
    {
        _data.Initialize(rank);
        _summonedData = _data;
        Initialize(id, standByPosition, summon, summonedPosition);
        SummonedAction summonedAction = GetComponent<SummonedAction>();
        if (summonedAction != null)
        {
            summonedAction.Initialize();
        }
    }
}
