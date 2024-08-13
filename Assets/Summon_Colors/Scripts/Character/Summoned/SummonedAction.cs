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
        Move,
        Chase,
        Return,
        Action
    }
    protected SummonedBase _summonedBase;
    protected NavMeshAgent _agent;
    protected State _state;
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
        
    }
}
