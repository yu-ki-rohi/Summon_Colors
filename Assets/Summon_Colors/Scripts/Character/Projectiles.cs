using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] protected int _power = 10;
    [SerializeField] private float _range = 10.0f;
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private bool _useRigidBody = false;

    private float _distance = 0.0f;

    protected virtual void DisAppear()
    {
        Destroy(gameObject);
    }
    protected virtual void BehaviorOnHitStage(Collider stage)
    {

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Element")
        {
            BehaviorOnHitStage(other);
            DisAppear();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
