using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HitPointBar
{
    public Image _greenHpBar;
    public Image _redHpBar;
    public float StartReduceTime = 1.0f;

    private Timer _reduceTimer;
    private float _redRemain = 1.0f;
    private bool _isReflecting = false;

    public void ReflectCurrentHp(float currentHp)
    {
        _greenHpBar.fillAmount = currentHp;
        _reduceTimer = new Timer(StartReduceRed, StartReduceTime);
    }

    public void ReflectCurrentHpImmediately(float currentHp)
    {
        _redRemain = Mathf.Clamp01(currentHp);
        _greenHpBar.fillAmount = _redRemain;
        _redHpBar.fillAmount = _redRemain;
    }

    public void CountTimer()
    {
        if(_reduceTimer == null) { return; }
        _reduceTimer.CountUp(Time.deltaTime);
    }

    public void ReduceRed()
    {
        if(!_isReflecting) { return; } 
        _redRemain -= Time.deltaTime;
        _redRemain = Mathf.Clamp01(_redRemain);
        _redHpBar.fillAmount = _redRemain;
        if(_redHpBar.fillAmount <= _greenHpBar.fillAmount)
        {
            _redHpBar.fillAmount = _greenHpBar.fillAmount;
            _isReflecting = false;
        }
    }

    private void StartReduceRed()
    {
        _isReflecting = true;
        _reduceTimer = null;
    }
}
