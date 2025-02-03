using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerIcon
{

    public Image Icon;
    [Space(20)]
    public Sprite NeutralExpression;
    public Sprite DamagedExpression;
    public Sprite ExhaustedExpression;

    private Sprite _beforeExpression;
    private Timer _shakingTimer;
    private ShakeByPerlinNoise _iconShaker;
    private Vector3 _initPosition;

    public void ChangeToNeutral()
    {
        Icon.sprite = NeutralExpression;
        UpdateBE();
    }

    public void ChangeToExhausted()
    {
        Icon.sprite = ExhaustedExpression;
        UpdateBE();
    }

    public void ChangeToDamaged(float damage)
    {
        Icon.sprite = DamagedExpression;
        _shakingTimer = new Timer(FinishShake, 1.0f);
        _iconShaker = new ShakeByPerlinNoise(_shakingTimer.Time, damage, 40.0f, _initPosition);
    }

    public void Initialize()
    {
        _initPosition = Icon.transform.position;
        ChangeToNeutral();
    }

    public void Update()
    {
        if (_shakingTimer == null) { return; }
        _shakingTimer.CountUp(Time.deltaTime);
        
        if (_iconShaker == null) { return; }
        Icon.gameObject.transform.position = _iconShaker.Shake(_shakingTimer.CurrentTime);
    }

    private void FinishShake()
    {
        Icon.gameObject.transform.position = _initPosition;
        _iconShaker = null;
        _shakingTimer = null;
        Icon.sprite = _beforeExpression;
    }

    private void UpdateBE()
    {
        _beforeExpression = Icon.sprite;
    }
}
