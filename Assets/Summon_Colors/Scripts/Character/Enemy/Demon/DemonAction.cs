using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonAction : EnemyAction
{
    [SerializeField] private Transform _breathPosition;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private Collider[] _rushColliders;
    [SerializeField] private Collider _handAttackCollider;
    [SerializeField] private Collider[] _tailColliders;

    [SerializeField] private GameObject _volcanicBomb;
    [SerializeField] private GameObject _earthQuake;
    [SerializeField] private GameObject _fireEffect;
    [SerializeField] private GameObject _fireBall;

    [SerializeField] private float _firePower = 10.0f;
    [SerializeField] private float _rushSpeed = 2.0f;

    private Vector3 _rushVector = Vector3.zero;
    private bool _isRush = false;


    public void CreateVolcanicBomb()
    {
        GameObject volcanicBomb = Instantiate(_volcanicBomb, _firePosition.position, Quaternion.identity);
        volcanicBomb.transform.forward = _firePosition.forward;
        Rigidbody rigidbody = volcanicBomb.GetComponent<Rigidbody>();
        rigidbody.AddForce(_firePosition.forward * _firePower, ForceMode.Impulse);
    }

    public void CreateEarthQuake()
    {
        GameObject earthQuake = Instantiate(_earthQuake, _handAttackCollider.transform.position + Vector3.down * 1.0f, Quaternion.identity);
        earthQuake.transform.forward = transform.forward;
    }
    public void CreateFireBall()
    {
        GameObject fireBall = Instantiate(_fireBall, _breathPosition.position, Quaternion.identity);
        fireBall.transform.forward = _breathPosition.forward;
    }
    public void StartBreath()
    {
        _fireEffect.SetActive(true);
    }

    public void StartRush()
    {
        _agent.speed = _enemyBase.Agility * _rushSpeed;
        _isRush = true;
    }

    private void FinishRush()
    {
        _agent.speed = _enemyBase.Agility;
        _isRush = false;
        _animator.SetTrigger("Rush");
    }

    public override void FinishAttack()
    {
        foreach (var collider in _rushColliders)
        {
            collider.enabled = false;
        }
        _handAttackCollider.enabled = false;
        foreach (var collider in _tailColliders)
        {
            collider.enabled = false;
        }
        _fireEffect.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        
        FinishAttack();
    }

    protected override void Combat()
    {
        _agent.SetDestination(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position);
        if (_enemyBase.GetDistance() < 0)
        {

        }
        else if (_enemyBase.GetDistance() > _enemyBase.StopDistance * _enemyBase.StopDistance)
        {
            if (_timer > _enemyBase.CoolTime)
            {
                _agent.SetDestination(transform.position);
                _state = State.Action;
                _timer = 0.0f;
                int judge = Random.Range(0, 3);
                switch(judge)
                {
                    case 0:
                        _animator.SetTrigger("Ball");
                        break;
                    case 1:
                        _animator.SetTrigger("Breath");
                        break;
                    case 2:
                        NavMeshHit navMeshHit;
                        if (NavMesh.SamplePosition(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position +
                            (_enemyBase.TargetCharacter.GetNearestPart(this.transform).position - transform.position).normalized * 10.0f,
                            out navMeshHit, 10.0f, NavMesh.AllAreas))
                        {
                            _rushVector = navMeshHit.position;
                            _animator.SetTrigger("Rush");
                        }
                        else
                        {
                            _state = State.Combat;
                        }
                        break;
                }
            }
        }
        else
        {
            _agent.velocity = Vector3.zero;
            if (_timer > _enemyBase.CoolTime)
            {
                _agent.SetDestination(transform.position);
                _state = State.Action;
                _timer = 0.0f;
                int judge = Random.Range(0, 3);
                switch (judge)
                {
                    case 0:
                        _animator.SetTrigger("Attack");
                        break;
                    case 1:
                        _animator.SetTrigger("Bite");
                        break;
                    case 2:
                        _animator.SetTrigger("Tail");
                        break;
                }
            }
        }
        _timer += Time.deltaTime;
    }
    protected override void Action()
    {
        if(_isRush)
        {
            _agent.SetDestination(_rushVector);
            if((_rushVector - transform.position).sqrMagnitude < 9.0f)
            {
                FinishRush();
            }
        }
    }
}
