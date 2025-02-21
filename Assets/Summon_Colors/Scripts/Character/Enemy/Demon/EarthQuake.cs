using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EarthQuake : MonoBehaviour
{
    private int _power = 20;
    private Timer _activeTimer;
    private ObjectPoolBase _pool;

    public void Initialize(int power)
    {
        _power = power;
        _activeTimer = new Timer(DisAppear, 4.5f);
    }

    public void RegisterPool(ObjectPoolBase pool)
    {
        _pool = pool;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_activeTimer != null)
        {
            _activeTimer.CountUp(Time.deltaTime);
        }
    }

    private void DisAppear()
    {
        if (_pool != null)
        {
            _pool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Summoned")
        {
            int damage = HitOther(other);
            if (damage != 0)
            {
                HitEffectManager.Instance.Play(HitEffectManager.Type.Hit, other.ClosestPointOnBounds(transform.position));
            }
        }
    }
    private int HitOther(Collider other)
    {
        Vector3 StartPos = gameObject.transform.position;
        Vector3 EndPos = other.gameObject.transform.position;

        float distance = (EndPos - StartPos).magnitude;

        CharacterBase characterBase = other.GetComponentInParent<CharacterBase>();
        if (characterBase != null)
        {
            int damage = characterBase.Damaged(_power);
            if (damage > 0)
            {
                StartPos.y = 0.0f;
                EndPos.y = 0.0f;

                float time = 12.0f;
                float forcePower = 400.0f;
                Vector3 forceVec = (EndPos - StartPos) / distance;
                characterBase.KnockBack(forceVec, forcePower, time);
            }

            return damage;
        }
        return 0;
    }

}
