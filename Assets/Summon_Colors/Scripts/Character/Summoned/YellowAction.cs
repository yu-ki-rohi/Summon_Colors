using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowAction : RedAction
{
    [SerializeField] private GameObject _projectile;
    private ObjectPoolBase _pool;

    public void SetPool(ObjectPoolBase pool)
    { 
        _pool = pool;
    }


    protected override void Action()
    {
        if (_pool == null)
        {
            Debug.Log("Pool is Null!");
            if(_projectile != null)
            {
                GameObject projectile = Instantiate(_projectile, transform.position, Quaternion.identity);
                projectile.transform.forward = _summonedBase.TargetCharacter.transform.position - transform.position;
                if (projectile.TryGetComponent<SummonedProjectiles>(out var projectiles))
                {
                    projectiles.Initialize(_summonedBase, _pool);
                }
            }
            return;
        }
        else
        {
            GameObject projectile = _pool.Get(transform.position);
            projectile.transform.forward = _summonedBase.TargetCharacter.transform.position - transform.position;
            if (projectile.TryGetComponent<SummonedProjectiles>(out var projectiles))
            {
                projectiles.Initialize(_summonedBase, _pool);
            }
        }
    }
}
