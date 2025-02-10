using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] protected int _power = 10;
    [SerializeField] private float _range = 10.0f;
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private bool _useRigidBody = false;

    private ObjectPoolBase _pool;
    private float _distance = 0.0f;
    
    public void RegisterPool(ObjectPoolBase pool)
    { 
        _pool = pool;
    }

    public void Initialize()
    {
        _distance = 0.0f;
    }

    public void Initialize(int power)
    {
        _power = power;
        _distance = 0.0f;
    }

    public void Initialize(int power, float range, float speed, bool useRigidBody)
    {
        _power = power;
        _range = range;
        _speed = speed;
        _useRigidBody = useRigidBody;
        _distance = 0.0f;
    }



    protected virtual void DisAppear()
    {
        if(_pool != null)
        {
            _pool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    protected virtual void BehaviorOnHitStage(Collider stage)
    {

    }
    protected virtual void BehaviorOnHitSubStage(Collider stage)
    {

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Element")
        {
            BehaviorOnHitStage(other);
            DisAppear();
        }
        if (other.tag == "SubElement")
        {
            BehaviorOnHitSubStage(other);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!_useRigidBody)
        {
            if(_distance < _range)
            {
                _distance += _speed * Time.deltaTime;
                transform.position += transform.forward * _speed * Time.deltaTime;
            }
            else
            {
                DisAppear();
            }
        }
    }
}
