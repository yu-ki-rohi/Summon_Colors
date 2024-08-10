using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterData/Player",fileName = "PlayerData")]
public class PlayerData : CharacterData
{
    public int ColorCapacity = 255;
    public int AbsorbPower = 10;
    public int SummonMax = 3;
    public int Money = 0;
}
