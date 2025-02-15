using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenAction : SummonedAction
{
    public virtual void Attack(Collider collider)
    {
        if (collider.tag == "Enemy")
        {
            CharacterBase character = collider.GetComponentInParent<CharacterBase>();
            if (character != null)
            {
                character.Damaged(0, 0, _summonedBase.Appearance, _summonedBase);
            }
        }
    }
    public void Heal(Collider collider)
    {
        if (collider.tag == "Player" || collider.tag == "Summoned")
        {
            CharacterBase character = collider.GetComponentInParent<CharacterBase>();
            if (character != null)
            {
                if(character.Hp > 0)
                {
                    character.Heal(_summonedBase.Attack);
                    HitEffectManager.Instance.Play(HitEffectManager.Type.Heal, character.gameObject.transform.position);
                }
            }
        }
    }

    protected override void Idle()
    {
        base.Idle();
        WalkAndRecover();
    }

    protected override void Action()
    {
        _animator.SetTrigger("Action");
        _state = State.Action;
        _agent.SetDestination(transform.position);
    }
    protected override void Return()
    {
        base.Return();
        WalkAndRecover();
    }

    private void WalkAndRecover()
    {
        if ((_summonedBase.StandByPosition.position - transform.position).sqrMagnitude > 0.64f)
        {
            _animator.SetBool("IsWalking", true);
        }
        else
        {
            _animator.SetBool("IsWalking", false);
        }
        if ((_summonedBase.StandByPosition.position - transform.position).sqrMagnitude < _summonedBase.StopDistance * _summonedBase.StopDistance)
        {
            if (_timer < _summonedBase.CoolTime)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                Action();
                _timer = 0.0f;
            }
        }
    }
}
