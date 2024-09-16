using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerActionController))]
public class Player : CharacterBase
{
    // フィールド
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private Image _hpBar;
    private PlayerActionController _actionController;

    // プロパティ
    public int ColorCapacity { get {  return _playerData.ColorCapacity; } }
    public int AbsorbPower { get { return _playerData.AbsorbPower; } }
    public int SummonMax { get {  return _playerData.SummonMax; } }

    public PlayerActionController ActionController { get { return _actionController; } }

    public override void Damaged(int attack, int shock = 0, int hate = 0, CharacterBase attacker = null)
    {
        base.Damaged(attack, shock, hate, attacker);
        ReflectHp();
    }

    public override void Heal(int heal)
    {
        base.Heal(heal);
        ReflectHp();
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
        _actionController = GetComponent<PlayerActionController>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base .Update();
    }

    private void ReflectHp()
    {
        _hpBar.fillAmount = (float)Hp / MaxHp;
    }
}
