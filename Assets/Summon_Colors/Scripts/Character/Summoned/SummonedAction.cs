using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SummonedBase))]
[RequireComponent(typeof(NavMeshAgent))]
public class SummonedAction : MonoBehaviour
{
    protected enum State
    {
        Idle,
        Combat,
        Return,
        Action,
        Down
    }
    protected SummonedBase _summonedBase;
    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected State _state = State.Idle;
    protected float _timer = 0.0f;

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

    public void FinishAction()
    {
        _state = State.Combat;
        _agent.updateRotation = true;
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
        _animator = GetComponent<Animator>();
        _agent.speed = _summonedBase.Agility;
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

        switch(_state)
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
}
