using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbBullet : Projectiles
{
    private Absorb _absorb;

    public void Initialize(Absorb absorb, Vector3 forward)
    {
        _absorb = absorb;
        transform.forward = forward;
    }

    protected override void BehaviorOnHitStage(Collider stage)
    {
        Elements element = stage.GetComponentInParent<Elements>();
        if (element != null)
        {

            element.RegisterAbsorb(_absorb, stage.ClosestPointOnBounds(this.transform.position));
#if true
            ColorElements.ColorType colorType = (ColorElements.ColorType)Random.Range(0, 3);
            element.ExtractEnergy(colorType);
#endif
        }
    }
}
