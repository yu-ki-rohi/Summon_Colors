using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : CharacterBase
{
    protected CharacterBase _targetCharacter = null;
    protected Dictionary<CharacterBase,int> _hate = new Dictionary<CharacterBase,int>();

    public override void Damaged(int attack, int hate = 0, CharacterBase attacker = null)
    {
        base.Damaged(attack, hate, attacker);
        if (attacker != null)
        {
            if(IsCharacterRecognized(attacker))
            {
                _hate[attacker] += hate;
                if(_hate[attacker] > _hate[_targetCharacter])
                {
                    _targetCharacter = attacker;
                }
                _targetCharacter = GetCharacterHaveMostHate();
            }
        }
    }
    public virtual void RecognizeCharacter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<CharacterBase>(out var character))
        {
            if (!IsCharacterRecognized(character) &&
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
        if (collider.gameObject.TryGetComponent<CharacterBase>(out var character))
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
            if(character == null)
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
        return character;
    }

    private bool IsInView(CharacterBase character)
    {
        Vector3 dir = character.gameObject.transform.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;
        int layerNum = LayerMask.NameToLayer("Stage");
        int layerMask = 1 << layerNum;

        if (Physics.Raycast(ray, out hit, dir.magnitude, layerMask))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
