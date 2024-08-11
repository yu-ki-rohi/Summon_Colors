using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : NPCBase
{
    [SerializeField] private EnemyData _enemyData;
    private CharacterBase _player;

    public override void Damaged(int attack, int hate = 0, CharacterBase attacker = null)
    {
        if(attacker == _player)
        {
            hate = (int)(hate * 1.0f);
        }
        base.Damaged(attack, hate, attacker);
    }

    public override void RecognizeCharacter(Collider collider)
    {
        if(collider.tag == "Player")
        {
            if(_player == null &&
                TryGetComponent<CharacterBase>(out var player))
            {
                _player = player;
            }
            base.RecognizeCharacter(collider);
        }
        else if(collider.tag == "Summoned")
        {
            base.RecognizeCharacter(collider);
        }
        
    }

    public override void LostCharacter(Collider collider)
    {
        if (collider.tag == "Player" ||
            collider.tag == "Summoned")
        {
            base.LostCharacter(collider);
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        if (_enemyData == null)
        {
            Debug.LogError("Enemy Data is Null!!");
            return;
        }
        _characterData = _enemyData;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
}
