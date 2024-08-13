using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerActionController))]
public class Player : CharacterBase
{
    // フィールド
    [SerializeField] private PlayerData _playerData;
    private PlayerActionController _actionController;

    // プロパティ
    public int ColorCapacity { get {  return _playerData.ColorCapacity; } }
    public int AbsorbPower { get { return _playerData.AbsorbPower; } }
    public int SummonMax { get {  return _playerData.SummonMax; } }

    public PlayerActionController ActionController { get { return _actionController; } }

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
        _actionController = GetComponent<PlayerActionController>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base .Update();
    }
}
