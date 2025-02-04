using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HitPointBar _hitPointBar;
    [SerializeField] private PlayerIcon _playerIcon;
    [SerializeField] private SelectedColorDisplay _selectedColorDisplay;


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
