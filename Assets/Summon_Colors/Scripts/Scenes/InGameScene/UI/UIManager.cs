using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum ButtonMask
    { 
        None = 0,
        BackText = 1 << 0,
        BackButton = 1 << 1,
        Attack = 1 << 2,
        Gathering = 1 << 3,
        Avoid = 1 << 4,
        Absorb = 1 << 5,
        Change = 1 << 6,
        Summon = 1 << 7,
        Command = 1 << 8,
        All = 511
    }

    public enum ButtonState
    {
        Idle,
        Attack,
        Avoid, 
        Absorb, 
        Summon,
        Command,
    }


    [SerializeField] private HitPointBar _hitPointBar;
    [SerializeField] private HitPointBar _enemyHitPointBar;
    [SerializeField] private PlayerIcon _playerIcon;
    [SerializeField] private SelectedColorDisplay _selectedColorDisplay;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private ChoicesMenu _choicesMenu;
    [SerializeField] private FadePanel _fadePanel;
    [SerializeField] private TextMeshProUGUI _enemyNum;
    [SerializeField] private GameObject _scoreSheet;
    [SerializeField] private GameObject _rankingSheet;
    [SerializeField] private Image[] _buttonDisplays;
    [SerializeField] private Animator _buttonDisplayAnimator;
    [SerializeField] private TextMeshProUGUI _phisicsTime;
    [SerializeField] private TextMeshProUGUI _scriptsTime;
    [SerializeField] private TextMeshProUGUI _otherTime;
    [SerializeField] private TextMeshProUGUI _fps;
    int _countUpdate = 0;
    float _timer = 0;

   

    #region--- Hit Point bar ---
    public void ReflectCurrentHp(float currentHp)
    {
        _hitPointBar?.ReflectCurrentHp(currentHp);
    }

    public void ReflectCurrentHpImmediately(float currentHp)
    {
        _hitPointBar?.ReflectCurrentHpImmediately(currentHp);
    }
    #endregion

    #region--- Enemy Hit Point bar ---
    public void ReflectEnemyHp(float currentHp)
    {
        _enemyHitPointBar?.ReflectCurrentHp(currentHp);
    }

    public void ReflectEnemyHpImmediately(float currentHp)
    {
        _enemyHitPointBar?.ReflectCurrentHpImmediately(currentHp);
    }
    #endregion

    #region--- Player Icon ---
    public void ChangeToNeutral()
    {
        _playerIcon.ChangeToNeutral();
    }

    public void ChangeToExhausted()
    { 
        _playerIcon.ChangeToExhausted();
    }

    public void ChangeToDamaged(float damage)
    {
        _playerIcon.ChangeToDamaged(damage);
    }

    #endregion

    #region--- Selected Color Display ---
    public void ChangeColor(ColorElements.ColorType color)
    {
        _selectedColorDisplay.ChangeColor(color);
    }
    #endregion

    #region--- Timer Text---
    public void ReflectTime(float totalTime)
    {
        if(_timerText == null) { return; }
        if(totalTime > 0) { totalTime += 1; }
        int minute = (int)(totalTime / 60.0f);
        float seconds = totalTime - minute * 60.0f;
        _timerText.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00");
    }
    #endregion

    #region--- Choice Menu ---
    public void ChoiceCursor(int index)
    {
        _choicesMenu.ChoiceCursor(index);
    }

    public void ChoiceScaling(int index)
    {
        _choicesMenu.ChoiceScaling(index);
    }

    public void ChoiceView(bool flag, int index = 0)
    {
        _choicesMenu.View(flag, index);
    }
    #endregion

    #region --- Enemy Num ---
    public bool SetEnemyNum(int num)
    {
        if (_enemyNum == null) { return false; }
        _enemyNum.text = num.ToString("00");
        return true;
    }
    #endregion

    #region--- Black Out ---

    public float GetFadeAlpha()
    {
        return _fadePanel.Alpha;
    }
    public void ChangeAlpha(float alpha)
    {
        _fadePanel.ChangeAlpha(alpha);
    }

    public void ChangeColor(float red, float green, float blue)
    {
        _fadePanel.ChangeColor(red, green, blue);
    }

    #endregion

    #region--- Score ---
    public void ViewScore()
    {
        _scoreSheet.SetActive(true);
    }
    #endregion

    #region--- Ranking ---
    public void ViewRanking()
    {
        _rankingSheet.SetActive(true);
    }
    #endregion

    #region--- Button Display ---
    public void SwitchViewBackButtonDisplay(bool isView)
    {
        int mask = (int)ButtonMask.All;
        if(!isView)
        {
            mask &= ~((int)ButtonMask.BackButton | (int)ButtonMask.BackText);
        }
        SwitchViewButtonDisplay(mask);
    }

    public void SwitchViewButtonDisplay(int mask)
    {
        for (int i = _buttonDisplays.Length - 1; i >= 0; i--)
        {
            int pow = 1;
            for (int j = 0; j < i; j++)
            {
                pow *= 2;
            }
            if (mask >= pow)
            {
                _buttonDisplays[i].enabled = true;
                mask -= pow;
            }
            else
            {
                _buttonDisplays[i].enabled = false;
            }
        }
    }
    public void SwitchViewButtonDisplay(ButtonState state)
    {
        switch(state)
        {
            case ButtonState.Idle:
                _buttonDisplayAnimator.SetTrigger("Idle");
                break;
            case ButtonState.Attack: 
                break;
            case ButtonState.Avoid: 
                break;
            case ButtonState.Absorb: 
                break;
            case ButtonState.Summon: 
                break;
            case ButtonState.Command:
                _buttonDisplayAnimator.SetTrigger("Command");
                break;

        }
    }

    public void ChangeButtonColor(int mask, Color color)
    {
        for (int i = _buttonDisplays.Length - 1; i >= 0; i--)
        {
            int pow = 1;
            for (int j = 0; j < i; j++)
            {
                pow *= 2;
            }
            if (mask >= pow)
            {
                mask -= pow;
                if (_buttonDisplays[i].enabled == false) 
                {
                    _buttonDisplays[i].enabled = true;
                    _buttonDisplays[i].color = color;
                    _buttonDisplays[i].enabled = false;
                }
                else
                {
                    _buttonDisplays[i].color = color;
                }
            }
        }
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _playerIcon.Initialize();
        _choicesMenu.Start();
#if UNITY_EDITOR
        StartCoroutine(ViewFPS());
#else
        _phisicsTime.enabled = false;
        _scriptsTime.enabled = false;
        _otherTime.enabled = false;
        _fps.enabled = false;
#endif

    }
    private void FixedUpdate()
    {
#if UNITY_EDITOR
        _otherTime.text = (Time.time - _timer).ToString("0.0000");
#else
#endif
        _timer = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        _phisicsTime.text = Time.deltaTime.ToString("0.0000");
#else
#endif
        _hitPointBar.CountTimer();
        _hitPointBar.ReduceRed();
        _enemyHitPointBar.CountTimer();
        _enemyHitPointBar.ReduceRed();

        _playerIcon.Update();
        _countUpdate++;
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        _scriptsTime.text = (Time.time - _timer).ToString("0.0000");
#else
#endif

        _timer = Time.time;
    }

    private IEnumerator ViewFPS()
    {
        while (true)
        {
            _fps.text = _countUpdate.ToString();
            _countUpdate = 0;
            yield return new WaitForSeconds(1.0f);
        }
    }
}
