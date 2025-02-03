using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeAction : RedAction
{
    public override void Attack(Collider collider)
    {
        if (collider.tag == "Enemy")
        {
            CharacterBase character = collider.GetComponentInParent<CharacterBase>();
            if (character != null)
            {
                character.Damaged(0,0, _summonedBase.Appearance, _summonedBase);
            }
        }
    }
}
