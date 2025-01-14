using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedAction : SummonedAction
{
    [SerializeField] private Collider _attackCollider;

    public virtual void Attack(Collider collider)
    {
        if(collider.tag == "Enemy")
        {
            CharacterBase character = collider.GetComponentInParent<CharacterBase>();
            if(character != null)
            {
                character.Damaged(_summonedBase.Attack, _summonedBase.Break, _summonedBase.Attack, _summonedBase);
            }
        }
    }

    public void StartAttack()
    {
        _attackCollider.enabled = true;
    }

    public void FinishAttack()
    {
        _attackCollider.enabled = false;
    }

    protected override void Start()
    {
        base.Start();
        if(_attackCollider != null )
        {
            _attackCollider.enabled = false;
        }
    }

    protected override void Idle()
    {
        base.Idle();
        if(_agent.velocity.sqrMagnitude > 0)
        {
            _animator.SetBool("IsWalking", true);
        }
        else
        {
            _animator.SetBool("IsWalking", false);
        }
    }

    protected override void Combat()
    {
        base.Combat();
        if(_summonedBase.GetDistance() < 0)
        {

        }
        else if (_summonedBase.GetDistance() > _summonedBase.StopDistance * _summonedBase.StopDistance)
        {
            _animator.SetBool("IsWalking", true);
        }
        else
        {
            _animator.SetBool("IsWalking", false);
            _agent.velocity = Vector3.zero;
            if (_timer > _summonedBase.CoolTime)
            {
                Action();
                _timer = 0.0f;
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }
    }

    protected override void Action()
    {
        _animator.SetTrigger("Action");
        _state = State.Action;
        _agent.updateRotation = false;
        _agent.SetDestination(transform.position);
    }

}
