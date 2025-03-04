using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBase : NPCBase
{
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private UIManager _uiMmanager;
    [SerializeField] private GameObject[] _bodyObjects;
    private EnemyAction _action;
    private CharacterBase _player;
    public float StopDistance { get { return _enemyData.StopDistance; } }

    public void Initialize()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_animator != null)
        {
            _animator.SetBool("Die", false);
        }
    }

    public override int Damaged(int attack, int shock = 0, int hate = 0, CharacterBase attacker = null)
    {
        if (Hp <= 0) { return -1; }
        // プレイヤーに個別の処理を入れる場合
        if (attacker == _player)
        {
            hate += (int)(hate * 1.0f);
        }

        int damage = base.Damaged(attack, shock, hate, attacker);
        if(_uiMmanager != null)
        {
            _uiMmanager.ReflectEnemyHp((float)Hp / MaxHp);
        }
        if (Hp <= 0)
        {
            InGameManager.Instance.DefeatEnemy();
            Die();
        }
        return damage;
    }

    public override void KnockBack(Vector3 dir, float strength, float time)
    {
        _action.KnockBack(dir, strength, time);
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

    protected override void CheckThePosition(CharacterBase attacker)
    {
        if(_action != null)
        {
            _action.CheckThePosition(attacker.transform.position);
        }
        else
        {
            _hate.Add(attacker, 1);
            _targetCharacter = attacker;
        }
    }

    protected override void Die()
    {
        _action.Die();
        ChangeTagAndLayer();
        base.Die();
    }

    private void ChangeTagAndLayer()
    {
        int layer = LayerMask.NameToLayer("Default");
        for(int i = 0; i < _bodyObjects.Length; i++)
        {
            _bodyObjects[i].layer = layer;
            _bodyObjects[i].tag = "Element";

        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        InGameManager.Instance.AddEnemy();
        if (_action == null)
        {
            _action = GetComponent<EnemyAction>();
        }
        if( _action == null ) { Debug.Log("Enemy Action " + gameObject + " has is Null" ); }
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }


    private void Awake()
    {
        if (_enemyData == null)
        {
            Debug.LogError("Enemy Data is Null!!");
            return;
        }
        _characterData = _enemyData;
    }
}
