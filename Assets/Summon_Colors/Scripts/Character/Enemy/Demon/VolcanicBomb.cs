using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicBomb : Projectiles
{
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _flames;

    private int _explosionPower;

    public void Initialize(int power, int explosionPower)
    {
        _explosionPower = explosionPower;
        Initialize(power);
    }

    protected override void BehaviorOnHitStage(Collider stage)
    {
        if(_explosion != null)
        {
            GameObject explosionObj = Instantiate(_explosion,transform.position,Quaternion.identity);
            if (explosionObj.TryGetComponent<Explosion>(out var explosion))
            {
                explosion.Initialize(_explosionPower);
            }
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
