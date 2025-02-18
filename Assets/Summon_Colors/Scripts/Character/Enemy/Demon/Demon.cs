using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : EnemyBase
{
    private DemonAction _demonAction;

    public override int Damaged(int attack, int shock = 0, int hate = 0, CharacterBase attacker = null)
    {
        int beforeHp = Hp;
        int damage = base.Damaged(attack, shock, hate, attacker);
        float changeBorder = 0.5f;
        int judgeChange = (Hp - (int)(MaxHp * changeBorder)) * (beforeHp - (int)(MaxHp * changeBorder));
        if (judgeChange <= 0 && _demonAction != null)
        {
            Debug.Log("Ignit");
            _demonAction.IgnitEventMove();
        }
        return damage;
    }

    public override void KnockBack(Vector3 dir, float strength, float time)
    {
        
    }

    protected override void Die()
    {
        InGameManager.Instance.GameClear();
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Die, transform);
        base.Die();
    }

    protected override void Start()
    {
        base.Start();
        _demonAction = GetComponent<DemonAction>();
        if (_demonAction == null) { Debug.Log("Demon Action is Null"); }
    }
}
