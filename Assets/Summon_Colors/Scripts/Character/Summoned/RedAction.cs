using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedAction : SummonedAction
{
    [SerializeField] private Collider _attackCollider;
    [SerializeField] private HitEffectManager.Type _type;
    [SerializeField] private bool _inverseForceVec = false;
    private List<CharacterBase> _characters = new List<CharacterBase>();
    public virtual void Attack(Collider collider)
    {
        if(collider.tag == "Enemy")
        {
            CharacterBase character = collider.GetComponentInParent<CharacterBase>();
            if(character != null)
            {
                if(!HasAttacked(character))
                {
                    character.Damaged(_summonedBase.Attack, _summonedBase.Break, _summonedBase.Appearance, _summonedBase);
                    HitEffectManager.Instance.Play(_type, collider.ClosestPointOnBounds(transform.position));
                    _characters.Add(character);
                    float time = 8.0f;
                    float forcePower = 25.0f;
                    Vector3 forceVec = (collider.transform.position - transform.position);
                    if (_inverseForceVec)
                    {
                        forceVec *= -1.0f;
                    }
                    character.KnockBack(forceVec, forcePower, time);
                }
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

    public override void FinishAction()
    {
        ResetHasAttacked();
        base.FinishAction();
    }

    public void ResetHasAttacked()
    {
        _characters.Clear();
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
        
        if((_summonedBase.StandByPosition.position - transform.position).sqrMagnitude > 0.64f)
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
        float dot = _summonedBase.GetDot();
        if (dot < 0) { return; }
        float distance = _summonedBase.GetDistance();
        float buffar = 2.8f;
        float borderDistance = _summonedBase.StopDistance + buffar;
        borderDistance *= borderDistance;
        if (distance > borderDistance || dot < 0.7071)
        {
            _animator.SetBool("IsWalking", true);
        }
        else
        {
            if(distance < _summonedBase.StopDistance * _summonedBase.StopDistance)
            {
                _agent.velocity = Vector3.zero;
                _animator.SetBool("IsWalking", false);
            }
            else
            {
                _animator.SetBool("IsWalking", true);
            }
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
        _agent.velocity = Vector3.zero;
        _agent.updateRotation = false;
        _agent.SetDestination(transform.position);
    }

    protected override void Return()
    {
        base.Return();
        if ((_summonedBase.StandByPosition.position - transform.position).sqrMagnitude > 0.64f)
        {
            _animator.SetBool("IsWalking", true);
        }
        else
        {
            _animator.SetBool("IsWalking", false);
        }
    }

    protected bool HasAttacked(CharacterBase characterBase)
    {
        foreach(var character in _characters)
        {
            if(character == characterBase)
            {
                return true;
            }
        }
        return false;
    }
}
