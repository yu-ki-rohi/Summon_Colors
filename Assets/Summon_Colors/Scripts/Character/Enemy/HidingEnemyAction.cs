using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class HidingEnemyAction : EnemyAction
{
    private int _power;
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
    protected override void Idle()
    {
        _agent.velocity = Vector3.zero;
    }

    protected override void Walk()
    {
        
    }

    protected override void SetState()
    {
        // この辺はちょっと煩雑なので、後々直したい
        if (_state != State.Action && _state != State.Down)
        {
            if (_enemyBase.TargetCharacter != null &&
            _enemyBase.TargetCharacter.gameObject.activeSelf)
            {
                _animator.SetBool("Recognize", true);
                _state = State.Combat;
            }
            else
            {
                if (_enemyBase.SetTarget())
                {
                    _animator.SetBool("Recognize", true);
                    _state = State.Combat;
                }
                else
                {
                    _animator.SetBool("Recognize", false);
                    _state = State.Idle;
                }
            }
        }
    }

    protected override void StartAction()
    {
        int judge = Random.Range(0, 2);
        switch (judge)
        {
            case 0:
                _animator.SetTrigger("Attack01");
                _power = _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification(0));
                break;
            case 1:
                _animator.SetTrigger("Attack02");
                _power = _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification(1));
                break;
        }
        _agent.updateRotation = false;
        _state = State.Action;
    }
}
