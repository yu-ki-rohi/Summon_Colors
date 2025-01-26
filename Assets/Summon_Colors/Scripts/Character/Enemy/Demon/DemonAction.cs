using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class DemonAction : EnemyAction
{
    [SerializeField] private ShaderManager _shaderManager;

    [SerializeField] private Transform _breathPosition;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private Collider[] _rushColliders;
    [SerializeField] private Collider _handAttackCollider;
    [SerializeField] private Collider[] _tailColliders;

    [SerializeField] private GameObject _volcanicBomb;
    [SerializeField] private GameObject _earthQuake;
    [SerializeField] private GameObject _fireBall;
    [SerializeField] private GameObject _tackleExplosion;
    [SerializeField] private ParticleSystem _flameStream;
    [SerializeField] private ParticleSystem _fireEmbers;
    [SerializeField] private ObjectPoolBase _breathPool;


    [SerializeField] private float _firePower = 10.0f;
    [SerializeField] private float _rushSpeed = 2.0f;
    [SerializeField] private float _explosionTackleSpeed = 60.0f;
    [SerializeField] private float _breathCoolTime = 0.3f;

    private float _breathTimer = 0.0f;
    private Vector3 _rushVector = Vector3.zero;
    private bool _isRush = false;
    private bool _isBreath = false;
    private float _roarIntensity = 0.0f;
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
        _handAttackCollider.enabled = true;
    }

    public void CreateExplosion()
    {
        Instantiate(_tackleExplosion, _rushColliders[2].transform.position, Quaternion.identity);
    }

    public void Bite()
    {
        _rushColliders[0].enabled = true;
    }


    public void CreateFireBall()
    {
        GameObject fireBall = Instantiate(_fireBall, _breathPosition.position, Quaternion.identity);
        fireBall.transform.forward = _breathPosition.forward;
    }
    public void StartBreath()
    {
        _flameStream.Play();
        _fireEmbers.Play();
        _isBreath = true;
    }

    public void StartTailAttack()
    {
        foreach (Collider col in _tailColliders)
        {
            col.enabled = true;
        }
    }

    public void StartRush()
    {
        _agent.speed = _enemyBase.Agility * _rushSpeed;
        _agent.velocity = (_rushVector - transform.position).normalized * 0.5f;
        _isRush = true;
        _agent.SetDestination(_rushVector);
        foreach(Collider col in _rushColliders)
        {
            col.enabled = true;
        }
    }

    public void StartExplosionTackle()
    {
        _agent.speed = _enemyBase.Agility * _explosionTackleSpeed;
        _agent.velocity = (_rushVector - transform.position).normalized * 60.0f;

        _agent.SetDestination(_rushVector);
        foreach (Collider col in _rushColliders)
        {
            col.enabled = true;
        }
    }

    public void FinishExplosionTackle()
    {
        _agent.speed = _enemyBase.Agility;
        _agent.updateRotation = true;
        _agent.SetDestination(transform.position);
    }

    public void Roar()
    {
        _roarIntensity = 0.6f;
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

        _flameStream.Stop();
        _fireEmbers.Stop();
        _isBreath = false;
    }

    protected override void Start()
    {
        base.Start();

        FinishAttack();
    }

    void FixedUpdate()
    {
        if(_roarIntensity > 0)
        {
            _shaderManager.SetIntensity(_firePosition.position, _roarIntensity);
            _roarIntensity *= 0.95f;
        }
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
                _agent.velocity = Vector3.zero;
                _agent.SetDestination(transform.position);
                _state = State.Action;
                _timer = 0.0f;
                int judge = Random.Range(0, 4);
                NavMeshHit navMeshHit;
                switch (judge)
                {
                    case 0:
                        _animator.SetTrigger("Ball");
                        break;
                    case 1:
                        _animator.SetTrigger("Breath");
                        break;
                    case 2:
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
                    case 3:
                        if (NavMesh.SamplePosition(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position,
                            out navMeshHit, 10.0f, NavMesh.AllAreas))
                        {
                            _rushVector = navMeshHit.position;
                            _agent.updateRotation = false;
                            _animator.SetTrigger("Explosion");
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
                int judge = Random.Range(0, 6);

                NavMeshHit navMeshHit;
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
                    case 3:
                        if (NavMesh.SamplePosition(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position +
                            (_enemyBase.TargetCharacter.GetNearestPart(this.transform).position - transform.position).normalized * 15.0f,
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
                    case 4:
                        _animator.SetTrigger("Breath");
                        break;
                    case 5:
                        if (NavMesh.SamplePosition(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position,
                            out navMeshHit, 10.0f, NavMesh.AllAreas))
                        {
                            _rushVector = navMeshHit.position;
                            _agent.updateRotation = false;
                            _animator.SetTrigger("Explosion");
                        }
                        else
                        {
                            _state = State.Combat;
                        }
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
            if((_rushVector - transform.position).sqrMagnitude < 9.0f ||
                _agent.velocity.sqrMagnitude <= 0)
            {
                FinishRush();
            }
        }
        else if(_isBreath)
        {
            if(_breathTimer < _breathCoolTime)
            {
                _breathTimer += Time.deltaTime;
            }
            else
            {
                _breathTimer = 0.0f;
                GameObject breath = _breathPool.Get(_firePosition.position);
                breath.transform.forward = _firePosition.forward;
            }
        }
    }
}
