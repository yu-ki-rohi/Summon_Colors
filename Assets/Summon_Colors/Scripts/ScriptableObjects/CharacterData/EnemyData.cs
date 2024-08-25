using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterData/Enemy", fileName = "EnemyData")]
public class EnemyData : CharacterData
{
    public float StopDistance = 2.0f;
    public int Money;
}
