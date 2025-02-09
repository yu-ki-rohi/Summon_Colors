using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmbersPool : ObjectPoolBase
{
    protected override GameObject OnCreatePoolObject()
    {
        GameObject ember = base.OnCreatePoolObject();
        if (ember.TryGetComponent<Embers>(out var embers))
        {
            embers.RegisterPool(this);
        }
        return ember;
    }

    protected override void OnTakeFromPool(GameObject target)
    {
        base.OnTakeFromPool(target);
        if (target.TryGetComponent<Embers>(out var embers))
        {
            embers.Initialize();
        }
    }
}
