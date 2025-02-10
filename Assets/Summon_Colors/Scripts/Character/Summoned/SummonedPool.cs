using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedPool : ObjectPoolBase
{
    [SerializeField] private HomeBase _homeBase;
    [SerializeField] private ColorElements.ColorType _colorType;
    [SerializeField] private ObjectPoolBase _projectilePool;
    private SummonedBase _summonedBase;

    private int _summonedNum = 0;
    private int _deadNum = 0;

    public int SummonedNum { get { return _summonedNum; } }
    public int DeadNum { get { return _deadNum; } }


    public ColorElements.ColorType ColorType { get { return _colorType; } }

    public int GetActiveNum()
    {
        return _summonedNum - _deadNum;
    }

    public int GetCosts(int rank = 0)
    {
        if(_summonedBase != null)
        {
            return _summonedBase.GetCosts(rank);
        }
        Debug.Log("Summoned Base is not found");
        return 10000;
    }

    public override GameObject Get(Vector3 position)
    {
        _summonedNum++;
        return base.Get(position);
    }

    public override void Release(GameObject obj)
    {
        _deadNum++;
        base.Release(obj);
    }

    protected override GameObject OnCreatePoolObject()
    {
        GameObject o = Instantiate(_prefab, this.transform);
        if(o.TryGetComponent<SummonedBase>(out var summonedBase))
        {
            summonedBase.Home = _homeBase;
            summonedBase.SummonedPool = this;
            if(_projectilePool != null)
            {
                YellowAction yellowAction = o.GetComponent<YellowAction>();
                if(yellowAction != null)
                {
                    Debug.Log("Set Projectile Pool");
                    yellowAction.SetPool(_projectilePool);
                }
            }
        }
        return o;
    }

    protected override void Start()
    {
        if (_prefab.TryGetComponent<SummonedBase>(out var summonedBase))
        {
            _summonedBase = summonedBase;
        }
        base.Start();
    }
}
