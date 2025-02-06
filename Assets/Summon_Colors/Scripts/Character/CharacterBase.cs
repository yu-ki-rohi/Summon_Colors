using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] private Transform[] _partTransforms;
    protected CharacterData _characterData;
    protected Animator _animator;
    protected bool _isActive = true;
    protected float _inbincibleTime = 0.0f;

    private int _currentHp;
    private int _armor;

    private bool _isInvincible = false;
    private Timer _invincibleTimer;

    public int Hp { get {  return _currentHp; } }
    public int MaxHp { get {  return _characterData.MaxHp; } }
    public int Attack { get { return _characterData.Attack; } }
    public int Vitality { get { return _characterData.Vitality; } }
    public int Break { get { return _characterData.Break; } }
    public int Appearance { get { return _characterData.Appearance; } }
    public float Agility { get {  return _characterData.Agility; } }
    public float CoolTime { get { return _characterData.CoolTime; } }

    public bool IsActive { get { return _isActive; } }
    public bool IsInvincible { get {  return _isInvincible; } }


    public float GetPowerMagnification(int index)
    {
        if(index < 0 ||
            index >= _characterData.PowerMagnifications.Length)
        { 
            return 1.0f;
        }

        return _characterData.PowerMagnifications[index];
    }

    public Transform GetNearestPart(Transform other)
    {
        Transform partTransform = null;
        for(int i = 0; i < _partTransforms.Length; i++)
        {
            if(partTransform == null)
            {
                partTransform = _partTransforms[i];
            }
            else
            {
                if (IsANearerThanB(_partTransforms[i], partTransform, other))
                {
                    partTransform = _partTransforms[i];
                }
            }
        }
        if(partTransform == null)
        {
            return this.transform;
        }
        return partTransform;
    }
    public virtual int Damaged(int attack, int shock = 0,int hate = 0, CharacterBase attacker = null)
    {
        if(!_isActive || _isInvincible)
        {
            return 0;
        }
        int damage = (int)(attack * (1.0f - Vitality * 0.01f));
        if (damage > 0)
        {
            _currentHp -= damage;
        }

        _armor -= shock;
        if(_armor <= 0)
        {
            _armor = _characterData.Armor;
            Broken();
        }

        _invincibleTimer = new Timer(FinishInvincible, _inbincibleTime);

        return damage;
    }

    public virtual void Heal(int heal)
    {
        _currentHp += heal;
        if(_currentHp > MaxHp)
        {
            _currentHp = MaxHp;
        }
    }

    public virtual void KnockBack(Vector3 dir, float strength, float time)
    {

    }

    public void StartInvincible()
    {
        _isInvincible = true;
    }

    public void FinishInvincible()
    {
        _isInvincible = false;
        _invincibleTimer = null;
    }

    protected void DamagedInvincible(float time)
    {
        _isInvincible = true;
        _invincibleTimer = new Timer(FinishInvincible, time);
    }

    protected virtual void Broken()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Broken");
        }
    }

    protected virtual void Die()
    {
        if(_animator != null)
        {
            _animator.SetBool("Die",true);
        }
        _isActive = false;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if(_characterData == null)
        {
            Debug.LogError("Character Data is Null!!");
            return;
        }
        _currentHp = _characterData.MaxHp;
        _armor = _characterData.Armor;
        _animator = GetComponent<Animator>();
        if( _animator != null )
        {
            _animator.SetBool("Die", false);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(_invincibleTimer != null)
        {
            _invincibleTimer.CountUp(Time.deltaTime);
        }
    }

    private bool IsANearerThanB(Transform A,Transform B, Transform target)
    {
        return (A.position - target.position).sqrMagnitude < (B.position - target.position).sqrMagnitude;
    }
}
