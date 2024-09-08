using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicBomb : Projectiles
{
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _flames;

    protected override void BehaviorOnHitStage(Collider stage)
    {
        if(_explosion != null)
        {
            Instantiate(_explosion,transform.position,Quaternion.identity);
        }
        if(_flames != null)
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
            if(characterBase != null)
            {
                characterBase.Damaged(_power);
            }
        }
    }
}
