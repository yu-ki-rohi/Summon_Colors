using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyBase))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAction : MonoBehaviour
{
    [SerializeField] private Collider _attackCollider;
    [SerializeField] private Collider _bodyCollider;

    protected enum State
    {
        Idle,
        Walk,
        Combat,
        Action
    }
    protected EnemyBase _enemyBase;
    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected State _state = State.Idle;
    protected float _timer = 0.0f;
    protected float _walkTimer = 0.0f;
    protected float _walkTime = 3.0f;
    protected Vector3 _walkVec = Vector3.zero;


    public void Attack(Collider collider)
    {
        if (collider.tag == "Player" || collider.tag == "Summoned")
        {
            CharacterBase character = collider.GetComponentInParent<CharacterBase>();
            if (character != null)
            {
                character.Damaged(_enemyBase.Attack, _enemyBase.Attack, _enemyBase);
            }
        }
    }

    public void StartAttack()
    {
        _attackCollider.enabled = true;
        //_bodyCollider.enabled = false;
    }

    public virtual void FinishAttack()
    {
        _attackCollider.enabled = false;
        //_bodyCollider.enabled = true;
    }

    public void FinishAction()
    {
        _state = State.Combat;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _enemyBase = GetComponent<EnemyBase>();
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        _state = State.Idle;
        _animator = GetComponent<Animator>();

        if (_enemyBase == null)
        {
            Debug.Log("Enemy Base is Null!!");
        }

        if (_agent == null)
        {
            Debug.Log("Agent is Null!!");
        }

        _agent.speed = _enemyBase.Agility;
        if(_attackCollider != null)
        {
            _attackCollider.enabled = false;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_state != State.Action)
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
                    if (_state != State.Walk)
                    {
                        _state = State.Idle;
                    }
                }
            }
        }
         

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
        }

        if (_agent.velocity.sqrMagnitude > 0)
        {
            _animator.SetBool("IsWalking", true);
        }
        else
        {
            _animator.SetBool("IsWalking", false);
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
        _agent.SetDestination(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position);
        if (_enemyBase.GetDistance() < 0)
        {

        }
        else if (_enemyBase.GetDistance() > _enemyBase.StopDistance * _enemyBase.StopDistance)
        {

        }
        else
        {
            _agent.velocity = Vector3.zero;
            if (_timer > _enemyBase.CoolTime)
            {
                _animator.SetTrigger("Attack");
                _state = State.Action;
                _timer = 0.0f;
            }
            else
            {
                _timer += Time.deltaTime;
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
}
