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
        Action
    }
    protected SummonedBase _summonedBase;
    protected NavMeshAgent _agent;
    protected State _state = State.Idle;
    protected float _timer;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _summonedBase = GetComponent<SummonedBase>();
        _agent = GetComponent<NavMeshAgent>();
        _state = State.Idle;

        _agent.speed = _summonedBase.Agility;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_state != State.Return)
        {
            if (_summonedBase.TargetCharacter != null)
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
        _agent.SetDestination(_summonedBase.TargetCharacter.transform.position);
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
