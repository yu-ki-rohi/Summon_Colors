using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HitPointBar
{
    public Image GreenHpBar;
    public Image RedHpBar;
    public float StartReduceTime = 1.0f;

    private Timer _reduceTimer;
    private float _redRemain = 1.0f;
    private bool _isReflecting = false;

    public void ReflectCurrentHp(float currentHp)
    {
        GreenHpBar.fillAmount = currentHp;
        _reduceTimer = new Timer(StartReduceRed, StartReduceTime);
    }

    public void ReflectCurrentHpImmediately(float currentHp)
    {
        _redRemain = Mathf.Clamp01(currentHp);
        GreenHpBar.fillAmount = _redRemain;
        RedHpBar.fillAmount = _redRemain;
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
        RedHpBar.fillAmount = _redRemain;
        if(RedHpBar.fillAmount <= GreenHpBar.fillAmount)
        {
            RedHpBar.fillAmount = GreenHpBar.fillAmount;
            _isReflecting = false;
        }
    }

    private void StartReduceRed()
    {
        _isReflecting = true;
        _reduceTimer = null;
    }
}
