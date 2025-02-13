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
        if(Hp <= 0) { return 0; };
        int beforeHp = Hp;
        int damage = base.Damaged(attack, shock, hate, attacker);
        if (damage == 0) { return damage; }
        else if (damage < 0) { damage = attack; }
        else { InGameManager.Instance.Damage(damage); }
        _uiManager.ReflectCurrentHp((float)Hp / MaxHp);
        _uiManager.ChangeToDamaged(attack);
        if (Hp <= 0)
        {
            Die();
            return damage;
        }
        DamagedInvincible(0.8f);
        if (damage <= _characterData.Armor)
        {
            AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.Damage01, 2, transform);
        }
        else
        {
            AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.BigDamage01, 2, transform); 
            if (_actionController.IsBoolAnimation())
            {
                _actionController.ChangeToIdle();
            }
        }
        float tiredBorder = 0.4f;
        int judgeTired = (int)((Hp - MaxHp * tiredBorder) * (beforeHp - MaxHp * tiredBorder));
        if (judgeTired < 0)
        {
            _uiManager.ChangeToExhausted();
            AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.Tired01, 1, transform);
        }
        
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

    public override void KnockBack(Vector3 dir, float strength, float time)
    {
        _actionController.KnockBack(dir, strength, time);
    }

    protected override void Die()
    {
        AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.GameOver01, 2, transform);
        _actionController.OnDie();
        InGameManager.Instance.GameOver();
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
