using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedProjectiles : MonoBehaviour
{
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private float _range = 10.0f;
    private CharacterBase _characterBase;
    private ObjectPoolBase _pool;
    private float _distance = 0.0f;

    public void Initialize(CharacterBase characterBase, ObjectPoolBase pool)
    {
        _characterBase = characterBase;
        _pool = pool;
        _distance = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _distance += _speed * Time.deltaTime;
        transform.position += transform.forward * _speed * Time.deltaTime;
        if (_distance > _range)
        {
            _distance = 0.0f;
            if(_pool != null)
            {
                _pool.Release(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            CharacterBase character = other.GetComponentInParent<CharacterBase>();
            if(character != null)
            {
                character.Damaged(_characterBase.Attack, _characterBase.Break, _characterBase.Attack, _characterBase);
                if (_pool != null)
                {
                    _pool.Release(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
