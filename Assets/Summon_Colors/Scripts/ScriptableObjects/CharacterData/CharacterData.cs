using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : ScriptableObject
{
    public int MaxHp = 100;
    public int Attack = 10;
    public int Vitality = 0;
    public int Break = 10;
    public int Armor = 50;
    public int Appearance = 10;
    public float Agility = 5.0f;
    public float CoolTime = 3.0f;
    public float[] PowerMagnifications;
}
