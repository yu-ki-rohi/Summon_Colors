using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyBase))]
public class EnemyAction : MonoBehaviour
{
    [SerializeField] private Collider _attackCollider;
    [SerializeField] private Collider _bodyCollider;
    [SerializeField] protected NavMeshAgent _agent;
    [SerializeField] protected bool _isForwardInverse = false;
    protected Animator _animator;
    private Rigidbody _rigidbody;
    protected enum State
    {
        Idle,
        Walk,
        Combat,
        Action,
        Down,
        Check
    }
    protected EnemyBase _enemyBase;
    protected State _state = State.Idle;
    protected float _timer = 0.0f;
    protected float _walkTimer = 0.0f;
    protected float _walkTime = 3.0f;
    protected Vector3 _walkVec = Vector3.zero;
    protected Timer _actionTimer = null;
    protected Timer _knockBackTimer = null;


    public virtual void Attack(Collider collider)
    {
        if (collider.tag == "Player" || collider.tag == "Summoned")
        {
            CharacterBase character = collider.GetComponentInParent<CharacterBase>();
            if (character != null)
            {
                int damage = character.Damaged(_enemyBase.Attack, _enemyBase.Break, _enemyBase.Appearance, _enemyBase);
                if (damage != 0)
                {
                    HitEffectManager.Instance.Play(HitEffectManager.Type.Hit, collider.ClosestPointOnBounds(transform.position));
                }
                if (damage > 0)
                {
                    float time = 10.0f;
                    float forcePower = 150.0f;
                    Vector3 forceVec = (collider.transform.position - transform.position);
                    character.KnockBack(forceVec, forcePower, time);
                }
            }
        }
    }

    public void KnockBack(Vector3 dir, float strength, float time)
    {
        _agent.velocity = Vector3.zero;
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _knockBackTimer = new Timer(FinishKnockBack, time / _rigidbody.mass);
        dir.y = 0;
        if (dir.sqrMagnitude != 1.0f)
        {
            dir = dir.normalized;
        }
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(dir * strength, ForceMode.Impulse);
    }

    public void Warp(Vector3 pos)
    {
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        _agent.Warp(pos);

        // 再起動時に呼ばれるので、ついでに実行
        _agent.updatePosition = true;
        _agent.updateRotation = true;
    }

    public void StartAttack()
    {
        if (_attackCollider != null)
        {
            _attackCollider.enabled = true;
        }
        //_bodyCollider.enabled = false;
    }

    public virtual void FinishAttack()
    {
        if( _attackCollider != null )
        {
            _attackCollider.enabled = false;
        }
        //_bodyCollider.enabled = true;
    }

    public virtual void FinishAction()
    {
        _state = State.Combat;
        _agent.updateRotation = true;
    }

    public virtual void ChangeDown()
    {
        _state = State.Down;
        FinishAttack();
        if(_agent != null)
        {
            _agent.SetDestination(transform.position);
            if (!_enemyBase.IsActive)
            {
                _agent.updateRotation = false;
                _agent.updatePosition = false;
            }
        }
        if(_actionTimer != null)
        {
            _actionTimer.Reset();
        }
    }

    public void Die()
    {
        ChangeDown();
        _rigidbody.isKinematic = true;
    }

    public virtual void CheckThePosition(Vector3 position)
    {
        _walkTimer = 0.0f;
        _walkTime = Random.Range(4.0f, 8.0f);
        _agent.SetDestination(position);
        _state = State.Check;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _enemyBase = GetComponent<EnemyBase>();
        _state = State.Idle;
        if(_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_enemyBase == null)
        {
            Debug.Log("Enemy Base is Null!!");
        }
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        if (_agent == null)
        {
            Debug.Log("Agent is Null!!");
        }
        _rigidbody = GetComponentInParent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.Log("RigidBody is Null at " + gameObject);
        }

