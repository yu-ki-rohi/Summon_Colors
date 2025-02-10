using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletPool : SummonedPool
{
    [SerializeField] private ExplosionPool explosionPool;

    protected override GameObject OnCreatePoolObject()
    {
        GameObject obj = base.OnCreatePoolObject();
        if (obj.TryGetComponent<VioletAction>(out var violetAction))
        {
            violetAction.RegisterPool(explosionPool);
        }
        return obj;
    }
}
