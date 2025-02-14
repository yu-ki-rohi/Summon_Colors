using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InGameBase
{
    public float GameTime = 180.0f;
    public Timer GameTimer;
    public SummonedPool[] SummonedPools;
    public UIManager UIManager;
    public CameraMove CameraMove;
    private bool _isClear = false;
    private bool _isPausing = false;
    private int _enemyNum = 0;
    private int _defeatNum = 0;
    private int _enegyAmount = 0;
    private int _damageAmount = 0;
    private int _continueNum = 0;

 
    public bool IsClear { get { return _isClear; } }

    public int GetSummonedsNum()
    {
        int num = 0;
        foreach (var pool in SummonedPools)
        {
            num += pool.GetActiveNum();
        }
        return num;
    }

    public void SetSummonedPools(SummonedPool[] pools)
    {
        SummonedPools = pools;
    }

    public void AddEnemyNum()
    {
        _enemyNum++;
    }

    public void DefeatEnemy()
    {
        _defeatNum++;
        int activeNum = _enemyNum - _defeatNum;
        bool isEnemy = UIManager.SetEnemyNum(_enemyNum - _defeatNum);
        if (isEnemy && activeNum == 0)
        {
            InGameManager.Instance.GameClear();
        }
    }

    public void AbsorbColor(int energy)
    {
        _enegyAmount += energy;
    }

    public void Damage(int damage)
    {
        _damageAmount += damage;
    }

    public void Continue()
    {
        _continueNum++;
    }

    public virtual void OnGameClear()
    {
        _isClear = true;
    }

    public virtual void OnGameOver()
    {

    }
    
    public virtual void OnTimeUp()
    {
        GameTimer = null;
        UIManager.ReflectTime(0);
    }

    public virtual void Start()
    {
        GameTimer = new Timer(OnTimeUp, GameTime);
        GameTimer.PrepareCountDown();
        UIManager.ChangeAlpha(0);
        UIManager.ChoiceView(false);
    }


    public virtual void Update(float elapsedTime)
    {

        if(IsClear) { return; }
        CountTime(elapsedTime);
    }

    private void CountTime(float elapsedTime)
    {
        if (GameTimer == null) { return;  }
        UIManager.ReflectTime(GameTimer.CurrentTime);
        GameTimer.CountDown(elapsedTime);
    }
}
