using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class FireBall : Projectiles
{
    [SerializeField] private GameObject _flames;
    [SerializeField] private GameObject _fire;
    private ObjectPoolBase _embersPool;
    private int _embersPower = 3;

    public void Initialize(int power, int embersPower, ObjectPoolBase embersPool)
    {
        Initialize(power);
        _embersPower = embersPower;
        _embersPool = embersPool;
    }

    protected override void BehaviorOnHitStage(Collider stage)
    {
        if (_embersPool != null)
        {
            GameObject ember = _embersPool.Get(stage.ClosestPointOnBounds(this.transform.position));
            if (ember.TryGetComponent<Embers>(out var embers))
            {
                embers.Initialize(_embersPower);
            }
        }
        else
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
        
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.tag == "Player" || other.tag == "Summoned")
        {
            CharacterBase characterBase = other.GetComponentInParent<CharacterBase>();
            if (characterBase != null)
            {
                int damage = characterBase.Damaged(_power);

                if(damage > 0)
                {
                    float time = 0.15f;
                    float forcePower = 25.0f;
                    float powerMagni = Mathf.Clamp01(damage / 100.0f);
                    Vector3 forceVec = (other.transform.position - transform.position);
                    characterBase.KnockBack(forceVec, forcePower * powerMagni, time);
                    HitEffectManager.Instance.Play(HitEffectManager.Type.Fire, other.ClosestPointOnBounds(transform.position));
                }
            }
        }
    }
}
