using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class NPCBase : CharacterBase
{
    [SerializeField] protected Transform _eyesTransform;
    protected CharacterBase _targetCharacter = null;
    protected Dictionary<CharacterBase,int> _hate = new Dictionary<CharacterBase,int>();
    private Vector3 _eyesPosition = Vector3.zero;

    public CharacterBase TargetCharacter { get { return _targetCharacter; } }

    public override void Damaged(int attack, int shock = 0, int hate = 0, CharacterBase attacker = null)
    {
        base.Damaged(attack, shock, hate, attacker);
        if (attacker != null)
        {
            if(IsCharacterRecognized(attacker))
            {
                _hate[attacker] += hate;
                _targetCharacter = GetCharacterHaveMostHate();
            }

            if(_targetCharacter == null)
            {
                float sqrDistance = (attacker.gameObject.transform.position - transform.position).sqrMagnitude;
                if(sqrDistance < 100.0f)
                {
                    _hate.Add(attacker, attack);
                }
            }
        }
    }

    public bool SetTarget()
    {
        _targetCharacter = GetCharacterHaveMostHate();
        if(_targetCharacter != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetDistance()
    {
        if (_targetCharacter == null)
        {
            return -1.0f;
        }

        return (_targetCharacter.transform.position - transform.position).sqrMagnitude;
    }

    public virtual void RecognizeCharacter(Collider collider)
    {
        CharacterBase character = collider.gameObject.GetComponentInParent<CharacterBase>();
        if (character != null)
        {
            if (character.IsActive &&
                !IsCharacterRecognized(character) &&
                IsInView(character))
            {
                _hate.Add(character, 1);
                if (_targetCharacter == null)
                {
                    _targetCharacter = character;
                }
            }
        }
    }

    public virtual void LostCharacter(Collider collider)
    {
        CharacterBase character = collider.gameObject.GetComponentInParent<CharacterBase>();
        if (character != null)
        {
            if (IsCharacterRecognized(character))
            {
                _hate.Remove(character);
                if (_targetCharacter == character)
                {
                    _targetCharacter = GetCharacterHaveMostHate();
                }
            }
        }
    }

    protected bool IsCharacterRecognized(CharacterBase character)
    {
        foreach(CharacterBase chara in _hate.Keys)
        {
            if(character == chara)
            {
                return true;
            }
        }
        return false;
    }

    protected CharacterBase GetCharacterHaveMostHate()
    {
        int hate = 0;
        CharacterBase character = null;
        foreach (CharacterBase chara in _hate.Keys)
        {
            if(chara.IsActive)
            {
                if (character == null)
                {
                    character = chara;
                    hate = _hate[chara];
                }
                else
                {
                    if (_hate[chara] > hate)
                    {
                        character = chara;
                        hate = _hate[chara];
                    }
                }
            }
        }
        return character;
    }
    protected override void Update()
    {
        if(_eyesTransform != null)
        {
            _eyesPosition = _eyesTransform.position;
        }
        else
        {
            _eyesPosition = transform.position;
        }
        RemoveNull();
        base.Update();
    }

    private bool IsInView(CharacterBase character)
    {
        Vector3 dir = character.transform.position - _eyesPosition;
        Ray ray = new Ray(_eyesPosition, dir);
        RaycastHit hit;
        int layerNum = LayerMask.NameToLayer("Stage");
        int layerMask = 1 << layerNum;

        return !Physics.Raycast(ray, out hit, dir.magnitude, layerMask);
    }

    private void RemoveNull()
    {
        Dictionary<CharacterBase, int> hate = new Dictionary<CharacterBase, int>(_hate);
        foreach (CharacterBase chara in hate.Keys)
        {
            if(chara == null || !chara.IsActive || !chara.gameObject.activeSelf)
            {
                _hate.Remove(chara);
                _targetCharacter = GetCharacterHaveMostHate();
            }
        }
    }
}
