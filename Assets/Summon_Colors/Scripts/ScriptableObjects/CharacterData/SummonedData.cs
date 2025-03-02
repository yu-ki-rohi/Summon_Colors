using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterData/Summoned", fileName = "Summoned[Color]Data")]
public class SummonedData : CharacterData
{
    public ColorElements.ColorType ColorType;
    public int Costs = 10;
    public float StopDistance = 2.0f;
}
