using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HitPointBar _hitPointBar;
    [SerializeField] private PlayerIcon _playerIcon;
    [SerializeField] private SelectedColorDisplay _selectedColorDisplay;
    [SerializeField] private TextMeshProUGUI _timerText;


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
        int minute = (int)(totalTime / 60.0f);
        float seconds = totalTime - minute * 60.0f;
        int intSeconds = (int)(seconds);
        if(seconds - intSeconds > 0)
        {
            intSeconds += 1;
        }
        _timerText.text = minute.ToString("00") + ":" + intSeconds.ToString("00");
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
        _playerIcon.Update();
    }
}
