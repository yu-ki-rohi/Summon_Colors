using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireBall : Projectiles
{
    [SerializeField] private GameObject _flames;
    [SerializeField] private GameObject _fire;
    private ObjectPoolBase _embersPool;
    private int _embersPower = 3;
    private Elements _elements;

    public void Initialize(int power, int embersPower, ObjectPoolBase embersPool)
    {
        Initialize(power);
        _embersPower = embersPower;
        _embersPool = embersPool;
        if(_elements == null)
        {
            _elements = GetComponent<Elements>();
        }
        else
        {
            _elements.Initialize();
        }
    }

    protected override void BehaviorOnHitStage(Collider stage)
    {
        if (_embersPool != null)
        {
            CreateEmbers();
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
    protected override void Update()
    {
        base.Update();
        CheckElementRemaining();
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

                if (damage > 0)
                {
                    float time = 18.0f;
                    float forcePower = 500.0f;
                    float powerMagni = Mathf.Clamp01((damage + 40.0f) / 100.0f);
                    Vector3 forceVec = (other.transform.position - transform.position);
                    characterBase.KnockBack(forceVec, forcePower * powerMagni, time * powerMagni);
                    HitEffectManager.Instance.Play(HitEffectManager.Type.Fire, other.ClosestPointOnBounds(transform.position));
                }
            }
        }
    }

    private void CheckElementRemaining()
    {
        if (_elements == null) return;
        if (_elements.IsColorRemaining()) return;
        DisAppear();
    }

    private void CreateEmbers()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        RaycastHit hit;
        int layerNum = LayerMask.NameToLayer("Stage");
        int layerMask = 1 << layerNum;
        layerNum = LayerMask.NameToLayer("Ground");
        layerMask |= 1 << layerNum;
        float distance = 1.5f;

        if (!Physics.Raycast(ray, out hit, distance, layerMask)) return;
        GameObject ember = _embersPool.Get(hit.point);
        if (ember.TryGetComponent<Embers>(out var embers))
        {
            embers.Initialize(_embersPower);
        }
    }
}
