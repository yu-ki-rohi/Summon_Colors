using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Elements))]
public class Embers : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _embers;
    [SerializeField] private float _activeTime = 5.0f;
    private int _power;
    private Elements _elements;
    private ObjectPoolBase _pool;
    private Timer _activeTimer;
    private Timer _eraseTimer;
    private bool _isDamagable;

    public void Initialize(int power)
    {
        _power = power;
    }

    public void Initialize()
    {
        if (_elements == null)
        {
            _elements = GetComponent<Elements>();
        }
        _elements.Initialize();
        _activeTimer = new Timer(Finish, _activeTime);
        _eraseTimer = null;
        StartCoroutine("HandleDamagable");
        foreach (var ember in _embers)
        {
            ember.Play();
        }
    }
    public void RegisterPool(ObjectPoolBase pool)
    {
        _pool = pool;
    }
    
    

    // Start is called before the first frame update
    void Start()
    {
        if (_elements == null)
        {
            Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_eraseTimer != null)
        {
            _eraseTimer.CountUp(Time.deltaTime);
        }
        else if(_elements != null &&
            !_elements.IsColorRemaining())
        {
            Finish();
            Disappear();
        }
        else if(_activeTimer != null)
        {
            _activeTimer.CountUp(Time.deltaTime);
        }
    }

    private void Finish()
    {
        _eraseTimer = new Timer(Disappear, 3.0f);
        _activeTimer = null;
        StopCoroutine("HandleDamagable");
        _isDamagable = false;
        foreach (var ember in _embers)
        {
            ember.Stop();
        }
    }

    private void Disappear()
    {
        _activeTimer = null;
        _eraseTimer = null;

        if (_pool != null)
        {
            _pool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(!_isDamagable) { return; }
        if (other.tag == "Player" || other.tag == "Summoned")
        {
            CharacterBase characterBase = other.GetComponentInParent<CharacterBase>();
            if (characterBase != null)
            {
                int damage = characterBase.Damaged(_power);
                if (damage != 0)
                {
                    HitEffectManager.Instance.Play(HitEffectManager.Type.Fire, other.ClosestPointOnBounds(transform.position));
                }
            }
        }
    }

    private IEnumerator HandleDamagable()
    {
        while(true)
        {
            _isDamagable = true;
            yield return new WaitForSeconds(0.1f);
            _isDamagable = false;
            yield return new WaitForSeconds(0.4f);
        }
    }
}
