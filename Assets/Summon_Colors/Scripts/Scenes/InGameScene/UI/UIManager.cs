using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HitPointBar _hitPointBar;
    [SerializeField] private HitPointBar _enemyHitPointBar;
    [SerializeField] private PlayerIcon _playerIcon;
    [SerializeField] private SelectedColorDisplay _selectedColorDisplay;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private ChoicesMenu _choicesMenu;
    [SerializeField] private Image _blackOut;

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

    public void ChoiceView(bool flag, int index = 0)
    {
        _choicesMenu.View(flag, index);
    }
    #endregion
    
    #region--- Black Out ---
    public void ChangeAlpha(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        _blackOut.color = new Color(0,0,0,alpha);
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _playerIcon.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        _hitPointBar.CountTimer();
        _hitPointBar.ReduceRed();
        _enemyHitPointBar.CountTimer();
        _enemyHitPointBar.ReduceRed();

        _playerIcon.Update();
    }
}
