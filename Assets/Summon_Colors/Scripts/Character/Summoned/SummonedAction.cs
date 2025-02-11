using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SummonedBase))]
public class SummonedAction : MonoBehaviour
{
    protected enum State
    {
        Idle,
        Combat,
        Return,
        Action,
        Down,
        Check
    }
    [SerializeField] protected NavMeshAgent _agent;
    [SerializeField] protected Animator _animator;

    protected SummonedBase _summonedBase;
    protected State _state = State.Idle;
    protected float _timer = 0.0f;
    private Rigidbody _rigidbody;
    private Timer _knockBackTimer;
    private Timer _checkTimer;


    public void Initialize()
    {
        if (_summonedBase == null)
        {
            _summonedBase = GetComponent<SummonedBase>();
        }
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        _agent.speed = _summonedBase.Agility;
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

    public void ChangeDown()
    {
        _state = State.Down;
        _agent.updatePosition = false;
        _agent.updateRotation = false;
    }

    public virtual void FinishAction()
    {
        _state = State.Combat;
        _agent.updateRotation = true;
    }

    public void KnockBack(Vector3 dir, float strength, float time)
    {
        _agent.velocity = Vector3.zero;
        _agent.updatePosition = false;
        _agent.updateRotation = false;

        _knockBackTimer = new Timer(FinishKnockBack, time + 0.5f);
        dir.y = 0;
        if (dir.sqrMagnitude != 1.0f)
        {
            dir = dir.normalized;
        }
        _rigidbody.AddForce(dir * strength, ForceMode.Impulse);
    }

    public void CheckThePosition(Vector3 position)
    {
        _state = State.Check;
        _agent.SetDestination(position);
        _checkTimer = new Timer(FinishAction, 1.0f);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if(_summonedBase == null)
        {
            _summonedBase = GetComponent<SummonedBase>();
        }
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        _state = State.Idle;
        if(_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _agent.speed = _summonedBase.Agility;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // ごちゃごちゃしているので、いずれ整理したい
        if (_state != State.Return && _state != State.Action && _state != State.Down)
        {
            if (_summonedBase.TargetCharacter != null)
            {
                _state = State.Combat;
            }
            else
            {
                if(_summonedBase.SetTarget())
                {
                    _state = State.Combat;
                }
                else
                {
                    _state = State.Idle;
                }
            }

            if (_summonedBase.Home.IsReturn())
            {
                _state = State.Return;
            }
        }
        if(_knockBackTimer != null)
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
                case State.Combat:
                    Combat();
                    break;
                case State.Return:
                    Return();
                    break;
            }
            _rigidbody.velocity = Vector3.zero;
        }
    }

    protected virtual void Idle()
    {
        _agent.SetDestination(_summonedBase.StandByPosition.position);
    }

    protected virtual void Combat()
    {
        _agent.SetDestination(_summonedBase.TargetCharacter.GetNearestPart(this.transform).position);
    }

    protected virtual void Return()
    {
        _agent.SetDestination(_summonedBase.StandByPosition.position);
        if((_summonedBase.StandByPosition.position - transform.position).sqrMagnitude < 1.0f)
        {
            _summonedBase.ReleaseCharacters();
            _state = State.Idle;
        }
    }

    protected virtual void Action()
    {

    }

    private void FinishKnockBack()
    {
        _knockBackTimer = null;
        Warp(transform.position);
    }

    private void FinishCheck()
    {
        if(_state == State.Check)
        {
            _state = State.Idle;
        }
        _checkTimer = null;
    }
}
