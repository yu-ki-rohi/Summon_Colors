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
        float baseRange = 8.0f;
        float baseSpeed = 10.0f;
        float weight = Random.Range(0.3f, 1.2f);
        Initialize(0, baseRange * weight, baseSpeed * weight, false);
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
        else
        {
            Debug.Log("Elements is not found");
        }
    }

    protected override void BehaviorOnHitSubStage(Collider stage)
    {
        BehaviorOnHitStage(stage);
        DisAppear();
    }
}
