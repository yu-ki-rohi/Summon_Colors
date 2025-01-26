using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterData/Player",fileName = "PlayerData")]
public class PlayerData : CharacterData
{
    public float RateOfFire = 30.0f;
    public float Accuracy = 1.0f;
    public int ColorCapacity = 255;
    public int AbsorbPower = 10;
    public int SummonMax = 3;
    public int Money = 0;

    public bool PayMoney(int amount)
    {
        if(Money < amount)
        {
            return false;
        }

        Money -= amount;
        return true;
    }
}