        _agent.speed = _enemyBase.Agility;
        if(_attackCollider != null)
        {
            _attackCollider.enabled = false;
        }
        _actionTimer = new Timer(StartAction, _enemyBase.CoolTime);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (InGameManager.Instance.IsEvent) { return; }
        SetState();

        if (_knockBackTimer != null)
        {
            _knockBackTimer.CountUp(Time.deltaTime);
        }
        else
        {

            switch (_state)
            {
                case State.Idle:
                    Idle();
                    break;
                case State.Walk:
                    Walk();
                    break;
                case State.Combat:
                    Combat();
                    break;
                case State.Action:
                    Action();
                    break;
                case State.Check:
                    Check();
                    break;
            }
        }

        if(_agent != null)
        {
            float dot = _enemyBase.GetDot();
            if(_isForwardInverse && dot <= 1.0f) { dot *= -1; }
            // 判定をfloatにしてもいいかも
            if (_agent.velocity.sqrMagnitude > 0.01f ||
                dot < 0.7071f)
            {
                _animator.SetBool("IsWalking", true);
            }
            else
            {
                _animator.SetBool("IsWalking", false);
            }
        }
    }

    protected virtual void SetState()
    {
        // この辺はちょっと煩雑なので、後々直したい
        if (_state != State.Action && _state != State.Down)
        {
            if (_enemyBase.TargetCharacter != null &&
         _enemyBase.TargetCharacter.gameObject.activeSelf)
            {
                _state = State.Combat;
            }
            else
            {
                if (_enemyBase.SetTarget())
                {
                    _state = State.Combat;
                }
                else
                {
                    if (_state != State.Walk && _state != State.Check)
                    {
                        _state = State.Idle;
                    }
                }
            }
        }


    }

    protected virtual void Idle()
    {
        if(_walkTimer < _walkTime)
        {
            _walkTimer += Time.deltaTime;
        }
        else
        {
            _walkTimer = 0.0f;
            _walkTime = Random.Range(4.0f, 8.0f);
            _walkVec = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
            _state = State.Walk;
        }
    }

    protected virtual void Combat()
    {
        
        float buffar = 2.0f;
        float borderDistance = _enemyBase.StopDistance + buffar;
        borderDistance *= borderDistance;
        if (_enemyBase.GetDistance() > borderDistance)
        {
            _agent.SetDestination(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position);
        }
        else
        {
            float dot = _enemyBase.GetDot();
            if (_isForwardInverse) { dot *= -1.0f; }
            if (_enemyBase.GetDistance() > _enemyBase.StopDistance * _enemyBase.StopDistance) 
            { 
                _agent.SetDestination(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position);
            }
            else 
            {
                if(dot > 0.7071f)
                {
                    _agent.velocity = Vector3.zero;
                }
                else
                {
                    _agent.SetDestination(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position);
                }
            }
            if (dot < 0) { return; }

            if (_actionTimer != null)
            {
                _actionTimer.CountUp(Time.deltaTime);
            }
        }
    }

    protected virtual void Walk()
    {
        if (_walkTimer < _walkTime)
        {
            _agent.SetDestination(transform.position + _walkVec * 5.0f);
            _walkTimer += Time.deltaTime;
        }
        else
        {
            _agent.SetDestination(transform.position);
            _walkTimer = 0.0f;
            _walkTime = Random.Range(2.0f, 4.0f);
            _state = State.Idle;
        }
    }

    protected virtual void Action()
    {
        
    }

    protected virtual void Check()
    {
        if (_walkTimer < _walkTime)
        {
            _walkTimer += Time.deltaTime;
        }
        else
        {
            _agent.SetDestination(transform.position);
            _walkTimer = 0.0f;
            _walkTime = Random.Range(2.0f, 4.0f);
            _state = State.Idle;
        }
    }

    protected virtual void StartAction()
    {
        _agent.velocity = Vector3.zero;
        _animator.SetTrigger("Attack");
        _state = State.Action;
        _agent.updateRotation = false;
    }
    private void FinishKnockBack()
    {
        _knockBackTimer = null;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        Warp(transform.position);
    }

}
