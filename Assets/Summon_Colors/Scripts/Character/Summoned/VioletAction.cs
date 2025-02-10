using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletAction : RedAction
{
    [SerializeField] private Transform _explodePosition;
    private ExplosionPool _explosionPool;

    public void RegisterPool(ExplosionPool explosionPool)
    {
        _explosionPool = explosionPool;
    }

    public void CreateExplosion()
    {
        if(_explosionPool == null ||
            _explodePosition == null) { return; }

        _explosionPool.Get(_explodePosition.position, _summonedBase.Attack, _summonedBase);
    }
}
