using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{
    enum State
    {
        Idle,
        Summon,
        Absorb
    }

    // フィールド
    [SerializeField] private PlayerData _playerData;
    private State _state = State.Idle;

    // プロパティ
    public int ColorCapacity { get {  return _playerData.ColorCapacity; } }
    public int AbsorbPower { get { return _playerData.AbsorbPower; } }
    public int SummonMax { get {  return _playerData.SummonMax; } }

    public bool IsAbsorbing()
    {
        if (_state == State.Absorb)
        {
            return true;
        }
        return false;
    }

    public bool ChangeToAbsorb()
    {
        _state = State.Absorb;
        return true;
    }

    public void TurnToIdle()
    {
        _state = State.Idle;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        if (_playerData == null)
        {
            Debug.LogError("Player Data is Null!!");
            return;
        }
        _characterData = _playerData;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base .Update();
    }
}
