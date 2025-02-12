using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private int _power = 20;
    private Collider _collider;
    private Timer _activeTimer;
    private Timer _explosionTimer;
    private ObjectPoolBase _pool;
    private CharacterBase _attacker;
    private List<CharacterBase> _characters = new List<CharacterBase>();

    public void Initialize(int power)
    {
        _power = power;
        _activeTimer = new Timer(DisAppear, 2.5f);
        _explosionTimer = new Timer(FinishExplosion, 0.05f);
        _collider = GetComponent<Collider>();
        _collider.enabled = true;
    }

    public void Initialize(int power, CharacterBase attacker)
    {
        Initialize(power);
        _attacker = attacker;
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
        if(_explosionTimer != null)
        {
            _explosionTimer.CountUp(Time.deltaTime);
        }
        else if(_activeTimer != null)
        {
            _activeTimer.CountUp(Time.deltaTime);
        }
    }

    private void DisAppear()
    {
        _characters.Clear();
        if(_pool != null)
        {
            _pool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FinishExplosion()
    {
        _collider.enabled = false;
        _explosionTimer = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.tag == "Enemy")
        {
            if (other.tag == "Player" || other.tag == "Summoned")
            {
                int damage = HitOther(other);
                if (damage > 0)
                {
                    HitEffectManager.Instance.Play(HitEffectManager.Type.Fire, other.ClosestPointOnBounds(transform.position));
                }
            }
        }
        else if (gameObject.tag == "Player" || gameObject.tag == "Summoned")
        {
            if((other.tag == "Enemy"))
            {
                HitOther(other);
            }
        }
    }
    private int HitOther(Collider other)
    {
        Vector3 StartPos = gameObject.transform.position;
        StartPos.y += 0.8f;
        Vector3 EndPos = other.gameObject.transform.position;
        EndPos.y += 0.8f;
        Ray ray = new Ray(StartPos, EndPos - StartPos);
        RaycastHit hit;
        int layerNum = LayerMask.NameToLayer("Stage");
        int layerMask = 1 << layerNum;
        layerNum = LayerMask.NameToLayer("Ground");
        layerMask |= 1 << layerNum;

        float distance = (EndPos - StartPos).magnitude;

        if (Physics.Raycast(ray, out hit, distance, layerMask))
        {
            return 0;
        }
        CharacterBase characterBase = other.GetComponentInParent<CharacterBase>();
        if (characterBase != null)
        {
            if (HasAttacked(characterBase))
            {
                return 0;
            }
            _characters.Add(characterBase);
            int damage;
            if(_attacker == null)
            {
                damage = characterBase.Damaged(_power);
            }
            else
            {
                damage = characterBase.Damaged(_power, _attacker.Break, _attacker.Appearance, _attacker);
            }

            if (damage > 0)
            {
                if (distance == 0) { return damage; }
                StartPos.y = 0.0f;
                EndPos.y = 0.0f;

                float time = 0.3f;
                float forcePower = 25.0f;
                float powerMagni = Mathf.Clamp01(damage / 100.0f);
                Vector3 forceVec = (EndPos - StartPos) / distance;
                characterBase.KnockBack(forceVec, forcePower * powerMagni, time);
            }
            return damage;
        }
        return 0;
    }
    private bool HasAttacked(CharacterBase characterBase)
    {
        foreach (var character in _characters)
        {
            if (character == characterBase)
            {
                return true;
            }
        }
        return false;
    }
}
