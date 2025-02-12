using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : EnemyBase
{
    protected override void Die()
    {
        InGameManager.Instance.GameClear();
        base.Die();
    }
}
