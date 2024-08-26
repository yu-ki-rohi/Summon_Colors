using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedPool : ObjectPoolBase
{
    [SerializeField] private HomeBase _homeBase;
    [SerializeField] private ColorElements.ColorType _colorType;
    [SerializeField] private ObjectPoolBase _projectilePool;
    private SummonedBase _summonedBase;

    public ColorElements.ColorType ColorType { get { return _colorType; } }

    public int GetCosts()
    {
        if(_summonedBase != null)
        {
            return _summonedBase.Costs;
        }
        Debug.Log("Summoned Base is not found");
        return 10000;
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
                YellowAction yellowAction = _prefab.GetComponent<YellowAction>();
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
