using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesPool : ObjectPoolBase
{
    protected override GameObject OnCreatePoolObject()
    {
        GameObject projectile = base.OnCreatePoolObject();
        if(projectile.TryGetComponent<Projectiles>(out var projectiles))
        {
            projectiles.RegisterPool(this);
        }
        return projectile;
    }

    protected override void OnTakeFromPool(GameObject target)
    {
        base.OnTakeFromPool(target);
        if (target.TryGetComponent<Projectiles>(out var projectiles))
        {
            projectiles.Initialize();
        }
    }
}
