using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] private Transform[] _partTransforms;
    protected CharacterData _characterData;

    private int _currentHp;

    public int Hp { get {  return _currentHp; } }
    public int MaxHp { get {  return _characterData.MaxHp; } }
    public int Attack { get { return _characterData.Attack; } }
    public int Vitality { get { return _characterData.Vitality; } }
    public float Agility { get {  return _characterData.Agility; } }
    public float CoolTime { get { return _characterData.CoolTime; } }

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
    public virtual void Damaged(int attack, int hate = 0, CharacterBase attacker = null)
    {
        int damage = attack - Vitality;
        if (damage > 0)
        {
            _currentHp -= damage;
        }
    }

    public virtual void Heal(int heal)
    {
        _currentHp += heal;
        if(_currentHp > MaxHp)
        {
            _currentHp = MaxHp;
        }
    }

    protected virtual void Die()
    {

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
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    private bool IsANearerThanB(Transform A,Transform B, Transform target)
    {
        return (A.position - target.position).sqrMagnitude < (B.position - target.position).sqrMagnitude;
    }
}
