using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SummonedBase : NPCBase
{
    [SerializeField] protected SummonedData _summonedData;
    private int _id;
    private Transform _standByPosition;
    private Summon _summon;
    private HomeBase _home;
    private SummonedPool _pool;
    private SummonedAction _action;
    private Vector3 _initialPos;
    public ColorElements.ColorType ColorType { get { return _summonedData.ColorType; } }
    public int Costs { get { return _summonedData.Costs; } }
    public float StopDistance { get { return _summonedData.StopDistance; } }    
    public HomeBase Home { get { return _home; } set { _home = value; } }
    public SummonedPool SummonedPool { get { return _pool; } set { _pool = value; } }

    public Transform StandByPosition { get { return _standByPosition; } }

    public virtual int GetCosts(int rank)
    {
        return Costs;
    }

    public override int Damaged(int attack, int shock = 0, int hate = 0, CharacterBase attacker = null)
    {
        int damage = base.Damaged(attack, shock, hate, attacker);
        if (damage <= 0) { return 0; }
        DamagedInvincible(0.8f);
        if(Hp <= 0)
        {
            Die();
        }
        return damage;
    }

    public override void KnockBack(Vector3 dir, float strength, float time)
    {
        _action.KnockBack(dir, strength, time);
    }

    public void Initialize(int id, Transform standByPosition, Summon summon, Vector3 summonedPosition)
    {
        _id = id;
        _standByPosition = standByPosition;
        _summon = summon;
        _characterData = _summonedData;
        _isActive = true;
        _action = GetComponent<SummonedAction>();
        if( _action != null )
        {
            _action.Warp(summonedPosition);
            _action.FinishAction();
        }
        _initialPos = summonedPosition;
        Heal(MaxHp);
        _targetCharacter = null;
        ReleaseCharacters();
    }

    public override void RecognizeCharacter(Collider collider)
    {
        if (collider.tag == "Enemy")
        {
            base.RecognizeCharacter(collider);
        }
    }

    public override void LostCharacter(Collider collider)
    {
        if (collider.tag == "Enemy")
        {
            base.LostCharacter(collider);
        }
    }

    public void ReleaseCharacters()
    {
        _hate.Clear();
        _targetCharacter = null;
    }

    public void Release()
    {
        _pool.Release(gameObject);
    }

    protected override void Die()
    {
        if (_summon != null)
        {
            _summon.Release(ColorType, _id);
        }
        if (_action != null)
        {
            _action.ChangeDown();
        }
        base.Die();
    }

    protected override void CheckThePosition(Vector3 position)
    {
        _action.CheckThePosition(position);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        if (_summonedData == null)
        {
            Debug.LogError("Summoned Data is Null!!");
            return;
        }
        _characterData = _summonedData;
        _action = GetComponent<SummonedAction>();
        if (_action != null)
        {
            _action.Warp(_initialPos);
        }
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base .Update();
    }
}
