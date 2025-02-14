using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingObject : Projectiles
{
    [SerializeField] private float _powerMagni;
    private int _break = 0;
    private int _hate = 0;
    private CharacterBase _character;

    public void Initialize(int attackPower, int breakPower, int hatePower, CharacterBase attacker)
    {
        _power = attackPower;
        _break = breakPower;
        _hate = hatePower;
        _character = attacker;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.tag == "Enemy")
        {
            CharacterBase characterBase = other.GetComponentInParent<CharacterBase>();
            if (characterBase != null)
            {
                int damage = characterBase.Damaged((int)(_power * _powerMagni),(int)(_break * _powerMagni), (int)(_hate * _powerMagni), _character);
                if(damage != 0)
                {
                    HitEffectManager.Instance.Play(HitEffectManager.Type.Hit, transform.position);
                }
            }
            DisAppear();
        }
    }

}
