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
    [SerializeField] private UIManager _uiManager;
    private PlayerActionController _actionController;

    // プロパティ
    public float RateOfFire { get { return _playerData.RateOfFire; } }
    public float Accuracy { get { return _playerData.Accuracy; } }
    public int ColorCapacity { get {  return _playerData.ColorCapacity; } }
    public int AbsorbPower { get { return _playerData.AbsorbPower; } }
    public int SummonMax { get {  return _playerData.SummonMax; } }

    public UIManager UIManager { get { return _uiManager; } }
    public PlayerActionController ActionController { get { return _actionController; } }

    public override int Damaged(int attack, int shock = 0, int hate = 0, CharacterBase attacker = null)
    {
        int damage = base.Damaged(attack, shock, hate, attacker);
        if (damage <= 0) { return damage; }
        _uiManager.ReflectCurrentHp((float)Hp / MaxHp);
        if (Hp < MaxHp * 0.4f)
        {
            _uiManager.ChangeToExhausted();
        }
        _uiManager.ChangeToDamaged(attack);
        DamagedInvincible(0.5f);
        Debug.Log(damage);
        return damage;
    }

    public override void Heal(int heal)
    {
        base.Heal(heal);
        _uiManager.ReflectCurrentHpImmediately((float)Hp / MaxHp);
        if(Hp > MaxHp * 0.4f)
        {
            _uiManager.ChangeToNeutral();
        }
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
