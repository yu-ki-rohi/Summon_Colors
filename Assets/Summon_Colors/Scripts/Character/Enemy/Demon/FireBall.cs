using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Projectiles
{
    [SerializeField] private GameObject _flames;
    [SerializeField] private GameObject _fire;

    protected override void BehaviorOnHitStage(Collider stage)
    {
        if (_fire != null)
        {
            Instantiate(_fire, transform.position, Quaternion.identity);
        }
        if (_flames != null)
        {
            Instantiate(_flames, stage.ClosestPointOnBounds(this.transform.position), Quaternion.identity);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.tag == "Player" || other.tag == "Summoned")
        {
            CharacterBase characterBase = other.GetComponentInParent<CharacterBase>();
            if (characterBase != null)
            {
                characterBase.Damaged(_power);
            }
        }
    }
}
