using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class DemonAction : EnemyAction
{
    private enum Skill
    {
        Bite,
        Clap,
        Tail,
        Rush,
        Fire_Head,
        Fire_Body,
        Fire_Remain,
        Ball_Direct,
        Ball_Explosion,
        Ball_Remain,
        Landing_Direct,
        Landing_Explosion,
        Landing_Remain
    }


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
    private int _power = 0;
    public void CreateVolcanicBomb()
    {
        GameObject volcanicBomb = Instantiate(_volcanicBomb, _firePosition.position, Quaternion.identity);
        volcanicBomb.transform.forward = _firePosition.forward;
        if(volcanicBomb.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            rigidbody.AddForce(_firePosition.forward * _firePower, ForceMode.Impulse);
        }
        if (volcanicBomb.TryGetComponent<VolcanicBomb>(out var projectile))
        {
            projectile.Initialize(
                (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Ball_Direct)),
                (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Ball_Explosion)));
        }
    }

    public void CreateEarthQuake()
    {
        GameObject earthQuake = Instantiate(_earthQuake, _handAttackCollider.transform.position + Vector3.down * 1.0f, Quaternion.identity);
        earthQuake.transform.forward = transform.forward;
        _handAttackCollider.enabled = true;
        _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Clap));
    }

    public void CreateExplosion()
    {
        GameObject explosionObj = Instantiate(_tackleExplosion, _rushColliders[2].transform.position, Quaternion.identity);
        if (explosionObj.TryGetComponent<Explosion>(out var explosion))
        {
            explosion.Initialize((int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Landing_Explosion)));
        }
    }

    public void Bite()
    {
        _rushColliders[0].enabled = true;
        _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Bite));
    }


    public void CreateFireBall()
    {
        GameObject fireBall = Instantiate(_fireBall, _breathPosition.position, Quaternion.identity);
        fireBall.transform.forward = _breathPosition.forward;
        if (fireBall.TryGetComponent<Projectiles>(out var projectile))
        {
            projectile.Initialize(
                (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Fire_Head)));
        }
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
        _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Tail));
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
        _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Rush));
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
        _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Landing_Direct));
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
        _agent.updateRotation = true;
        _power = 0;
    }

    public override void Attack(Collider collider)
    {
        if (collider.tag == "Player" || collider.tag == "Summoned")
        {
            CharacterBase character = collider.GetComponentInParent<CharacterBase>();
            if (character != null)
            {
                character.Damaged(_power, _enemyBase.Break, _enemyBase.Appearance, _enemyBase);
            }
        }
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
        SelectAttack();
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
                if (breath.TryGetComponent<Projectiles>(out var projectile))
                {
                    projectile.Initialize(
                        (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Fire_Body)));
                }
            }
        }
    }

    private void SelectAttack()
    {
        float dot = _enemyBase.GetDot();
        if (dot < 0) { return; }

        float buffar = 2.0f;
        float borderDistance = _enemyBase.StopDistance + buffar;
        borderDistance *= borderDistance;
        if (_enemyBase.GetDistance() > borderDistance)
        {
            if (_timer > _enemyBase.CoolTime)
            {
                _timer = 0.0f;
                SelectFarAttack(dot);
            }
        }
        else
        {
            if (_enemyBase.GetDistance() < _enemyBase.StopDistance * _enemyBase.StopDistance)
            { _agent.velocity = Vector3.zero; }
                
            if (_timer > _enemyBase.CoolTime)
            {
                _timer = 0.0f;
                SelectNearAttack(dot);
            }
        }
    }

    private void SelectFarAttack(float dot)
    {
        if(dot < 0.866f) { return; }

        _agent.velocity = Vector3.zero;
        _agent.SetDestination(transform.position);
        _state = State.Action;
        int judge;
        if (dot < 0.9659f)
        {
            judge = Random.Range(1, 4);
        }
        else
        {
            judge = Random.Range(0, 4);
        }
        NavMeshHit navMeshHit;
        switch (judge)
        {
            case 0:
                _animator.SetTrigger("Ball");
                _agent.updateRotation = false;
                break;
            case 1:
                _animator.SetTrigger("Breath");
                _agent.updateRotation = false;
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

    private void SelectNearAttack(float dot)
    {
        _agent.SetDestination(transform.position);
        _state = State.Action;
        int judge;
        if (dot < 0.7071f)
        {
            judge = Random.Range(3, 6);
        }
        else if(dot < 0.9397f)
        {
            judge = Random.Range(1, 6);
        }
        else
        {
            judge = Random.Range(0, 6);
        }
        

        NavMeshHit navMeshHit;
        switch (judge)
        {
            case 0:
                _animator.SetTrigger("Bite");
                _agent.updateRotation = false;
                break;
            case 1:
                _animator.SetTrigger("Attack");
                _agent.updateRotation = false;
                break;
            case 2:
                _animator.SetTrigger("Breath");
                _agent.updateRotation = false;
                break;
            case 3:
                _animator.SetTrigger("Tail");
                _agent.updateRotation = false;
                break;
            case 4:
                if (NavMesh.SamplePosition(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position +
                    (_enemyBase.TargetCharacter.GetNearestPart(this.transform).position - transform.position).normalized * 12.0f,
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
