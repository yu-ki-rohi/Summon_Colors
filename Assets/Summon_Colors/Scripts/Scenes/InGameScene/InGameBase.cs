using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class InGameBase
{
    public float GameTime = 180.0f;
    public Timer GameTimer;
    public SummonedPool[] SummonedPools;
    public UIManager UIManager;
    public CameraMove CameraMove;

    public TextMeshProUGUI[] _scoreTexts;
    private bool _isClear = false;
    private bool _isPausing = false;
    private int _enemyNum = 0;
    private int _defeatNum = 0;
    private int _enegyAmount = 0;
    private int _damageAmount = 0;
    private int _continueNum = 0;

 
    public bool IsClear { get { return _isClear; } }

    public int GetActiveSummonedsNum()
    {
        int num = 0;
        foreach (var pool in SummonedPools)
        {
            num += pool.GetActiveNum();
        }
        return num;
    }
    public int GetSummonedsNum()
    {
        int num = 0;
        foreach (var pool in SummonedPools)
        {
            num += pool.SummonedNum;
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
        UIManager.SetEnemyNum(_enemyNum - _defeatNum);
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

    public void ViewScore()
    {
        UIManager.ViewScore();
        int[] scores = new int[7];
        scores[0] = 10000 + (_defeatNum - 1) * 500;
        scores[1] = (int)(100 * (GameTime - GameTimer.CurrentTime));
        scores[2] = 100 * GetSummonedsNum();
        scores[3] = -_enegyAmount;
        scores[4] = -_damageAmount;
        scores[5] = -_continueNum * 2000;
        for(int i = 0; i < scores.Length - 1; i++)
        {
            scores[scores.Length - 1] += scores[i];
        }
        for(int i = 0; i < _scoreTexts.Length; i++)
        {
            _scoreTexts[i].enabled = true;
            _scoreTexts[i].text = scores[i].ToString();
            _scoreTexts[i].enabled = false;
        }
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
        GameTimer = new Timer(null, -1);
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
        GameTimer.CountUp(elapsedTime);
    }
}
